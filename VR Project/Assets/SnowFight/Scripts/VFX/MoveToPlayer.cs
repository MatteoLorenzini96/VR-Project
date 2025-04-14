using System.Collections;
using UnityEngine;

public class MoveToPlayer : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string _targetTag = "PlayerPoint";

    [Header("Movement Settings")]
    [SerializeField] private float _duration = 2f;
    [SerializeField] private Vector2 _controlPointOffsetX = new Vector2(-2f, 2f);
    [SerializeField] private Vector2 _controlPointOffsetY = new Vector2(1f, 3f);

    [Header("Initial Fall")]
    [SerializeField] private float _fallDistance = 0.5f;
    [SerializeField] private float _fallDuration = 1f;

    private Vector3 _startPoint;
    private Vector3 _controlPoint;
    private Vector3 _endPoint;
    private float _elapsedTime;
    private bool _canMove = false;
    private GameObject _player;
    private HealthManager _healthManager;

    private void Start()
    {
        GameObject target = GameObject.FindGameObjectWithTag(_targetTag);

        if (target == null)
        {
            Debug.LogWarning($"Nessun oggetto trovato con il tag '{_targetTag}'");
            return;
        }

        _player = GameObject.FindGameObjectWithTag("Player");
        if (_player == null)
        {
            Debug.LogWarning("Player non trovato");
            return;
        }
        else
        {
            _healthManager = _player.GetComponent<HealthManager>();
        }

        _startPoint = transform.position;
        _endPoint = target.transform.position;

        float randomX = Random.Range(_controlPointOffsetX.x, _controlPointOffsetX.y);
        float randomY = Random.Range(_controlPointOffsetY.x, _controlPointOffsetY.y);
        _controlPoint = _startPoint + new Vector3(randomX, randomY, 0f);

        StartCoroutine(FallThenStartMovement());
    }

    private IEnumerator FallThenStartMovement()
    {
        Vector3 fallTarget = transform.position - new Vector3(0f, _fallDistance, 0f);
        Vector3 initialPosition = transform.position;
        float elapsedFallTime = 0f;

        while (elapsedFallTime < _fallDuration)
        {
            float t = elapsedFallTime / _fallDuration;
            transform.position = Vector3.Lerp(initialPosition, fallTarget, t);
            elapsedFallTime += Time.deltaTime;
            yield return null;
        }

        transform.position = fallTarget;
        _startPoint = fallTarget;
        _canMove = true;
    }

    private void FixedUpdate()
    {
        if (!_canMove) return;

        _elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsedTime / _duration);
        transform.position = CalculateBezierPoint(t, _startPoint, _controlPoint, _endPoint);

        if (t >= 1f)
        {
            PlayerReached();
        }
    }

    private void PlayerReached()
    {
        _healthManager.HealthRecovery(1);
        //Debug.Log("Target raggiunto, aggiungo 1 vita e mi distruggo oggetto.");
        Destroy(gameObject);
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1f - t;
        return (u * u * p0) + (2 * u * t * p1) + (t * t * p2);
    }
}
