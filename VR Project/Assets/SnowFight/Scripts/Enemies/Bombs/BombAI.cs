using System;
using UnityEngine;

public class BombAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _explosionRadius = 1.5f;

    [Header("Explosion Damage")]
    [SerializeField] private int _playerMaxDamage = 5;
    [SerializeField] private int _playerMinDamage = 1;
    [SerializeField] private int _snowBlockMaxDamage = 3;
    [SerializeField] private int _snowBlockMinDamage = 1;

    [Header("Ticking SFX")]
    [SerializeField] private string _bombTickingSFXName = "BombTickingSound";

    [Header("Explosion VFX and SFX")]
    [SerializeField] private string _bombExplosionVFXName = "BombExplosionEffect";
    [SerializeField] private string _bombExplosionSFXName = "BombExplosionSound";

    public bool isMoving = true;

    private Transform _playerTransform;
    private EnemyIdentifier _enemyIdentifier;
    private bool _isExploding = false;
    private SphereCollider _explosionCollider;
    private EmissiveBlinkDynamic _emissiveControl;

    private AudioSource _tickingAudioSource;

    // Aggiunto per gestire la Point Light
    [SerializeField] private Light _pointLight;
    [SerializeField] private float _maxLightIntensity = 5f;  // Intensità massima della luce
    [SerializeField] private float _minLightIntensity = 0.2f;   // Intensità minima della luce
    [SerializeField] private float _lightSpeed = 3f;          // Velocità di blinking della luce

    private void Start()
    {
        SearchForEmissive();
        SearchForPlayer();
        SearchForIdentifier();
        PlayTickingSound();
        LookAtPlayer();
        SearchPointLight();
    }

    private void SearchPointLight()
    {
        // Trova la Point Light nel GameObject (può essere aggiunta come componente)
        _pointLight = GetComponentInChildren<Light>();
        if (_pointLight == null)
        {
            Debug.LogWarning("Point Light non trovata su " + gameObject.name + "!");
        }
    }

    private void SearchForEmissive()
    {
        _emissiveControl = GetComponentInChildren<EmissiveBlinkDynamic>();
        if (_emissiveControl == null)
        {
            Debug.LogError("DynamicEmissiveControl non trovato su " + gameObject.name + "!");
        }
        else
        {
            //Debug.Log("DynamicEmissiveControl trovato e inizializzato.");
        }
    }

    private void FixedUpdate()
    {
        if (_playerTransform != null && !_isExploding && isMoving)
        {
            MoveTowardsPlayer();
            UpdateTickingSound();
        }
        if (_emissiveControl != null)
        {
            _emissiveControl.UpdateEmissiveEffect();
        }
        // Gestiamo la luce che lampeggia man mano che la bomba si avvicina al giocatore
        if (_pointLight != null && _playerTransform != null)
        {
            UpdateLightEffect();
        }
    }

    private void LookAtPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("PlayerPoint");
        if (playerObject)
        {
            _playerTransform = playerObject.transform;
            transform.LookAt(_playerTransform);
        }
    }

    private void SearchForIdentifier()
    {
        _enemyIdentifier = GetComponent<EnemyIdentifier>();

        if (_enemyIdentifier == null)
        {
            Debug.LogError("EnemyIdentifier non trovato su " + gameObject.name + "!");
        }
    }

    private void SearchForPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player non trovato! Assicurati che l'oggetto abbia il tag 'Player'.");
        }
    }

    private void PlayTickingSound()
    {
        // Trova la clip dal AudioManager
        Sound tickingSound = Array.Find(AudioManager.Instance.sfxSounds, s => s.name == _bombTickingSFXName);

        if (tickingSound == null || tickingSound.clip == null)
        {
            Debug.LogWarning("Clip per il ticking non trovata!");
            return;
        }

        // Crea un nuovo AudioSource solo per questo suono
        _tickingAudioSource = gameObject.AddComponent<AudioSource>();
        _tickingAudioSource.clip = tickingSound.clip;
        _tickingAudioSource.loop = true;
        _tickingAudioSource.playOnAwake = false;
        _tickingAudioSource.volume = AudioManager.Instance.sfxSource.volume;
        _tickingAudioSource.pitch = 1f; // Inizialmente normale
        _tickingAudioSource.Play();
    }

    private void UpdateTickingSound()
    {
        if (_tickingAudioSource != null && _playerTransform != null)
        {
            float maxDistance = 10f;
            float distance = Vector3.Distance(transform.position, _playerTransform.position);
            float t = Mathf.Clamp01(1f - (distance / maxDistance));

            float newPitch = Mathf.Lerp(1f, 3f, t); // più vicino = pitch più alto
            float newVolume = Mathf.Lerp(1f, 3f, t); // più vicino = volume più alto

            _tickingAudioSource.pitch = newPitch;
            _tickingAudioSource.volume = newVolume;

            //Debug.Log($"Distanza: {distance:F2}, Pitch: {newPitch:F2}, Volume: {newVolume:F2}");
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        transform.position += directionToPlayer * _moveSpeed * Time.deltaTime;
    }

    public void TargetReached()
    {
        isMoving = false;
        Explode();
    }

    public void CreateExplosionCollider()
    {
        _explosionCollider = gameObject.AddComponent<SphereCollider>();
        _explosionCollider.radius = _explosionRadius;
        _explosionCollider.isTrigger = true;
    }

    public void Explode()
    {
        if (_isExploding) return;
        _isExploding = true;

        // Ferma il suono di ticking
        if (_tickingAudioSource != null)
        {
            _tickingAudioSource.Stop();
        }

        VFXManager.Instance.SpawnEffect(_bombExplosionVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_bombExplosionSFXName);

        ExplosionCheck();
        _enemyIdentifier.DestroyEnemy();
    }

    private void ExplosionCheck()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (Collider obj in objectsInRange)
        {
            float distance = Vector3.Distance(transform.position, obj.transform.position);
            float distancePercentage = Mathf.Clamp01(distance / _explosionRadius);

            int damage = 0;

            if (obj.CompareTag("Player"))
            {
                damage = Mathf.RoundToInt(Mathf.Lerp(_playerMaxDamage, _playerMinDamage, distancePercentage));
            }
            else if (obj.CompareTag("SnowWall") || obj.CompareTag("SnowBlock"))
            {
                damage = Mathf.RoundToInt(Mathf.Lerp(_snowBlockMaxDamage, _snowBlockMinDamage, distancePercentage));
            }

            if (damage > 0)
            {
                Debug.Log($"Danno inflitto a {obj.name}: {damage}");

                HealthManager hm = obj.GetComponent<HealthManager>();
                if (hm != null)
                {
                    hm.TakeExplosion(damage);
                }
            }
        }
    }

    private void UpdateLightEffect()
    {
        // Calcoliamo la distanza tra la bomba e il giocatore
        float distance = Vector3.Distance(transform.position, _playerTransform.position);

        // Modifica la velocità di blinking della luce in base alla distanza
        float blinkSpeed = Mathf.Clamp01(distance / _explosionRadius);
        float lightIntensity = Mathf.PingPong(Time.time * _lightSpeed * blinkSpeed, _maxLightIntensity - _minLightIntensity) + _minLightIntensity;

        _pointLight.intensity = lightIntensity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, _explosionRadius);
    }
}
