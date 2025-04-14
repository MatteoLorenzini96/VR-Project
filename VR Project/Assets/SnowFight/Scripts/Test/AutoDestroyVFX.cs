using UnityEngine;
using UnityEngine.VFX;

public class AutoDestroyVFX : MonoBehaviour
{
    [SerializeField] private float _duration = 2f;

    private VisualEffect _visualEffect;

    void Start()
    {
        _visualEffect = GetComponent<VisualEffect>();

        if (_visualEffect != null)
        {
            _visualEffect.Play();
        }

        Destroy(gameObject, _duration);
    }
}
