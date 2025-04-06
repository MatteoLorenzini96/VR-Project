using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BlockSupport : MonoBehaviour
{
    public List<XRSocketInteractor> connectedSockets = new List<XRSocketInteractor>(); // Lista dei socket sopra
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        grabInteractable.selectEntered.AddListener(OnGrabbed); // Quando il blocco viene preso
        grabInteractable.selectExited.AddListener(OnReleased); // Quando il blocco viene rilasciato
    }

    private void Start()
    {
        // Se il blocco è già snappato in un socket, registriamo i socket sopra
        FindConnectedSockets();
    }

    private void FindConnectedSockets()
    {
        // Trova tutti i socket sopra il blocco
        XRSocketInteractor[] sockets = GetComponentsInChildren<XRSocketInteractor>(true); // Recupera tutti i socket nei figli

        foreach (var socket in sockets)
        {
            if (socket.hasSelection)
            {
                connectedSockets.Add(socket);
            }
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        // Disattiva i blocchi sopra e fa cadere tutto
        foreach (var socket in connectedSockets)
        {
            DetachAndDrop(socket);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // Prova a riattaccare ai socket disponibili
        TryReconnect();
    }

    private void DetachAndDrop(XRSocketInteractor socket)
    {
        // Rimuovi l'interactable dal socket
        var interactable = socket.interactablesSelected.Count > 0 ? socket.interactablesSelected[0] : null;

        if (interactable != null)
        {
            // Usa l'interaction manager globale per la disconnessione
            var interactionManager = socket.GetComponentInParent<XRInteractionManager>();
            if (interactionManager != null)
            {
                // Cast dell'interactable per usarlo come IXRSelectInteractable
                var ixrInteractable = interactable as IXRSelectInteractable;
                if (ixrInteractable != null)
                {
                    interactionManager.SelectExit(socket, ixrInteractable); // Rimuove la selezione
                }
            }
        }

        // Attiva la fisica per farlo cadere
        if (interactable != null)
        {
            Rigidbody socketRb = GetComponent<Rigidbody>();
            if (socketRb != null)
            {
                socketRb.isKinematic = false; // Permetti la fisica
                socketRb.useGravity = true; // Usa la gravità
            }
        }
    }

    private void TryReconnect()
    {
        // Cerca un nuovo socket disponibile
        XRSocketInteractor[] sockets = Object.FindObjectsByType<XRSocketInteractor>(FindObjectsSortMode.None); // Nuovo metodo

        foreach (var socket in sockets)
        {
            // Se il socket non ha già un oggetto collegato e l'oggetto è abbastanza vicino:
            if (!socket.hasSelection && Vector3.Distance(transform.position, socket.transform.position) < 0.5f)
            {
                // Riattacca il blocco al socket usando la logica dell'XR Interaction Manager
                var interactionManager = socket.GetComponentInParent<XRInteractionManager>();
                if (interactionManager != null)
                {
                    // Cast dell'interactable per usarlo come IXRSelectInteractable
                    var ixrInteractable = grabInteractable as IXRSelectInteractable;
                    if (ixrInteractable != null)
                    {
                        interactionManager.SelectEnter(socket, ixrInteractable); // Usa SelectEnter correttamente
                    }
                }
                break; // Interrompe quando trova il primo socket disponibile
            }
        }
    }
}
