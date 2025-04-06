using UnityEngine;

public class ToggleSocket : MonoBehaviour
{
    [SerializeField] private GameObject _targetObject;

    public void ActivateSocket()
    {
        if (_targetObject != null)
        {
            _targetObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Target Object non assegnato in " + gameObject.name);
        }
    }

    public void DeactivateSocket()
    {
        if (_targetObject != null)
        {
            _targetObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Target Object non assegnato in " + gameObject.name);
        }
    }
}
