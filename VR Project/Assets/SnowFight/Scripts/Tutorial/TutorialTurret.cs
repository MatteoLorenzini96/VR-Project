using UnityEngine;

public class TutorialTurret : MonoBehaviour
{
    private WaveManager _waveManager;

    private void Start()
    {
        _waveManager = FindAnyObjectByType<WaveManager>();
    }
    public void ActivateWavesManager()
    {
        _waveManager.ActivateWaves();
        Destroy(gameObject);
    }
}
