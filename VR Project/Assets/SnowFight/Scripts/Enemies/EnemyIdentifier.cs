using UnityEngine;
using System.Collections;

public class EnemyIdentifier : MonoBehaviour
{
    private WaveManager _waveManager;

    [Header("Parabolic Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _arcHeight = 3f;

    private TurretAI _turretAI;
    private BombAI _bombAI;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private float _journeyLength;
    private float _startTime;
    private bool _isMoving = false;
    private bool _hasLanded = false;

    public void SetWaveManager(WaveManager manager)
    {
        _waveManager = manager;
    }

    public void SetArrivalPoint(Vector3 point)
    {
        _targetPosition = point;
    }

    void Start()
    {
        _turretAI = GetComponent<TurretAI>();
        _bombAI = GetComponent<BombAI>();

        // Inizia movimento verso il target
        _startPosition = transform.position;
        _journeyLength = Vector3.Distance(_startPosition, _targetPosition);
        _startTime = Time.time;
        _isMoving = true;
    }

    void FixedUpdate()
    {
        if (!_isMoving || _hasLanded) return;

        float distCovered = (Time.time - _startTime) * _moveSpeed;
        float fraction = Mathf.Clamp01(distCovered / _journeyLength);

        Vector3 currentPos = Vector3.Lerp(_startPosition, _targetPosition, fraction);
        float height = Mathf.Sin(Mathf.PI * fraction) * _arcHeight;
        currentPos.y += height;

        transform.position = currentPos;

        if (fraction >= 1f)
        {
            _isMoving = false;
            _hasLanded = true;
            transform.position = _targetPosition; // Snap finale preciso
            OnLanding(); // Chiama la tua funzione custom
        }
    }

    private void OnLanding()
    {
        // Qui puoi mettere quello che vuoi succeda quando atterra.
        // Tipo attivare un AI controller, animazione idle, ecc.
        //Debug.Log($"{gameObject.name} è atterrato nel punto di arrivo.");
        if (_bombAI != null)
        {
            return;
        }
        if (_turretAI != null)
        {
            _turretAI.ActivateTurret();
        }

    }

    public void DestroyEnemy()
    {
        StartCoroutine(DestroyAfterDelay(5f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log("Notifico la morte al WaveManager");
        _waveManager?.EnemyDied();
        Destroy(gameObject);
    }
}
