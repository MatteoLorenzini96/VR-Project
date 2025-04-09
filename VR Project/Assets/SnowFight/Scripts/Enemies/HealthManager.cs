using Unity.VisualScripting;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int lives = 1;

    private BombAI _bombAI;
    private TurretAI _turretAI;
    private SnowBlockHealth _snowBlockHealth;
    private TutorialTurret _tutorialTurret;

    private void Start()
    {
        _bombAI = GetComponent<BombAI>();
        _turretAI = GetComponent<TurretAI>();
        _snowBlockHealth = GetComponent<SnowBlockHealth>();
        _tutorialTurret = GetComponent<TutorialTurret>();


        if (_bombAI == null && _turretAI == null && _snowBlockHealth == null && _tutorialTurret == null)
        {
            Debug.LogWarning("Né BombAI né TurretAI né SnowBlockHealth nè TutorialTurret trovati su " + gameObject.name);
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
        else if (_bombAI != null)
        {
            _bombAI.Explode();
        }
        else if (_tutorialTurret != null)
        {
            _tutorialTurret.ActivateWavesManager();
        }
    }
}
