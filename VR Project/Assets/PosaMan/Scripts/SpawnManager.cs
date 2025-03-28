using UnityEngine;
using System.Collections;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private float _spawnPointSwitchDelay = 2f;
    [SerializeField] private GameObject[] _availablePrefabs;

    private Transform[] _spawnPoints;
    private Transform _lastSpawnPoint;
    private Transform _playerTransform;

    private void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_playerTransform == null)
        {
            Debug.LogError("Player non trovato! Assicurati che il Player abbia il tag 'Player'.");
            return;
        }

        InitializeSpawnPoints();
        StartCoroutine(SpawnPrefabAtIntervals());
    }

    private void InitializeSpawnPoints()
    {
        int numberOfChildren = transform.childCount;
        _spawnPoints = new Transform[numberOfChildren];

        for (int i = 0; i < numberOfChildren; i++)
        {
            _spawnPoints[i] = transform.GetChild(i);
        }
    }

    private IEnumerator SpawnPrefabAtIntervals()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval);

            Transform selectedSpawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

            if (selectedSpawnPoint != _lastSpawnPoint)
            {
                yield return new WaitForSeconds(_spawnPointSwitchDelay);
                HandleSpawnPointChange();
            }

            GameObject prefabToSpawn = _availablePrefabs[Random.Range(0, _availablePrefabs.Length)];
            GameObject spawnedObject = Instantiate(prefabToSpawn, selectedSpawnPoint.position, Quaternion.identity);

            // Ruota il prefab per guardare il Player
            spawnedObject.transform.LookAt(_playerTransform);

            // Aggiunge una rotazione di 90° sull'asse Y
            spawnedObject.transform.Rotate(0, 90, 0);

            MovingPrefab movingPrefab = spawnedObject.GetComponent<MovingPrefab>();
            movingPrefab.Initialize(_playerTransform);
            movingPrefab.StartMoving();

            _lastSpawnPoint = selectedSpawnPoint;
        }
    }

    private void HandleSpawnPointChange()
    {
        Debug.Log("Il punto di spawn è stato cambiato.");
    }
}
