using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TagHoverAndSelectFilter : MonoBehaviour, IXRSelectFilter, IXRHoverFilter
{
    [Tooltip("Tag accettato per interazione.")]
    public string acceptedTag = "SnowBlock";

    public bool canProcess => isActiveAndEnabled;

    // Filtro per il socket (snap)
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        return interactable.transform.CompareTag(acceptedTag);
    }

    // Filtro per l'hover (visual preview snap)
    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        return interactable.transform.CompareTag(acceptedTag);
    }
}
