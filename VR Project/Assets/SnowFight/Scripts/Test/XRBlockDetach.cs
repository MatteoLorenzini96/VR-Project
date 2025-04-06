using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRBlockDetach : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private XRSocketInteractor[] socketInteractors;
    private bool wasDetached = false;

    void Start()
    {
        // Ottieni il riferimento a XRGrabInteractable
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Trova gli XR Socket Interactors collegati
        socketInteractors = GetComponentsInChildren<XRSocketInteractor>();

        // Usa i nuovi eventi con la firma aggiornata
        grabInteractable.selectEntered.AddListener(OnGrab);
        //grabInteractable.selectExited.AddListener(OnRelease);
    }

    // Funzione chiamata quando il blocco viene preso
    private void OnGrab(SelectEnterEventArgs args)
    {
        // Stacca il blocco dai socket solo quando viene preso
        foreach (var socket in socketInteractors)
        {
            // Se l'oggetto è attaccato al socket, staccalo
            if (socket.hasSelection)
            {
                // Utilizza SelectExit per disconnettere l'oggetto dal socket
                socket.interactionManager.SelectExit(
                    (IXRSelectInteractor)args.interactorObject,
                    (IXRSelectInteractable)grabInteractable
                );
                wasDetached = true;
            }
        }
    }

    // Funzione chiamata quando il blocco viene rilasciato
    /* private void OnRelease(SelectExitEventArgs args)
    {
        // Se il blocco è stato staccato, riaggancialo ai socket
        if (wasDetached)
        {
            // Trova un socket disponibile
            foreach (var socket in socketInteractors)
            {
                // Se il socket non ha ancora un oggetto attaccato
                if (!socket.hasSelection)
                {
                    // Collega l'oggetto al socket automaticamente
                    socket.StartPlacement(grabInteractable);  // Questa funzione inizia la "collocazione" dell'oggetto nel socket
                    break; // Una volta attaccato, esci dal ciclo
                }
            }

            wasDetached = false;  // Reset dello stato
        }
    }     */
}
