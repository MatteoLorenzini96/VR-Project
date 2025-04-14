using UnityEngine;

public class EmissiveBlinkDynamic : MonoBehaviour
{
    [Header("Emission Settings")]
    public Color emissionColor = Color.red;
    public float minIntensity = 0.1f;  // Aumentato per migliorare la visibilità
    public float maxIntensity = 2f;    // Aumentato per migliorare la visibilità
    public float baseBlinkSpeed = 2f;  // Velocità più alta per un cambiamento più rapido

    [Header("Dynamic Blink Settings")]
    public float maxSpeedMultiplier = 3f;
    public float distanceThreshold = 10f;

    private Material material;
    private Transform playerTransform;

    private void Start()
    {
        Renderer rend = GetComponent<Renderer>();
        material = Instantiate(rend.sharedMaterial);
        rend.material = material;
        material.EnableKeyword("_EMISSION");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("Player non trovato! Assicurati che l'oggetto abbia il tag 'Player'.");
        }
    }

    // Funzione per aggiornare l'emissione in base alla distanza dal giocatore
    public void UpdateEmissiveEffect()
    {
        if (playerTransform != null)
        {
            //Debug.Log("Funzione UpdateEmissiveEffect chiamata.");
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            //Debug.Log($"Distanza dal player: {distance}");

            float dynamicBlinkSpeed = baseBlinkSpeed;

            // Modificare la velocità di blinking in base alla distanza
            if (distance < distanceThreshold)
            {
                //Debug.Log("Velocità di blinking modificata");
                float multiplier = Mathf.Clamp(distanceThreshold / distance, 1f, maxSpeedMultiplier);
                dynamicBlinkSpeed *= multiplier;
            }

            //Debug.Log($"Velocità di blinking base: {baseBlinkSpeed}");
            //Debug.Log($"Velocità di blinking finale: {dynamicBlinkSpeed}");

            // Calcoliamo l'emissione dinamica
            float emission = Mathf.PingPong(Time.time * dynamicBlinkSpeed, maxIntensity - minIntensity) + minIntensity;
            //Debug.Log($"Emissione calcolata: {emission}");

            // Calcoliamo il colore finale emissivo
            Color finalColor = emissionColor * Mathf.LinearToGammaSpace(emission);
            //Debug.Log($"Colore finale emissivo: {finalColor}");

            // Impostiamo il colore dell'emissione
            material.SetColor("_EmissionColor", finalColor);
        }
    }
}
