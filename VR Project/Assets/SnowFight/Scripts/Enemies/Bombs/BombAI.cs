using UnityEngine;

public class BombAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _explosionRadius = 5f;
    [SerializeField] private float _moveSpeed = 3f;

    [Header("VFX and SFX")]
    [SerializeField] private string _explosionVFXName = "ExplosionEffect";
    [SerializeField] private string _explosionSFXName = "ExplosionSound";

    private Transform _playerTransform;
    private EnemyIdentifier _enemyIdentifier;

    private void Start()
    {
        SearchForPlayer();
        SearchForIdentifier();
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

    private void Update()
    {
        if (_playerTransform != null)
        {
            MoveTowardsPlayer();

            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer <= _explosionRadius)
            {
                Explode();
            }
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        transform.position += directionToPlayer * _moveSpeed * Time.deltaTime;
    }

    public void Explode()
    {
        Debug.Log("Esplosione avvenuta");

        VFXManager.Instance.SpawnEffect(_explosionVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_explosionSFXName);

        // Aggiungere codice per i danni (se necessario)

        _enemyIdentifier.DestroyEnemy();
    }
}
