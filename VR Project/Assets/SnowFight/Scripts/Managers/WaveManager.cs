using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    [Header("Waves Settings")]
    [SerializeField] private bool _ignoreTutorial = false;
    [SerializeField] private bool _waitBeforeFirstWave = false;
    [SerializeField] private float _delayBetweenWaves = 15f;
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private List<SpawnerData> _wavesData;

    [Header("Spawn Offset Settings")]
    [SerializeField] private float _spawnOffsetDistance = 20f;

    private int _currentWaveIndex = 0;
    private Transform _player;
    private Transform _enemiesParent;
    private Dictionary<int, List<Transform>> _groupedSpawnPoints = new Dictionary<int, List<Transform>>();

    void Start()
    {
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
        SpawnerIdentifier[] spawners = FindObjectsByType<SpawnerIdentifier>(FindObjectsSortMode.InstanceID).OrderBy(s => s.name).ToArray();
        _spawnPoints.Clear();
        _groupedSpawnPoints.Clear();

        for (int i = 0; i < spawners.Length; i++)
        {
            List<Transform> groupedPoints = new List<Transform>();
            foreach (Transform child in spawners[i].transform)
            {
                groupedPoints.Add(child);
            }
            _groupedSpawnPoints[i] = groupedPoints;
        }
    }

    private IEnumerator StartWave()
    {
        if (_currentWaveIndex >= _wavesData.Count)
        {
            Debug.Log("All waves completed!");
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

            // Calcola direzione opposta al player
            Vector3 directionFromPlayer = (targetPosition - _player.position).normalized;
            Vector3 spawnPosition = targetPosition + directionFromPlayer * _spawnOffsetDistance;

            GameObject enemy = Instantiate(currentWave._enemies[i], spawnPosition, Quaternion.identity, _enemiesParent);

            EnemyIdentifier enemyScript = enemy.GetComponent<EnemyIdentifier>();
            enemyScript.SetWaveManager(this);
            enemyScript.SetArrivalPoint(targetPosition); // passiamo il punto d'arrivo

            spawnedEnemies++;
        }

        Debug.Log("Wave " + _currentWaveIndex + " spawned " + spawnedEnemies + " enemies.");
    }

    public void EnemyDied()
    {
        StartCoroutine(CheckRemainingEnemies());
    }

    private IEnumerator CheckRemainingEnemies()
    {
        yield return new WaitForEndOfFrame();
        int remainingEnemies = FindObjectsByType<EnemyIdentifier>(FindObjectsSortMode.None).Length;
        Debug.Log("Enemies remaining: " + remainingEnemies);

        if (remainingEnemies == 0)
        {
            StartCoroutine(NextWave());
        }
    }

    private IEnumerator NextWave()
    {
        yield return new WaitForSeconds(_delayBetweenWaves);
        _currentWaveIndex++;
        StartCoroutine(StartWave());
    }
}
