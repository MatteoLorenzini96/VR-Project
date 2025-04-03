using UnityEngine;

public class SnowBallDetach : MonoBehaviour
{
    private Vector3 initialPosition;  // La posizione iniziale dell'oggetto
    private bool hasDetached = false; // Flag per eseguire il detach solo una volta

    private void Start()
    {
        // Salviamo la posizione iniziale al momento dell'avvio
        initialPosition = transform.position;
    }

    public void DetachAndNotifyParent()
    {
        if (hasDetached)
            return;  // Assicuriamoci che il detach venga fatto solo una volta

        hasDetached = true;

        // Troviamo l'oggetto "SnowBalls" o lo creiamo se non esiste
        GameObject snowBallsParent = GameObject.Find("SnowBalls");
        if (snowBallsParent == null)
        {
            snowBallsParent = new GameObject("SnowBalls");
        }

        // Stacchiamo l'oggetto dal suo parent attuale
        transform.SetParent(null);

        // Impostiamo il nuovo parent
        transform.SetParent(snowBallsParent.transform);

        // Chiamare la funzione SpawnPrefab nel parent SnowBalls
        SnowBallsParent parentScript = snowBallsParent.GetComponent<SnowBallsParent>();
        if (parentScript != null)
        {
            parentScript.SpawnPrefab(initialPosition);
        }
        else
        {
            Debug.LogWarning("Componente SnowBallsParent non trovato su SnowBalls.");
        }
    }
}
