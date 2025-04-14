using UnityEngine;
using TMPro;

public class HpText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private HealthManager _healthManager;
    [SerializeField] private GameObject _panel;
    [SerializeField] private float stillThreshold = 0.001f; // Sensibilità movimento
    [SerializeField] private float stillTimeRequired = 0.5f; // Tempo necessario da fermo

    private Vector3 _lastPosition;
    private float _stillTimer = 0f;
    private bool _isPanelVisible = false;

    private void Start()
    {
        _lastPosition = transform.position;
        UpdateHpText();
        SetPanelVisible(false);
    }

    private void Update()
    {
        float distanceMoved = Vector3.Distance(transform.position, _lastPosition);

        if (distanceMoved < stillThreshold)
        {
            _stillTimer += Time.deltaTime;

            if (_stillTimer >= stillTimeRequired && !_isPanelVisible)
            {
                SetPanelVisible(true);
            }
        }
        else
        {
            _stillTimer = 0f;

            if (_isPanelVisible)
            {
                SetPanelVisible(false);
            }
        }

        _lastPosition = transform.position;
    }

    public void UpdateHpText()
    {
        if (_hpText != null && _healthManager != null)
        {
            _hpText.text = $"{_healthManager.lives} / {_healthManager.maxLives}";
        }
    }

    private void SetPanelVisible(bool visible)
    {
        if (_panel != null)
        {
            _panel.SetActive(visible);
            _isPanelVisible = visible;
        }
    }
}
