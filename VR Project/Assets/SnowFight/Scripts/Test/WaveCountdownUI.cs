using UnityEngine;
using TMPro;

public class WaveCountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro _countdownText;
    
    private WaveManager _waveManager;

    private float _remainingTime = 0f;
    private bool _isCountingDown = false;

    private void Start()
    {
        _waveManager = FindAnyObjectByType<WaveManager>();
    }

    public void StartCountdown(float delay)
    {
        _remainingTime = delay;
        _isCountingDown = true;
    }

    public void StopCountdown()
    {
        _isCountingDown = false;
        _countdownText.text = "";
    }

    void FixedUpdate()
    {
        if (_isCountingDown)
        {
            _remainingTime -= Time.deltaTime;

            if (_remainingTime > 0f)
            {
                _countdownText.text = $"Next Wave in: {Mathf.CeilToInt(_remainingTime)}";
            }
            else
            {
                _countdownText.text = "";
                _isCountingDown = false;
            }
        }
    }
}
