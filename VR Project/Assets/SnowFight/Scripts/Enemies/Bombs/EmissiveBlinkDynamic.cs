using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EmissiveBlinkDynamic : MonoBehaviour
{
    [Header("Emission Settings")]
    public Color emissionColor = Color.white;
    public float minIntensity = 0f;
    public float maxIntensity = 1f;
    public float baseBlinkSpeed = 1f;

    [Header("Dynamic Blink Settings")]
    public float maxSpeedMultiplier = 3f;
    public float distanceThreshold = 10f;

    [Header("Blink Sound")]
    public AudioClip blinkSFX;
    public float blinkSFXVolume = 1f;

    private Material material;
    private Transform playerTransform;
    private AudioSource audioSource;
    private bool blinkSoundPlayedThisCycle = false;

    void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        // Creiamo una copia del materiale per evitare modifiche globali
        material = Instantiate(rend.sharedMaterial);
        rend.material = material;
        material.EnableKeyword("_EMISSION");

        // Non rimuovere l'emission map, così le parti specificate rimangono attive
        // material.SetTexture("_EmissionMap", null);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player non trovato! Assicurati che l'oggetto abbia il tag 'Player'.");
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        float dynamicBlinkSpeed = baseBlinkSpeed;
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            if (distance < distanceThreshold)
            {
                float multiplier = Mathf.Clamp(distanceThreshold / distance, 1f, maxSpeedMultiplier);
                dynamicBlinkSpeed *= multiplier;
            }
        }

        float emission = Mathf.PingPong(Time.time * dynamicBlinkSpeed, maxIntensity - minIntensity) + minIntensity;

        // Resetta la possibilità di suonare il Blink SFX quando il ciclo ricomincia
        if (emission <= minIntensity + 0.05f)
        {
            blinkSoundPlayedThisCycle = false;
        }
        if (!blinkSoundPlayedThisCycle && emission > minIntensity + (maxIntensity - minIntensity) * 0.1f)
        {
            if (blinkSFX != null)
            {
                audioSource.PlayOneShot(blinkSFX, blinkSFXVolume);
            }
            blinkSoundPlayedThisCycle = true;
        }

        // Modula il colore emissivo: qui _EmissionColor viene moltiplicato per la tua emission map
        Color finalColor = emissionColor * Mathf.LinearToGammaSpace(emission);
        material.SetColor("_EmissionColor", finalColor);
    }
}
