using UnityEngine;

public class TutorialTurret : MonoBehaviour
{
    private WaveManager _waveManager;
    GameObject _setTutorial;

    [SerializeField] public bool _skipTurret = false;

    private bool _isDead = false;

    private void Start()
    {
        _waveManager = FindAnyObjectByType<WaveManager>();
        _setTutorial = GameObject.FindGameObjectWithTag("Tutorial");
        if (_setTutorial == null)
        {
            return;
        }
    }

    public void ActivateWavesManager()
    {
        if (!_isDead)
        {
            _waveManager.ActivateWaves();
            _isDead = true;
            Destroy(_setTutorial);
            Destroy(gameObject);
        }
        return;
    }

    public void SkipWavesTimer()
    {
        if (!_isDead)
        {
            _waveManager.SkipDelayBetweenWaves();
            _isDead = true;
            Destroy(gameObject);
        }
        return;
    }

    public void DeleteTurret()
    {
        if (!_isDead)
        {
            _isDead = true;
            Destroy(gameObject);
        }
        return;
    }
}
