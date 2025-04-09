using UnityEngine;

public class SnowballTriggerRelay : MonoBehaviour
{
    private SnowballMerge _mergeScript;

    [Header("VFX and SFX")]
    [SerializeField] private string _snowBallImpactVFXName = "SnowBallImpactEffect";
    [SerializeField] private string _snowBallImpactSFXName = "SnowBallImpactSound";

    private void Awake()
    {
        _mergeScript = GetComponent<SnowballMerge>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _mergeScript?.HandleTriggerWith(other.gameObject);
        // Debug.Log("Trigger rilevato con " + other.gameObject.name);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("SnowBall ha colpito " + collision.gameObject.name);

        VFXManager.Instance?.SpawnEffect(_snowBallImpactVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance?.PlaySFX(_snowBallImpactSFXName);

        if (!collision.gameObject.CompareTag("SnowBall"))
        {
            Destroy(gameObject);
        }
    }
}
