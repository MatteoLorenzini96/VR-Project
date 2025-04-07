using UnityEngine;

public class SnowballTriggerRelay : MonoBehaviour
{
    private SnowballMerge _parentMergeScript;

    private void Awake()
    {
        _parentMergeScript = GetComponentInParent<SnowballMerge>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _parentMergeScript?.HandleTriggerWith(other.gameObject);
        //Debug.Log("Trigger rilevato con " + other.gameObject);
    }
}
