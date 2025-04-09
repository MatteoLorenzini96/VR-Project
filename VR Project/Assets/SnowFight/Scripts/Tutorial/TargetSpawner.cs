using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Target da spawnare")]
    public GameObject oggettoDaSpawnare;

    [Header("Trasform di spawn")]
    public Transform puntoDiSpawn;

    public void SpawnOggetto()
    {
        if (oggettoDaSpawnare != null && puntoDiSpawn != null)
        {
            Instantiate(oggettoDaSpawnare, puntoDiSpawn.position, puntoDiSpawn.rotation);
        }
        else
        {
            Debug.LogWarning("Oggetto da spawnare o punto di spawn non assegnato!");
        }
    }
}
