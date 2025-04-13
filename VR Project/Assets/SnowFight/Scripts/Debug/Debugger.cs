using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Debugger : MonoBehaviour
{
    private HealthManager _playerHealthManager;
    private WaveManager _waveManager;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _playerHealthManager = player.GetComponent<HealthManager>();
            if (_playerHealthManager == null)
            {
                Debug.LogError("HealthManager non trovato sul GameObject Player.");
            }
        }
        else
        {
            Debug.LogError("Player non trovato con il tag 'Player'.");
        }

        _waveManager = FindAnyObjectByType<WaveManager>();
        if (_waveManager == null)
        {
            Debug.LogError("WaveManager non trovato.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC premuto: uscita dal gioco.");

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        if (Input.GetKeyUp(KeyCode.N))
        {
            if (_playerHealthManager != null)
            {
                _playerHealthManager.TakeDamage();
            }
            else
            {
                Debug.LogWarning("HealthManager non è inizializzato.");
            }
        }

        if (Input.GetKeyUp(KeyCode.M))
        {
            if (_playerHealthManager != null)
            {
                _playerHealthManager.HealthRecovery(1);
            }
            else
            {
                Debug.LogWarning("HealthManager non è inizializzato.");
            }
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            if (_waveManager != null)
            {
                _waveManager.SkipDelayBetweenWaves();
            }
            else
            {
                Debug.LogWarning("WaveManager non è inizializzato.");
            }
        }
    }
}
