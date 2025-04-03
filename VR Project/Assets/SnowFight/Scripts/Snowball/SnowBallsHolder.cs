using UnityEngine;

public class SnowBallsHolder : MonoBehaviour
{
    public GameObject prefabToSpawn;

    public void SpawnPrefab(Vector3 spawnPosition, Transform originalParent)
    {
        if (prefabToSpawn != null)
        {
            GameObject newPrefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            newPrefab.transform.SetParent(originalParent);
        }
        else
        {
            Debug.LogWarning("Prefab non assegnato nel componente SnowBallsParent.");
        }
    }
}
