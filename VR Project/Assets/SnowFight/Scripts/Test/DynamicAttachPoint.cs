using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DynamicAttachPoint : MonoBehaviour
{
    public Transform grabAttachPoint; // Punto di attach per il grab
    public Transform socketAttachPoint; // Punto di attach per il socket

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor && socketAttachPoint != null)
        {
            grabInteractable.attachTransform = socketAttachPoint;
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (grabAttachPoint != null)
        {
            grabInteractable.attachTransform = grabAttachPoint;
        }
    }
}
