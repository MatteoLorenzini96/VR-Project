using UnityEngine;

public class ToggleSocket : MonoBehaviour
{
    [SerializeField] private GameObject _targetObject;

    public void ActivateSocket()
    {
        //Debug.Log(gameObject.name + "Attivo il Socket");

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
        //Debug.Log(gameObject.name + "Disattivo il Socket");

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
