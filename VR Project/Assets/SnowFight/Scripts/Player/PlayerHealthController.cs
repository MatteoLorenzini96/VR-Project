using UnityEngine;
using System.Collections;

public class PlayerHealthController : MonoBehaviour
{
    [Header("Renderer dell'oggetto con il materiale Vignette")]
    [SerializeField] private Renderer vignetteRenderer;

    [Header("Durata FlashScreen")]
    [SerializeField] private float _flashScreenTime = 0.3f;

    private Material vignetteMaterial;
    private int _maxLives = 0;
    private float _maxVignetteIntensity = 2f;
    private HealthManager _healthManager;
    [SerializeField] private HpText _hpText;
    private Color _originalNoiseColor;
    private UIController _uiController;

    private void Start()
    {
        if (vignetteRenderer == null)
        {
            Debug.LogError("Renderer non assegnato per il materiale Vignette.");
            return;
        }

        // Crea un'istanza del materiale solo per questo Renderer (evita modifiche globali)
        vignetteMaterial = vignetteRenderer.material;

        if (vignetteMaterial == null)
        {
            Debug.LogError("Il renderer non ha un materiale assegnato.");
            return;
        }

        _uiController = GetComponent<UIController>();

        _originalNoiseColor = vignetteMaterial.GetColor("_NoiseColor");
        vignetteMaterial.SetFloat("_VignetteIntensity", 0.75f);

        SetHealthManager();
        UpdateVignette();
    }

    private void SetHealthManager()
    {
        _healthManager = GetComponent<HealthManager>();
        if (_healthManager == null)
        {
            Debug.LogError("HealthManager non assegnato a " + gameObject.name);
            return;
        }

        _maxLives = _healthManager.lives;
    }

    public void GettingDamaged()
    {
        UpdateVignette();
        UpdateHpText();
        FlashScreen(Color.red);

        if(_healthManager.lives == 0f)
        {
            _uiController.ShowCanvas();
        }
    }

    public void UpdateHpText()
    {
        _hpText.UpdateHpText();
    }

    public void UpdateVignette()
    {
        if (_healthManager == null || vignetteMaterial == null)
            return;

        float normalizedLives = Mathf.Clamp01((float)_healthManager.lives / _maxLives);
        float intensity = Mathf.Lerp(_maxVignetteIntensity, 0.75f, normalizedLives);

        vignetteMaterial.SetFloat("_VignetteIntensity", intensity);
    }

    public void FlashScreen(Color flashColor)
    {
        StartCoroutine(FlashCoroutine(flashColor));
    }

    private IEnumerator FlashCoroutine(Color flashColor)
    {
        if (vignetteMaterial == null)
            yield break;

        vignetteMaterial.SetColor("_NoiseColor", flashColor);
        yield return new WaitForSeconds(_flashScreenTime);
        vignetteMaterial.SetColor("_NoiseColor", _originalNoiseColor);
    }
}
