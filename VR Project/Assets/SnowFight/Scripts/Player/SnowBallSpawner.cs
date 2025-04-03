using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SnowBallSpawner : MonoBehaviour
{
    public GameObject snowBallPrefab;  // Prefab della palla di neve
    public Vector3 handOffset = new Vector3(0, 0.1f, 0);  // Offset della mano

    private XRSimpleInteractable simpleInteractable;  // Riferimento all'interattore semplice
    private bool hasSpawned = false;  // Flag per evitare la creazione di più palle di neve

    void Start()
    {
        // Ottieni o aggiungi il componente XRSimpleInteractable
        simpleInteractable = gameObject.GetComponent<XRSimpleInteractable>();
        if (simpleInteractable == null)
        {
            simpleInteractable = gameObject.AddComponent<XRSimpleInteractable>();
        }

        // Aggiungi il listener per l'evento di selezione
        simpleInteractable.selectEntered.AddListener(OnSelectEntered);
    }

    // Questo metodo viene chiamato quando l'oggetto viene selezionato (interagito)
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log("Tentativo di afferrare lo spawner");

        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;
        if (interactor != null && !hasSpawned)
        {
            // Crea la palla di neve
            CreateSnowBall(interactor);

            // Forza il rilascio immediato dell'interazione con lo spawner
            ForceReleaseSpawner(interactor);

            // Imposta il flag per evitare la creazione di più palle di neve
            hasSpawned = true;
        }
        else
        {
            Debug.LogWarning("L'interactor non è di tipo XRBaseInteractor o la palla di neve è già stata creata.");
        }
    }

    // Crea la palla di neve e la assegna alla mano del giocatore
    private void CreateSnowBall(XRBaseInteractor interactor)
    {
        if (snowBallPrefab == null)
        {
            Debug.LogWarning("Prefab della palla di neve non settato.");
            return;
        }

        // Posiziona la palla di neve nella mano del giocatore con l'offset desiderato
        Vector3 handPosition = interactor.transform.position + handOffset;
        GameObject snowBall = Instantiate(snowBallPrefab, handPosition, Quaternion.identity);

        // Aggiungi XRGrabInteractable alla palla di neve
        XRGrabInteractable snowBallInteractable = snowBall.GetComponent<XRGrabInteractable>();
        if (snowBallInteractable == null)
        {
            snowBallInteractable = snowBall.AddComponent<XRGrabInteractable>();
        }

        // Seleziona immediatamente la palla di neve per l'interactor (mano)
        if (interactor.interactionManager != null)
        {
            interactor.interactionManager.SelectEnter(
                (IXRSelectInteractor)interactor,
                (IXRSelectInteractable)snowBallInteractable
            );
        }
    }

    // Rilascia immediatamente l'interazione con lo spawner
    private void ForceReleaseSpawner(XRBaseInteractor interactor)
    {
        // Rilascia l'interazione con lo spawner solo se l'interactor lo sta effettivamente selezionando
        if (simpleInteractable.isSelected && interactor.interactionManager != null)
        {
            interactor.interactionManager.SelectExit(
                (IXRSelectInteractor)interactor,
                (IXRSelectInteractable)simpleInteractable
            );

            // Resetta il flag di spawner
            hasSpawned = false;
        }
    }
}
