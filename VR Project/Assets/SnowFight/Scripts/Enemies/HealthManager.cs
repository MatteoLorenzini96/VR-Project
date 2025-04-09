using Unity.VisualScripting;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int lives = 1;

    private BombAI _bombAI;
    private TurretAI _turretAI;
    private SnowBlockHealth _snowBlockHealth;

    private void Start()
    {
        _bombAI = GetComponent<BombAI>();
        _turretAI = GetComponent<TurretAI>();
        _snowBlockHealth = GetComponent<SnowBlockHealth>();


        if (_bombAI == null && _turretAI == null && _snowBlockHealth == null)
        {
            Debug.LogWarning("Né BombAI né TurretAI né SnowBlockHealth trovati su " + gameObject.name);
        }
    }

    public void TakeDamage()
    {
        lives--;

        Debug.Log($"{gameObject.name} ha subito un colpo. Vite rimaste: {lives}");

        if (_snowBlockHealth != null)
        {
            _snowBlockHealth.GettingDamaged();
            return;
        }
        else if (_turretAI != null)
        {
            _turretAI.HandleDamage();
        }

        if (lives <= 0)
        {
            if (_bombAI != null)
            {
                _bombAI.Explode();
            }

            else
            {
                Debug.LogWarning("Nessuna logica di distruzione definita!");
            }
        }
    }
}
