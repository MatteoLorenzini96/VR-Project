using UnityEngine;

public class BombAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _explosionRadius = 1.5f;

    [Header("VFX and SFX")]
    [SerializeField] private string _explosionVFXName = "ExplosionEffect";
    [SerializeField] private string _explosionSFXName = "ExplosionSound";

    public bool isMoving = true;

    private Transform _playerTransform;
    private EnemyIdentifier _enemyIdentifier;
    private bool _isExploding = false;
    private SphereCollider _explosionCollider;

    private void Start()
    {
        SearchForPlayer();
        SearchForIdentifier();
    }

    private void FixedUpdate()
    {
        if (_playerTransform != null && !_isExploding && isMoving)
        {
            MoveTowardsPlayer();
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

    private void MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        transform.position += directionToPlayer * _moveSpeed * Time.deltaTime;
    }

    public void TargetReached()
    {
        //Debug.Log("Bersaglio raggiunto - Movimento fermato.");
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

        //Debug.Log("Esplosione avvenuta");
        VFXManager.Instance.SpawnEffect(_explosionVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_explosionSFXName);
        ExplosionCheck();

        _enemyIdentifier.DestroyEnemy();
    }

    private void ExplosionCheck()
    {
        Collider[] objectsInRange = Physics.OverlapSphere(transform.position, _explosionRadius);

        foreach (Collider obj in objectsInRange)
        {
            if (obj.CompareTag("Player") || obj.CompareTag("SnowWall"))
            {
                Debug.Log("Danno inflitto a " + obj.name);
                // Qui puoi chiamare una funzione per infliggere danno agli oggetti colpiti
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, _explosionRadius);
    }
}
