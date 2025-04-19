using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject canvasRoot; // Usa GameObject invece di Canvas per attivare/disattivare tutto

    private const string LastWaveKey = "LastWave";

    // Attiva il canvas world-space
    public void ShowCanvas()
    {
        if (canvasRoot != null)
        {
            canvasRoot.SetActive(true);
        }
        else
        {
            //Debug.LogWarning("Canvas Root non assegnato nell'Inspector!");
        }
    }

    // Reset completo della scena
    public void ResetScene()
    {
        PlayerPrefs.DeleteKey(LastWaveKey);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Reset mantenendo la wave salvata
    public void ResetSceneToLastWave()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Utile per aggiornare da altri oggetti XR se serve
    public void UpdateLastWave(int currentWave)
    {
        PlayerPrefs.SetInt(LastWaveKey, currentWave);
        PlayerPrefs.Save();
    }

    public int GetLastWave()
    {
        return PlayerPrefs.GetInt(LastWaveKey, 0);
    }

    public void ExitFromGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
