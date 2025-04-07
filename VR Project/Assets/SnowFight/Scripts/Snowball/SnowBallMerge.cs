using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SnowballMerge : MonoBehaviour
{
    [Header("Merge Settings")]
    [SerializeField] private GameObject _snowBlockPrefab;
    //[SerializeField] private float _mergeCooldown = .1f;

    private XRGrabInteractable _grabInteractable;
    private bool _isGrabbed;
    private float _lastMergeTime;

    private void Awake()
    {
        _grabInteractable = GetComponent<XRGrabInteractable>();
        _grabInteractable.selectEntered.AddListener(OnGrab);
        _grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDestroy()
    {
        _grabInteractable.selectEntered.RemoveListener(OnGrab);
        _grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        //Debug.Log("Palla grabbata");
        _isGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        //Debug.Log("Palla lasciata");
        _isGrabbed = false;
    }

    public void HandleTriggerWith(GameObject other)
    {
        //if (Time.time - _lastMergeTime < _mergeCooldown) return;
        //Debug.Log("" + _isGrabbed);
        if (!_isGrabbed) return;

        if (other.CompareTag("SnowBall"))
        {
            //Debug.Log("L'altro oggetto ha il tag Snowball");

            var otherSnowball = other.GetComponentInParent<SnowballMerge>();
            if (otherSnowball == null)
            {
                Debug.LogWarning("SnowballMerge non trovato sull'altro oggetto!");
                return;
            }

            //Debug.Log("otherSnowball trovato");
            //Debug.Log("otherSnowball._isGrabbed: " + otherSnowball._isGrabbed);

            if (otherSnowball._isGrabbed)
            {
                //Debug.Log("Entrambe le palle soddisfano le condizioni");
                MergeWith(otherSnowball);
            }
        }
    }

    private void MergeWith(SnowballMerge otherSnowball)
    {
        //Debug.Log("Comincio il merge");

        _lastMergeTime = Time.time;
        otherSnowball._lastMergeTime = Time.time;

        Vector3 spawnPosition = (transform.position + otherSnowball.transform.position) / 2f;
        Quaternion spawnRotation = Quaternion.identity;

        Instantiate(_snowBlockPrefab, spawnPosition, spawnRotation);

        Destroy(otherSnowball.gameObject);
        Destroy(gameObject);
    }
}
