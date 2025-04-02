using UnityEngine;

public class EnemyIdentifier : MonoBehaviour
{
    private WaveManager _waveManager;
    public void SetWaveManager(WaveManager _manager)
    {
        _waveManager = _manager;
    }

    public void DestroyEnemy()
    {
        _waveManager.EnemyDied();
        Destroy(gameObject);
    }
}
