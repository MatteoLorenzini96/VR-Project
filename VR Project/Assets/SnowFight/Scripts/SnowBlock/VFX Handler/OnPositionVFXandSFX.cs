using UnityEngine;

public class OnPositionVFXandSFX : MonoBehaviour
{
    [Header("Positioning Blocks VFX and SFX")]
    [SerializeField] private string _snowBlockPositioningVFXName = "SnowBlockPositioningEffect";
    [SerializeField] private string _snowBlockPositioningSFXName = "SnowBlockPositioningSound";

    [Header("Taking Blocks VFX and SFX")]
    [SerializeField] private string _snowBlockTakingVFXName = "SnowBlockTakingEffect";
    [SerializeField] private string _snowBlockTakingSFXName = "SnowBlockTakingSound";

    public void DoPositioningSFXandVFX()
    {
        VFXManager.Instance.SpawnEffect(_snowBlockPositioningVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_snowBlockPositioningSFXName);
    }

    public void DoTakingSFXandVFX()
    {
        VFXManager.Instance.SpawnEffect(_snowBlockTakingVFXName, transform.position, Quaternion.identity);
        AudioManager.Instance.PlaySFX(_snowBlockTakingSFXName);
    }
}
