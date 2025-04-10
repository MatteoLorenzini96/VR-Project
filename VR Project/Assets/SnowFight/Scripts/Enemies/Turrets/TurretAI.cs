using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TurretAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private bool _canMove = true;
    [SerializeField] private Vector3 _movementOffset = Vector3.zero;
    [SerializeField] private float _movementSpeed = 2f;

    [Header("Shooting Settings")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private List<Transform> _firePoints;
    [SerializeField] private float _bulletSpeed = 10f;
    [SerializeField] private float _normalCooldown = 3f; // Cooldown fisso
    [SerializeField] private float _targetUpdateInterval = 1f;
    [Header("Shooting VFX and SFX")]
    [SerializeField] private string _turretShootVFXName = "TurretShootEffect";
    [SerializeField] private string _turretShootSFXName = "TurretShootSound";

    [Header("Is Cooldown Random?")]
    [SerializeField] private bool _randomCooldown = false; // Se true, cooldown random, altrimenti fisso
    [SerializeField] private float _shootCooldownMin = 2f;
    [SerializeField] private float _shootCooldownMax = 5f;

    [Header("Creazione VFX and SFX")]
    [SerializeField] private string _turretCreationVFXName = "TurretCreationEffect";
    [SerializeField] private string _turretCreationSFXName = "TurretCreationSound";

    [Header("Distruzione VFX and SFX")]
    [SerializeField] private string _turretDestructionVFXName = "TurretDestructionEffect";
    [SerializeField] private string _turretDestructionSFXName = "TurretDestructionSound";

    private Vector3 _startPosition;
    private Vector3 _targetPosition;
    private bool _movingToTarget = true;
    private Transform _playerTransform;
    private EnemyIdentifier _enemyIdentifier;
    private HealthManager _healthManager;

    private void Start()
    {
        SearchHealthManager();
        SearchForIdentifier();

        VFXManager.Instance.SpawnEffect(_turretCreationVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_turretCreationSFXName);

        _startPosition = transform.position;
        _targetPosition = _startPosition + _movementOffset;

        LookAtPlayer();
        StartCoroutine(UpdateTarget());
        StartCoroutine(ShootAtPlayer());
    }

    private void SearchHealthManager()
    {
        _healthManager = GetComponent<HealthManager>();
    }

    private void SearchForIdentifier()
    {
        _enemyIdentifier = GetComponent<EnemyIdentifier>();

        if (_enemyIdentifier == null)
        {
            Debug.LogError("EnemyIdentifier non trovato su " + gameObject.name + "!");
        }
    }

    private void LookAtPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject)
        {
            _playerTransform = playerObject.transform;
            transform.LookAt(_playerTransform);
        }
    }

    private void FixedUpdate()
    {
        if (_canMove)
        {
            HandleMovement();
        }

        else return;
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
            if (_playerTransform != null && _bulletPrefab != null && _firePoints.Count > 0)
            {
                foreach (var firePoint in _firePoints)
                {
                    Vector3 direction = (_playerTransform.position - firePoint.position).normalized;
                    GameObject bulletInstance = Instantiate(_bulletPrefab, firePoint.position, Quaternion.identity);

                    VFXManager.Instance.SpawnEffect(_turretShootVFXName, firePoint.position, Quaternion.identity);
                    AudioManager.Instance.PlaySFX(_turretShootSFXName);

                    Bullet bulletScript = bulletInstance.GetComponent<Bullet>();
                    if (bulletScript != null)
                    {
                        bulletScript.Initialize(direction, _bulletSpeed);
                    }
                }

                float cooldown = _randomCooldown ? Random.Range(_shootCooldownMin, _shootCooldownMax) : _normalCooldown;
                yield return new WaitForSeconds(cooldown);
            }
            else
            {
                yield return null;
            }
        }
    }

    public void HandleDamage()
    {
        switch (_healthManager.lives)
        {
            case 0:
                //Debug.Log("Vita 0: Il blocco è distrutto!");

                VFXManager.Instance.SpawnEffect(_turretDestructionVFXName, transform.position, Quaternion.identity);
                AudioManager.Instance.PlaySFX(_turretDestructionSFXName);

                HandleDestruction();

                break;
            default:
                Debug.Log("Valore di lives non valido.");
                break;
        }
    }

    public void HandleDestruction()
    {
        _enemyIdentifier.DestroyEnemy();
    }
}
