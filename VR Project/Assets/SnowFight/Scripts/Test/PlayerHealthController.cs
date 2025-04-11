using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    [Header("Oggetto con la proprietà _VignetteIntensity")]
    public Material vignetteMaterial;

    private int _maxLives = 0;
    private float _maxVignetteIntensity = 2f;
    private HealthManager _healthManager;

    private void Start()
    {
        if (vignetteMaterial == null)
        {
            Debug.LogError("Materiale Vignette non assegnato a " + gameObject.name);
        }

        vignetteMaterial.SetFloat("_VignetteIntensity", 0f);
        
        SetHealthManager();
        UpdateVignette();
    }

    private void SetHealthManager()
    {
        _healthManager = GetComponent<HealthManager>();
        _maxLives = _healthManager.lives;
        if (_healthManager == null)
        {
            Debug.LogError("HealthManager non assegnato a " + gameObject.name);
        }
    }

    public void GettingDamaged()
    {
        UpdateVignette();
    }

    public void UpdateVignette()
    {
        if (_healthManager == null || vignetteMaterial == null)
            return;

        // Calcolo proporzionale tra vite e intensità
        float normalizedLives = Mathf.Clamp01((float)_healthManager.lives / _maxLives);
        float intensity = Mathf.Lerp(_maxVignetteIntensity, 0, normalizedLives); // Meno vite = più intensità

        vignetteMaterial.SetFloat("_VignetteIntensity", intensity);
    }
}
