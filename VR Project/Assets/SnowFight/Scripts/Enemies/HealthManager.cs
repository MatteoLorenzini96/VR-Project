using Unity.VisualScripting;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public int lives = 1;

    private int _maxLives;
    private BombAI _bombAI;
    private TurretAI _turretAI;
    private SnowBlockHealth _snowBlockHealth;
    private TutorialTurret _tutorialTurret;
    private PlayerHealthController _playerHealthController;

    private void Start()
    {
        _maxLives = lives;
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
            if (_tutorialTurret._skipTurret == false)
            {
                _tutorialTurret.ActivateWavesManager();
            }
            else
            {
                _tutorialTurret.SkipWavesTimer();
            }
        }
        else if (_playerHealthController != null)
        {
            _playerHealthController.GettingDamaged();
        }
    }

    public void TakeExplosion(int damage)
    {
        lives -= damage;
        Debug.Log($"{gameObject.name} ha subito un'esplosione. Danno: {damage}. Vite rimaste: {lives}");

        if (_snowBlockHealth != null)
        {
            _snowBlockHealth.GettingDamaged();
            Debug.Log("Aggiorno la vita del Blocco");
            return;
        }
        else if (_playerHealthController != null)
        {
            _playerHealthController.GettingDamaged();
            Debug.Log("Aggiorno la vita del Player");
            return;
        }
    }

    public void HealthRecovery(int amount)
    {
        int oldLives = lives;
        lives = Mathf.Min(lives + amount, _maxLives);
        _playerHealthController.UpdateVignette();
        _playerHealthController.FlashScreen(Color.green);
        Debug.Log($"{gameObject.name} ha recuperato {lives - oldLives} vite. Vite attuali: {lives}");
    }
}
