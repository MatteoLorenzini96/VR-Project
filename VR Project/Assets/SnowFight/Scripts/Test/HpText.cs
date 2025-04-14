using UnityEngine;
using TMPro;

public class HpText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private HealthManager _healthManager;
    [SerializeField] private GameObject _panel;

    private void Start()
    {
        UpdateHpText();
    }

    public void UpdateHpText()
    {
        if (_hpText != null && _healthManager != null)
        {
            _hpText.text = $"{_healthManager.lives} / {_healthManager.maxLives}";
        }
    }

    public void TogglePanel()
    {
        if (_panel != null)
        {
            _panel.SetActive(!_panel.activeSelf);
        }
    }
}
