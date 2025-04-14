using UnityEngine;
using System.Collections;

public class TutorialTurret : MonoBehaviour
{
    [Header("Distruzione VFX and SFX")]
    [SerializeField] private string _turretDestructionVFXName = "SnowBlockCreationEffect";
    [SerializeField] private string _turretDestructionSFXName = "TurretDestructionSound";

    private WaveManager _waveManager;
    private Animations _animations;

    GameObject _setTutorial;

    [SerializeField] public bool _skipTurret = false;

    private bool _isDead = false;

    private void Start()
    {
        _animations = GetComponent<Animations>();
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
            _animations.BegingTargetUp();

            _waveManager.ActivateWaves();
            _isDead = true;
            Destroy(_setTutorial);
            StartCoroutine(DestroyAfterDelay(1.5f));
        }
        return;
    }

    public void SkipWavesTimer()
    {
        if (!_isDead)
        {
            _animations.BegingTargetUp();

            _waveManager.SkipDelayBetweenWaves();
            _isDead = true;
            StartCoroutine(DestroyAfterDelay(1.5f));
        }
        return;
    }

    public void DeleteTurret()
    {
        if (!_isDead)
        {
            _animations.BegingTargetUp();

            _isDead = true;
            StartCoroutine(DestroyAfterDelay(1.5f));
        }
        return;
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        VFXManager.Instance.SpawnEffect(_turretDestructionVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_turretDestructionSFXName);
        Destroy(gameObject);
    }

}
