using UnityEngine;
using System.Collections;

public class TargetSpawner : MonoBehaviour
{
    [Header("Target da spawnare")]
    public GameObject oggettoDaSpawnare;

    [Header("Punto di arrivo del movimento")]
    public Transform puntoDiArrivo;

    [Header("Offset di spawn rispetto al player")]
    public float spawnOffsetDistance = 20f;

    [Header("Impostazioni movimento parabolico")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float arcHeight = 3f;

    private Transform _player;

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("PlayerPoint");
        if (playerObject != null)
        {
            _player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Nessun oggetto trovato con tag 'PlayerPoint'.");
        }
    }

    public void SpawnOggetto()
    {
        if (oggettoDaSpawnare == null || puntoDiArrivo == null || _player == null)
        {
            Debug.LogWarning("Oggetto, punto di arrivo o player non assegnato!");
            return;
        }

        // Calcola posizione di spawn offsettata
        Vector3 direction = (puntoDiArrivo.position - _player.position).normalized;
        Vector3 spawnPosition = puntoDiArrivo.position + direction * spawnOffsetDistance;

        GameObject nuovoOggetto = Instantiate(oggettoDaSpawnare, spawnPosition, Quaternion.identity);

        // Avvia la coroutine per il movimento parabolico
        StartCoroutine(MuoviParabolicamente(nuovoOggetto.transform, spawnPosition, puntoDiArrivo.position));
    }

    private IEnumerator MuoviParabolicamente(Transform oggetto, Vector3 inizio, Vector3 destinazione)
    {
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(inizio, destinazione);
        bool movimentoAttivo = true;

        while (movimentoAttivo)
        {
            float distPercorsa = (Time.time - startTime) * moveSpeed;
            float frazione = Mathf.Clamp01(distPercorsa / journeyLength);

            Vector3 posizioneCorrente = Vector3.Lerp(inizio, destinazione, frazione);
            float altezza = Mathf.Sin(Mathf.PI * frazione) * arcHeight;
            posizioneCorrente.y += altezza;

            oggetto.position = posizioneCorrente;

            if (frazione >= 1f)
            {
                oggetto.position = destinazione;
                movimentoAttivo = false;
            }

            yield return null;
        }
    }
}
