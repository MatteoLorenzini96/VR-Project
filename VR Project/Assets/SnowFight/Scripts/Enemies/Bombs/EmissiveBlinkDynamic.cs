using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EmissiveBlinkDynamic : MonoBehaviour
{
    [Header("Emission Settings")]
    [Tooltip("Colore di base per l'emissione")]
    public Color emissionColor = Color.white;
    [Tooltip("Intensità minima dell'emissione")]
    public float minIntensity = 0f;
    [Tooltip("Intensità massima dell'emissione")]
    public float maxIntensity = 1f;
    [Tooltip("Velocità base del lampeggio (quanto veloce oscilla quando il giocatore è lontano)")]
    public float baseBlinkSpeed = 1f;

    [Header("Dynamic Blink Settings")]
    [Tooltip("Moltiplicatore massimo applicato alla velocità in base alla distanza")]
    public float maxSpeedMultiplier = 3f;
    [Tooltip("Distanza a cui il moltiplicatore inizia ad aumentare (se il giocatore è più vicino di questo valore, il lampeggio accelera)")]
    public float distanceThreshold = 10f;

    [Header("Blink Sound")]
    [Tooltip("SFX che viene riprodotto ad ogni ciclo (all'inizio del lampeggio)")]
    public AudioClip blinkSFX;
    [Tooltip("Volume del SFX di lampeggio")]
    public float blinkSFXVolume = 1f;

    // Riferimenti interni
    private Material material;
    private Transform playerTransform;
    private AudioSource audioSource;

    // Variabili per la gestione del blink SFX
    private bool blinkSoundPlayedThisCycle = false;

    void Start()
    {
        // Recupera il Renderer e crea una copia del materiale per lavorare in modo indipendente
        Renderer rend = GetComponent<Renderer>();
        // Utilizza Instantiate per creare una copia esplicita del materiale condiviso
        material = Instantiate(rend.sharedMaterial);
        rend.material = material;
        material.EnableKeyword("_EMISSION");

        // Cerca il giocatore in scena (assicurati che il GameObject del giocatore abbia il tag "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player non trovato! Assicurati che l'oggetto abbia il tag 'Player'.");
        }

        // Prepara l'AudioSource per riprodurre il blink SFX
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        // Calcola la velocità di lampeggio dinamica
        float dynamicBlinkSpeed = baseBlinkSpeed;
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            // Se il giocatore è dentro la soglia, aumenta la velocità con un moltiplicatore
            if (distance < distanceThreshold)
            {
                float multiplier = Mathf.Clamp(distanceThreshold / distance, 1f, maxSpeedMultiplier);
                dynamicBlinkSpeed *= multiplier;
            }
        }

        // Calcola il valore corrente dell'intensità usando un'oscillazione (PingPong)
        // Il range di oscillazione è (maxIntensity - minIntensity)
        float emission = Mathf.PingPong(Time.time * dynamicBlinkSpeed, maxIntensity - minIntensity) + minIntensity;

        // Controllo per riprodurre il suono del lampeggio:
        // Ogni ciclo, appena il valore esce dal minimo, riproduce il SFX (se non già riprodotto per il ciclo)
        if (Mathf.Approximately(emission, minIntensity))
        {
            // Reset della possibilità di riprodurre il SFX per il nuovo ciclo
            blinkSoundPlayedThisCycle = false;
        }
        // Quando il valore supera una soglia iniziale (es. 10% del range) e il suono non è ancora stato riprodotto nel ciclo, lo riproduce
        if (!blinkSoundPlayedThisCycle && emission > minIntensity + (maxIntensity - minIntensity) * 0.1f)
        {
            if (blinkSFX != null)
            {
                audioSource.PlayOneShot(blinkSFX, blinkSFXVolume);
            }
            blinkSoundPlayedThisCycle = true;
        }

        // Applica il colore emissivo calcolato, tenendo conto della conversione per la corretta luminosità
        Color finalColor = emissionColor * Mathf.LinearToGammaSpace(emission);
        material.SetColor("_EmissionColor", finalColor);
    }
}
