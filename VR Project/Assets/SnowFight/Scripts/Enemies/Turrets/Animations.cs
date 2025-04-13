using UnityEngine;
using System.Collections.Generic;

public class Animations : MonoBehaviour
{
    [Header("Renderer Settings")]
    [SerializeField] private GameObject _targetObject;
    [SerializeField] private Material _replacementMaterial;

    [Header("Target Settings")]
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Collider _targetCollider;

    private List<Renderer> _renderers = new List<Renderer>();
    private List<Material> _originalMaterials = new List<Material>();
    private Coroutine _rotationCoroutine;

    private void Awake()
    {
        if (_targetObject != null)
        {
            _renderers.AddRange(_targetObject.GetComponentsInChildren<Renderer>());

            foreach (Renderer renderer in _renderers)
            {
                _originalMaterials.Add(renderer.material);
            }
        }
    }

    public void ChangeMaterial()
    {
        if (_replacementMaterial == null) return;

        foreach (Renderer renderer in _renderers)
        {
            renderer.material = _replacementMaterial;
        }
    }

    public void ResetMaterial()
    {
        for (int i = 0; i < _renderers.Count; i++)
        {
            if (i < _originalMaterials.Count)
            {
                _renderers[i].material = _originalMaterials[i];
            }
        }
    }

    public void TargetDown()
    {
        if (_rotationCoroutine != null) StopCoroutine(_rotationCoroutine);
        _rotationCoroutine = StartCoroutine(RotateToXAngle(180f));
        if (_targetCollider != null)
        {
            _targetCollider.enabled = true;
        }
    }

    public void TargetUp()
    {
        if (_rotationCoroutine != null) StopCoroutine(_rotationCoroutine);
        _rotationCoroutine = StartCoroutine(RotateToXAngle(90f));
    }

    private IEnumerator<WaitForEndOfFrame> RotateToXAngle(float targetX)
    {
        if (_targetTransform == null) yield break;

        Quaternion startRotation = _targetTransform.rotation;
        Vector3 currentEuler = _targetTransform.eulerAngles;
        Quaternion endRotation = Quaternion.Euler(targetX, currentEuler.y, currentEuler.z);

        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            _targetTransform.rotation = Quaternion.Slerp(startRotation, endRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        _targetTransform.rotation = endRotation;
    }
}
