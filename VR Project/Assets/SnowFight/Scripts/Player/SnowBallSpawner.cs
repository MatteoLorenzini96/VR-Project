using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SnowBallSpawner : MonoBehaviour
{
    public GameObject snowBallPrefab;
    public Vector3 handOffset = new Vector3(0, 0.1f, 0);

    private XRGrabInteractable grabInteractable;

    void Start()
    {
        // Assicurati che l'oggetto abbia un componente XRGrabInteractable
        grabInteractable = gameObject.GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<XRGrabInteractable>();
        }

        // Aggiungi un listener per quando l'oggetto viene selezionato
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
    }

    // Gestisce l'evento quando il giocatore tenta di afferrare l'oggetto
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("Tentativo di afferrare la palla di neve");

        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;

        // Verifica che l'interactor sia valido
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

    private void CreateSnowBall(XRBaseInteractor interactor)
    {
        if (snowBallPrefab == null)
        {
            Debug.LogWarning("Prefab della palla di neve non settato.");
            return;
        }

        Vector3 handPosition = interactor.transform.position + handOffset;

        // Instanzia la palla di neve
        GameObject snowBall = Instantiate(snowBallPrefab, handPosition, Quaternion.identity);

        // Aggiungi il componente XRGrabInteractable per permettere l'interazione
        XRGrabInteractable grabInteractable = snowBall.AddComponent<XRGrabInteractable>();

        // Aggiungi l'XRBaseInteractor come interactor che "prende" l'oggetto
        grabInteractable.interactorsSelecting.Add(interactor);

        // Puoi anche opzionalmente aggiungere un effetto visivo o fisico qui
    }
}
