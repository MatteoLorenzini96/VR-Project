using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SnowballSpawner : MonoBehaviour
{
    [Header("Snowball Settings")]
    public GameObject SnowballPrefab; // Prefab della palla di neve
    public LayerMask SnowLayer; // Layer che rappresenta la "neve"
    public float SpawnCooldown = 1f; // Tempo minimo tra spawn
    public float HandCheckDistance = 0.2f; // Distanza massima dal pavimento per creare la palla

    [Header("Player Settings")]
    public float BendThreshold = 0.4f; // Altezza relativa per il piegamento

    private XRNode _headNode = XRNode.Head;
    private XRNode _leftHandNode = XRNode.LeftHand;
    private XRNode _rightHandNode = XRNode.RightHand;
    private float _lastSpawnTime = 0f;
    private Transform _xrOrigin;

    void Start()
    {
        _xrOrigin = Camera.main.transform.root;
    }

    void Update()
    {
        if (Time.time - _lastSpawnTime < SpawnCooldown)
            return;

        Vector3 headPosition;
        if (TryGetHeadPosition(out headPosition))
        {
            float relativeHeight = headPosition.y - _xrOrigin.position.y;
            if (relativeHeight < BendThreshold)
            {
                CheckHandPositionAndSpawn();
                _lastSpawnTime = Time.time;
            }
        }
    }

    private bool TryGetHeadPosition(out Vector3 position)
    {
        InputDevices.GetDeviceAtXRNode(_headNode).TryGetFeatureValue(CommonUsages.devicePosition, out position);
        return position != Vector3.zero;
    }

    private bool TryGetHandPosition(XRNode handNode, out Vector3 position)
    {
        return InputDevices.GetDeviceAtXRNode(handNode).TryGetFeatureValue(CommonUsages.devicePosition, out position);
    }

    private void CheckHandPositionAndSpawn()
    {
        Vector3 leftHandPos, rightHandPos;
        bool leftHandValid = TryGetHandPosition(_leftHandNode, out leftHandPos);
        bool rightHandValid = TryGetHandPosition(_rightHandNode, out rightHandPos);

        if (leftHandValid && IsTouchingSnow(leftHandPos))
        {
            SpawnSnowball(leftHandPos);
        }
        else if (rightHandValid && IsTouchingSnow(rightHandPos))
        {
            SpawnSnowball(rightHandPos);
        }
    }

    private bool IsTouchingSnow(Vector3 handPosition)
    {
        return Physics.Raycast(handPosition, Vector3.down, HandCheckDistance, SnowLayer);
    }

    private void SpawnSnowball(Vector3 spawnPosition)
    {
        GameObject snowball = Instantiate(SnowballPrefab, spawnPosition, Quaternion.identity);
        var interactable = snowball.GetComponent<XRGrabInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSnowballGrabbed);
        }
    }

    private void OnSnowballGrabbed(SelectEnterEventArgs args)
    {
        Rigidbody rb = args.interactableObject.transform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }
    }
}
