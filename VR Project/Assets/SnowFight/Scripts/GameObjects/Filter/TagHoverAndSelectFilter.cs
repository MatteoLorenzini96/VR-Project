using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class TagHoverAndSelectFilter : MonoBehaviour, IXRSelectFilter, IXRHoverFilter
{
    [Tooltip("Tag accettato per l'interazione.")]
    public string acceptedTag = "SnowBlock";

    [Tooltip("Tag per cui si deve negare l'hover (visual preview).")]
    public string negatedTagHover = "SnowWall";

    public bool canProcess => isActiveAndEnabled;

    // Filtro per il socket (snap)
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        string tag = interactable.transform.tag;
        return tag == acceptedTag || tag == negatedTagHover;
    }

    // Filtro per l'hover (visual preview snap)
    public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
    {
        string tag = interactable.transform.tag;
        if (tag == acceptedTag)
            return true;

        if (tag == negatedTagHover)
            return false;

        return false;
    }
}
