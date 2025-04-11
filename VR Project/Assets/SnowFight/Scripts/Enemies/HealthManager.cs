using Unity.VisualScripting;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int lives = 1;

    private BombAI _bombAI;
    private TurretAI _turretAI;
    private SnowBlockHealth _snowBlockHealth;
    private TutorialTurret _tutorialTurret;
    private PlayerHealthController _playerHealthController;

    private void Start()
    {
        _bombAI = GetComponent<BombAI>();
        _turretAI = GetComponent<TurretAI>();
        _snowBlockHealth = GetComponent<SnowBlockHealth>();
        _tutorialTurret = GetComponent<TutorialTurret>();
        _playerHealthController = GetComponent<PlayerHealthController>();


        if (_bombAI == null && _turretAI == null && _snowBlockHealth == null && _tutorialTurret == null && _playerHealthController == null)
        {
            Debug.LogWarning("Né BombAI né TurretAI né SnowBlockHealth nè TutorialTurret nè PlayerHealthController trovati su " + gameObject.name);
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
        else if (_playerHealthController != null)
        {
            _playerHealthController.GettingDamaged();
        }
    }

    public void PlayerExploded()
    {
        lives -= 5;
        _playerHealthController.GettingDamaged();
        Debug.Log($"{gameObject.name} è esploso. Vite diminuite di 5. Vite attuali: {lives}");
    }
}
