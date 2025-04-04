using UnityEngine;

public class DeactivateSocket : MonoBehaviour
{
    [SerializeField] private GameObject _targetObject;

    public void ToggleObject()
    {
        if (_targetObject != null)
        {
            _targetObject.SetActive(!_targetObject.activeSelf);
        }
        else
        {
            Debug.LogWarning("Target Object non assegnato in " + gameObject.name);
        }
    }
}
