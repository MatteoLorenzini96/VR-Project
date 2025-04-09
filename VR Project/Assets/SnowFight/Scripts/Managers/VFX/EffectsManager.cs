using System.Collections.Generic;
using UnityEngine;

public class VFXManager : MonoBehaviour
{
    [System.Serializable]
    public class EffectEntry
    {
        public string effectName; // Nome dell'effetto
        public GameObject effectPrefab; // Prefab dell'effetto
    }

    public static VFXManager Instance; // Singleton

    [SerializeField]
    private List<EffectEntry> effectsList = new List<EffectEntry>();

    private Dictionary<string, GameObject> effectsDictionary;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Mantieni l'oggetto al cambio di scena
        }
        else
        {
            Destroy(gameObject);
        }

        // Crea un dizionario per un accesso rapido agli effetti
        effectsDictionary = new Dictionary<string, GameObject>();
        foreach (var entry in effectsList)
        {
            if (!effectsDictionary.ContainsKey(entry.effectName))
            {
                effectsDictionary.Add(entry.effectName, entry.effectPrefab);
            }
            else
            {
                Debug.LogWarning($"Effetto con nome duplicato: {entry.effectName}. Sarà ignorato.");
            }
        }
    }

    /// <summary>
    /// Instanzia un effetto in una posizione specifica con una rotazione specifica e restituisce l'oggetto istanziato.
    /// </summary>
    /// <param name="effectName">Nome dell'effetto.</param>
    /// <param name="position">Posizione dove istanziare l'effetto.</param>
    /// <param name="rotation">Rotazione dell'effetto.</param>
    /// <returns>Il GameObject istanziato.</returns>
    public GameObject SpawnEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        if (effectsDictionary.TryGetValue(effectName, out var effectPrefab))
        {
            if (effectPrefab != null)
            {
                return Instantiate(effectPrefab, position, rotation);
            }
            else
            {
                Debug.LogWarning($"Il prefab associato a '{effectName}' è null. L'effetto non verrà istanziato.");
                return null;
            }
        }
        else
        {
            Debug.LogWarning($"Effetto non trovato: '{effectName}'. Assicurati che sia nella lista del VFXManager.");
            return null;
        }
    }

    /// <summary>
    /// Instanzia un effetto nella posizione e rotazione predefinita (identity) e restituisce l'oggetto istanziato.
    /// </summary>
    /// <param name="effectName">Nome dell'effetto.</param>
    /// <param name="position">Posizione dove istanziare l'effetto.</param>
    /// <returns>Il GameObject istanziato.</returns>
    public GameObject SpawnEffect(string effectName, Vector3 position)
    {
        return SpawnEffect(effectName, position, Quaternion.identity); // Chiama la versione con rotazione predefinita
    }
}
