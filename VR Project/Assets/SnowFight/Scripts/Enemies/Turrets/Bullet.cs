using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Distruzione VFX and SFX")]
    [SerializeField] private string _bulletDestructionVFXName = "BulletDestructionEffect";
    [SerializeField] private string _bulletDestructionSFXName = "BulletDestructionSound";

    [Header("Max Lifetime")]
    [SerializeField] private float _lifeTime = 15f;

    private Rigidbody _rigidbody;
    private bool _isDead = false;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 direction, float speed)
    {
        _rigidbody.linearVelocity = direction * speed;
        DestroyAfterTime();
    }

    private void OnCollisionEnter(Collision collision)
    {
        VFXManager.Instance.SpawnEffect(_bulletDestructionVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_bulletDestructionSFXName);

        if (collision.gameObject.CompareTag("Player") ||
            collision.gameObject.CompareTag("SnowWall") ||
            collision.gameObject.CompareTag("SnowBlock"))
        {
            HealthManager healthManager = collision.gameObject.GetComponentInParent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.TakeDamage();
                //Debug.Log("Colpito");
            }
            else
            {
                Debug.LogError("HealthManager non trovato sull'oggetto");
            }
        }
        Destroy(gameObject);
        _isDead = true;
    }

    private void DestroyAfterTime()
    {
        if (!_isDead)
        {
            Destroy(gameObject, _lifeTime);
        }
    }
}
