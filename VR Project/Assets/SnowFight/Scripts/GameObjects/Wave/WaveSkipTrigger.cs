using UnityEngine;

public class WaveSkipTrigger : MonoBehaviour
{
    private WaveManager _waveManager;

    void Start()
    {
        _waveManager = FindAnyObjectByType<WaveManager>();

        if (_waveManager == null)
        {
            Debug.LogError("WaveManager non trovato nella scena.");
        }
    }

    public void TriggerSkip()
    {
        if (_waveManager != null)
        {
            _waveManager.SkipDelayBetweenWaves();
            Debug.Log("Salto dell'attesa tra le onde richiesto.");
        }
    }
}
