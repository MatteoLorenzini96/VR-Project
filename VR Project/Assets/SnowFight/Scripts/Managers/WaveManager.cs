using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WaveManager : MonoBehaviour
{
    [Header("Waves Settings")]
    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private List<SpawnerData> _wavesData;
    [SerializeField] private float _delayBetweenWaves = 5f;

    private int _currentWaveIndex = 0;
    private int _enemiesAlive = 0;
    private Transform _player;
    private Transform _turretParent;
    private Dictionary<int, List<Transform>> _groupedSpawnPoints = new Dictionary<int, List<Transform>>();

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_player == null)
        {
            Debug.LogError("Player not found! Make sure there is an object with the tag 'Player'.");
        }

        _turretParent = new GameObject("Turrets").transform;
        FindSpawnerPositions();
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
        TurretAI[] turrets = FindObjectsOfType<TurretAI>();
        foreach (TurretAI turret in turrets)
        {
            turret.Die();
        }
        Debug.Log("All enemies have been killed.");
    }

    private void FindSpawnerPositions()
    {
        SpawnerIdentifier[] spawners = FindObjectsOfType<SpawnerIdentifier>().OrderBy(s => s.name).ToArray();
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

        _enemiesAlive = currentWave._enemies.Length;
        for (int i = 0; i < currentWave._enemies.Length; i++)
        {
            GameObject enemy = Instantiate(currentWave._enemies[i], spawnPointsForWave[i].position, Quaternion.identity, _turretParent);
            if (_player != null)
            {
                enemy.transform.LookAt(_player);
            }
            enemy.GetComponent<TurretAI>().SetWaveManager(this);
        }
    }

    public void EnemyDied()
    {
        _enemiesAlive--;
        if (_enemiesAlive <= 0)
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
