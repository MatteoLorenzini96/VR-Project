using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SnowBallSpawner : MonoBehaviour
{
    // Riferimento al prefab della palla di neve
    public GameObject snowBallPrefab;

    // Distanza di offset rispetto alla mano
    public Vector3 handOffset = new Vector3(0, 0.1f, 0);

    // Metodo chiamato quando l'oggetto viene selezionato
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("Funziono");

        // Cast dell'argomento interactorObject a XRBaseInteractor
        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;

        // Verifica che il cast abbia avuto successo
        if (interactor != null)
        {
            // Chiamare il metodo per creare la palla di neve
            CreateSnowBall(interactor);
        }
        else
        {
            Debug.LogWarning("L'interactor non è di tipo XRBaseInteractor.");
        }
    }

    // Metodo per creare la palla di neve
    private void CreateSnowBall(XRBaseInteractor interactor)
    {
        if (snowBallPrefab == null)
        {
            Debug.LogWarning("Prefab della palla di neve non settato.");
            return;
        }

        // Calcola la posizione della mano con offset
        Vector3 handPosition = interactor.transform.position + handOffset;

        // Istanzia la palla di neve alla posizione della mano
        Instantiate(snowBallPrefab, handPosition, Quaternion.identity);
    }
}
