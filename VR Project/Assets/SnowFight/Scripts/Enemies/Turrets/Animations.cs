using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    private Transform _playerTransform;

    private Coroutine _rotationCoroutine;
    private Coroutine _targetUpCoroutine;
    private Coroutine _targetDownCoroutine;

    private void Awake()
    {
        LookAtPlayer();

        if (_targetObject != null)
        {
            _renderers.AddRange(_targetObject.GetComponentsInChildren<Renderer>());

            foreach (Renderer renderer in _renderers)
            {
                _originalMaterials.Add(renderer.material);
            }
        }
    }

    private void LookAtPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("PlayerPoint");
        if (playerObject)
        {
            _playerTransform = playerObject.transform;
            transform.LookAt(_playerTransform);
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

    private IEnumerator TargetDown()
    {
        if (_rotationCoroutine != null)
            yield break; // o StopCoroutine(_rotationCoroutine); se vuoi forzare

        _rotationCoroutine = StartCoroutine(RotateToXAngle(0f, 45f));
        yield return _rotationCoroutine;

        if (_targetCollider != null)
        {
            _targetCollider.enabled = true;
        }

        //Debug.Log("Attesa TatgetDown iniziata");
        yield return new WaitForSeconds(.5f);
        //Debug.Log("Attesa TatgetDown finita");

        _targetDownCoroutine = null;
    }

    public void BegingTargetDown()
    {
        if (_targetDownCoroutine != null)
        {
            return;
        }
        _targetDownCoroutine = StartCoroutine(TargetDown());
    }

    private IEnumerator TargetUp()
    {
        if (_rotationCoroutine != null)
            yield break;

        _rotationCoroutine = StartCoroutine(RotateToXAngle(90f, 45f));
        yield return _rotationCoroutine;

        if (_targetCollider != null)
        {
            _targetCollider.enabled = false;
        }

        //Debug.Log("Attesa TatgetUp iniziata");
        yield return new WaitForSeconds(2f);
        //Debug.Log("Attesa TatgetUp finita");

        _targetUpCoroutine = null;
    }

    public void BegingTargetUp()
    {
        if (_targetUpCoroutine != null)
        {
            return;
        }
        _targetUpCoroutine = StartCoroutine(TargetUp());
    }

    private IEnumerator RotateToXAngle(float targetX, float degreesPerSecond = 90f)
    {
        if (_targetTransform == null) yield break;

        // Prendiamo solo la componente X, ma manteniamo Y e Z attuali
        float currentX = _targetTransform.localEulerAngles.x;
        float targetAngle = Mathf.DeltaAngle(currentX, targetX); // Delta tra -180 e 180
        float direction = Mathf.Sign(targetAngle); // -1 o 1
        float angleRemaining = Mathf.Abs(targetAngle);

        while (angleRemaining > 0.1f)
        {
            float deltaAngle = degreesPerSecond * Time.deltaTime;
            float rotationStep = Mathf.Min(deltaAngle, angleRemaining); // evita overshoot

            currentX += rotationStep * direction;
            _targetTransform.localEulerAngles = new Vector3(currentX, _targetTransform.localEulerAngles.y, _targetTransform.localEulerAngles.z);

            angleRemaining -= rotationStep;
            yield return null;
        }

        // Imposta con precisione l’angolo finale
        _targetTransform.localEulerAngles = new Vector3(targetX, _targetTransform.localEulerAngles.y, _targetTransform.localEulerAngles.z);

        _rotationCoroutine = null;
    }
}
