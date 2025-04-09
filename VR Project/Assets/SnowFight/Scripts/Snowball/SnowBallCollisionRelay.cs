using UnityEngine;

public class SnowballTriggerRelay : MonoBehaviour
{
    private SnowballMerge _parentMergeScript;

    [Header("VFX and SFX")]
    [SerializeField] private string _snowBallImpactVFXName = "SnowBallImpactEffect";
    [SerializeField] private string _snowBallImpactSFXName = "SnowBallImpactSound";

    private void Awake()
    {
        _parentMergeScript = GetComponentInParent<SnowballMerge>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _parentMergeScript?.HandleTriggerWith(other.gameObject);
        //Debug.Log("Trigger rilevato con " + other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        VFXManager.Instance.SpawnEffect(_snowBallImpactVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_snowBallImpactSFXName);

        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
