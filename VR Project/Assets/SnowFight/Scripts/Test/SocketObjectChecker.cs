using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketObjectChecker : MonoBehaviour
{
    private XRSocketInteractor socketInteractor;

    private void Awake()
    {
        socketInteractor = GetComponent<XRSocketInteractor>();
        if (socketInteractor == null)
        {
            Debug.LogError("Nessun XRSocketInteractor trovato su questo GameObject.");
        }
        AddListener();
    }

    private void AddListener()
    {
        socketInteractor.selectEntered.AddListener(OnSelectEntered);
        socketInteractor.selectExited.AddListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        //Debug.Log("Cerco l'oggetto con lo script (entrata)");
        GameObject enteredObject = args.interactableObject.transform.gameObject;

        ToggleSocket toggleScript = enteredObject.GetComponent<ToggleSocket>();
        if (toggleScript != null)
        {
            toggleScript.ActivateSocket();
            enteredObject.tag = "SnowWall";
            //Debug.Log("Attivo il Socket dallo script: " + gameObject.name);
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        //Debug.Log("Cerco l'oggetto con lo script (uscita)");
        GameObject exitedObject = args.interactableObject.transform.gameObject;

        ToggleSocket toggleScript = exitedObject.GetComponent<ToggleSocket>();
        if (toggleScript != null)
        {
            toggleScript.DeactivateSocket();
            exitedObject.tag = "SnowBlock";
            //Debug.Log("Disattivo il Socket dallo script: " + gameObject.name);
        }
    }
}
