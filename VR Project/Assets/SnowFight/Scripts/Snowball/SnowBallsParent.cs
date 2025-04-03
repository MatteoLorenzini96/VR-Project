using UnityEngine;

public class SnowBallsParent : MonoBehaviour
{
    public GameObject prefabToSpawn;

    public void SpawnPrefab(Vector3 spawnPosition)
    {
        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Prefab non assegnato nel componente SnowBallsParent.");
        }
    }
}
