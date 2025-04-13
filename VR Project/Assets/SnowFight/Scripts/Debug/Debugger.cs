using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Debugger : MonoBehaviour
{
    private HealthManager _playerHealthManager;


    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerHealthManager = player.GetComponent<HealthManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC premuto: uscita dal gioco.");

#if UNITY_EDITOR
            // Se siamo nell'Editor, fermiamo la modalità Play
            EditorApplication.isPlaying = false;
#else
            // Se siamo in una build, chiudiamo l'applicazione
            Application.Quit();
#endif
        }
        if (Input.GetKeyUp(KeyCode.N))
        {
            _playerHealthManager.TakeDamage();
        }
        if (Input.GetKeyUp(KeyCode.M))
        {
            _playerHealthManager.HealthRecovery(1);
        }
    }
}
