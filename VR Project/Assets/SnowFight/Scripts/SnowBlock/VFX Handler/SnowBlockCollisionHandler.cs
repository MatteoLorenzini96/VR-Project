using UnityEngine;

public class SnowBlockCollisionHandler : MonoBehaviour
{
    private HealthManager _healthManager;

    void Start()
    {
        _healthManager = GetComponentInParent<HealthManager>();
        if (_healthManager == null)
        {
            Debug.LogError("HealthManager non trovato nel parent!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (CompareTag("Bullet"))
        {
            if (_healthManager != null)
            {
                _healthManager.TakeDamage();
            }
        }
    }

}
