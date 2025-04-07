using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class XRBlockDetach : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private XRSocketInteractor[] socketInteractors;

    void Start()
    {
        // Ottieni il riferimento a XRGrabInteractable
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Trova gli XR Socket Interactors collegati
        socketInteractors = GetComponentsInChildren<XRSocketInteractor>();
    }

    // Funzione chiamata quando il blocco viene preso
    public void OnGrab(SelectEnterEventArgs args)
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
            }
        }
    }
}
