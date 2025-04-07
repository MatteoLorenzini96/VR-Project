using UnityEngine;

public class TargetCollisionHandler : MonoBehaviour
{
    private HealthManager _healthManager;

    private void Start()
    {
        _healthManager = GetComponentInParent<HealthManager>();
        if (_healthManager == null)
        {
            Debug.LogError("HealthManager non trovato nel parent!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SnowBall"))
        {
            if (_healthManager != null)
            {
                _healthManager.TakeDamage();
            }
        }
    }
}
