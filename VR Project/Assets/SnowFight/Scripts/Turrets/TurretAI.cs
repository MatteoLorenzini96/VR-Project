using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TurretAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Vector3 _movementOffset = Vector3.zero; // Quanto deve muoversi
    [SerializeField] private float _movementSpeed = 2f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _bulletSpeed = 10f;
    [SerializeField] private float _shootCooldown = 3f;
    [SerializeField] private float _targetUpdateInterval = 1f;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private bool _movingToTarget = true;
    private Transform _playerTransform;

    private void Start()
    {
        _startPosition = transform.position;
        _targetPosition = _startPosition + _movementOffset;

        StartCoroutine(UpdateTarget());
        StartCoroutine(ShootAtPlayer());
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 destination = _movingToTarget ? _targetPosition : _startPosition;
        transform.position = Vector3.MoveTowards(transform.position, destination, _movementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.1f)
        {
            _movingToTarget = !_movingToTarget;
        }
    }

    private IEnumerator UpdateTarget()
    {
        while (true)
        {
            if (_playerTransform == null)
            {
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject)
                {
                    _playerTransform = playerObject.transform;
                }
            }
            yield return new WaitForSeconds(_targetUpdateInterval);
        }
    }

    private IEnumerator ShootAtPlayer()
    {
        while (true)
        {
            if (_playerTransform != null && _bulletPrefab != null && _firePoint != null)
            {
                Vector3 direction = (_playerTransform.position - _firePoint.position).normalized;
                GameObject bulletInstance = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);

                Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.Initialize(direction, _bulletSpeed);
                }
            }
            yield return new WaitForSeconds(_shootCooldown);
        }
    }
}
