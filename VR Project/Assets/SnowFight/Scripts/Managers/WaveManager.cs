using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class NamedSpawnGroup
{
    public string parentName;
    public List<Transform> spawnPoints = new List<Transform>();
}

public class WaveManager : MonoBehaviour
{
    [Header("Waves Settings")]
    [SerializeField] private bool _ignoreTutorial = false;
    [SerializeField] private bool _waitBeforeFirstWave = false;
    [SerializeField] private float _delayBetweenWaves = 15f;

    [SerializeField] private List<Transform> _spawnPoints;

    [Header("Debug - Spawn Points per Nome (solo visuale)")]
    [SerializeField] private List<NamedSpawnGroup> _namedSpawnGroups = new List<NamedSpawnGroup>();

    [SerializeField] private List<SpawnerData> _wavesData;

    [Header("Spawn Offset Settings")]
    [SerializeField] private float _spawnOffsetDistance = 20f;

    private int _currentWaveIndex = 0;
    private Coroutine _waitingNextWaveCoroutine;

    private Transform _player;
    private Transform _enemiesParent;
    private TargetSpawner _targetSpawner;

    private Dictionary<int, List<Transform>> _groupedSpawnPoints = new Dictionary<int, List<Transform>>();
    private Dictionary<string, List<Transform>> _namedGroupedSpawnPoints = new Dictionary<string, List<Transform>>();

    private float _elapsedTime = 0f;

    private const string LastWaveKey = "LastWave"; // << MODIFICA

    void Awake()
    {
        // Carica la wave salvata
        _currentWaveIndex = PlayerPrefs.GetInt(LastWaveKey, 0); // << MODIFICA
        Debug.Log("Wave iniziale caricata da PlayerPrefs: " + _currentWaveIndex); // << MODIFICA
    }

    void Start()
    {
        _targetSpawner = GetComponent<TargetSpawner>();
        FindSpawnerPositions();

        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_player == null)
        {
            Debug.LogError("Player not found! Make sure there is an object with the tag 'Player'.");
        }

        _enemiesParent = new GameObject("Enemies").transform;

        if (_ignoreTutorial)
        {
            StartCoroutine(StartWave());
        }
    }

    public void ActivateWaves()
    {
        StartCoroutine(StartWave());
    }

    void Update()
    {
        _elapsedTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DebugKillAll();
        }
    }

    private void DebugKillAll()
    {
        EnemyIdentifier[] enemies = FindObjectsByType<EnemyIdentifier>(FindObjectsSortMode.None);
        foreach (EnemyIdentifier enemy in enemies)
        {
            enemy.DestroyEnemy();
        }
    }

    private void FindSpawnerPositions()
    {
        _groupedSpawnPoints.Clear();
        _namedGroupedSpawnPoints.Clear();
        _namedSpawnGroups.Clear();

        int waveIndex = 0;

        foreach (Transform holder in _spawnPoints)
        {
            if (holder.GetComponent<SpawnerIdentifier>() == null)
            {
                Debug.LogWarning($"{holder.name} non ha uno SpawnerIdentifier, verrà ignorato.");
                continue;
            }

            List<Transform> children = new List<Transform>();

            foreach (Transform child in holder)
            {
                children.Add(child);
            }

            _groupedSpawnPoints[waveIndex] = children;
            _namedGroupedSpawnPoints[holder.name] = children;

            NamedSpawnGroup group = new NamedSpawnGroup
            {
                parentName = holder.name,
                spawnPoints = children
            };
            _namedSpawnGroups.Add(group);

            waveIndex++;
        }
    }

    private IEnumerator StartWave()
    {
        TutorialTurret _tutorialTurret = FindAnyObjectByType<TutorialTurret>();
        if (_tutorialTurret != null)
        {
            _tutorialTurret.DeleteTurret();
        }

        if (_currentWaveIndex >= _wavesData.Count)
        {
            Debug.Log("All waves completed! " + _elapsedTime + " seconds.");
            yield break;
        }

        if (_currentWaveIndex == 0 && _waitBeforeFirstWave)
        {
            yield return new WaitForSeconds(_delayBetweenWaves);
        }

        if (!_groupedSpawnPoints.ContainsKey(_currentWaveIndex))
        {
            Debug.LogError("No spawn points found for wave index " + _currentWaveIndex);
            yield break;
        }

        SpawnerData currentWave = _wavesData[_currentWaveIndex];
        List<Transform> spawnPointsForWave = _groupedSpawnPoints[_currentWaveIndex];

        if (currentWave._enemies.Length > spawnPointsForWave.Count)
        {
            Debug.LogError("Not enough spawn points for all enemies in wave " + _currentWaveIndex);
            yield break;
        }

        int spawnedEnemies = 0;
        for (int i = 0; i < currentWave._enemies.Length; i++)
        {
            Vector3 targetPosition = spawnPointsForWave[i].position;

            Vector3 directionFromPlayer = (targetPosition - _player.position).normalized;
            Vector3 spawnPosition = targetPosition + directionFromPlayer * _spawnOffsetDistance;

            GameObject enemy = Instantiate(currentWave._enemies[i], spawnPosition, Quaternion.identity, _enemiesParent);

            EnemyIdentifier enemyScript = enemy.GetComponent<EnemyIdentifier>();
            enemyScript.SetWaveManager(this);
            enemyScript.SetArrivalPoint(targetPosition);

            spawnedEnemies++;
        }

        Debug.Log("Wave " + _currentWaveIndex + " spawned " + spawnedEnemies + " enemies at " + _elapsedTime + " seconds.");
    }

    public void EnemyDied()
    {
        StartCoroutine(CheckRemainingEnemies());
    }

    private IEnumerator CheckRemainingEnemies()
    {
        yield return new WaitForSeconds(0.7f);
        int remainingEnemies = FindObjectsByType<EnemyIdentifier>(FindObjectsSortMode.None).Length;
        Debug.Log("Enemies remaining: " + remainingEnemies);

        if (remainingEnemies == 0)
        {
            StartCoroutine(NextWave());
        }
    }

    private IEnumerator NextWave()
    {
        SpawnSkipTurret();
        _waitingNextWaveCoroutine = StartCoroutine(DelayedNextWave());
        yield return _waitingNextWaveCoroutine;
    }

    private IEnumerator DelayedNextWave()
    {
        yield return new WaitForSeconds(_delayBetweenWaves);
        _currentWaveIndex++;

        PlayerPrefs.SetInt(LastWaveKey, _currentWaveIndex); // << MODIFICA
        PlayerPrefs.Save(); // << MODIFICA

        _waitingNextWaveCoroutine = null;
        StartCoroutine(StartWave());
    }

    public void SkipDelayBetweenWaves()
    {
        if (_waitingNextWaveCoroutine != null)
        {
            StopCoroutine(_waitingNextWaveCoroutine);
            _waitingNextWaveCoroutine = null;
            _currentWaveIndex++;

            PlayerPrefs.SetInt(LastWaveKey, _currentWaveIndex); // << MODIFICA
            PlayerPrefs.Save(); // << MODIFICA

            StartCoroutine(StartWave());
        }
    }

    private void SpawnSkipTurret()
    {
        _targetSpawner.SpawnOggetto();
    }

    // Metodo pubblico utile per UI o debug
    public int GetCurrentWaveIndex()
    {
        return _currentWaveIndex;
    }
}
