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

    private AudioSource _tickingAudioSource;

    private void Start()
    {
        SearchForPlayer();
        SearchForIdentifier();
        PlayTickingSound();
        LookAtPlayer();
    }

    private void FixedUpdate()
    {
        if (_playerTransform != null && !_isExploding && isMoving)
        {
            MoveTowardsPlayer();
            UpdateTickingSound();
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
        // Usa AudioManager per iniziare a riprodurre il suono di ticking
        AudioManager.Instance.PlaySFX(_bombTickingSFXName);

        // Trova l'AudioSource attualmente in uso dal AudioManager
        _tickingAudioSource = AudioManager.Instance.sfxSource; // Assumendo che sfxSource stia riproducendo il suono
        if (_tickingAudioSource == null)
        {
            Debug.LogWarning("Nessun AudioSource trovato per il ticking sound!");
        }
    }

    private void UpdateTickingSound()
    {
        if (_tickingAudioSource != null && _playerTransform != null)
        {
            // Calcola la distanza tra la bomba e il giocatore
            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            // Aggiorna pitch e velocità in base alla distanza (più vicino = più veloce)
            float pitchMultiplier = Mathf.Clamp(1f / distanceToPlayer, 1f, 3f); // Configura i valori limite come preferito
            _tickingAudioSource.pitch = pitchMultiplier; // Modifica la velocità di riproduzione
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

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, _explosionRadius);
    }
}
