using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int lives = 1;

    private BombAI _bombAI;
    private TurretAI _turretAI;

    private void Start()
    {
        _bombAI = GetComponent<BombAI>();
        _turretAI = GetComponent<TurretAI>();

        if (_bombAI == null && _turretAI == null)
        {
            Debug.LogWarning("Né BombAI né TurretAI trovati su " + gameObject.name);
        }
    }

    public void TakeDamage()
    {
        lives--;

        Debug.Log($"{gameObject.name} ha subito un colpo. Vite rimaste: {lives}");

        if (lives <= 0)
        {
            if (_bombAI != null)
            {
                _bombAI.Explode();
            }
            else if (_turretAI != null)
            {
                _turretAI.HandleDestruction();
            }
            else
            {
                Debug.LogWarning("Nessuna logica di distruzione definita!");
            }
        }
    }
}
