using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine;
using System.Collections.Generic;

public class SnowBlock : MonoBehaviour
{
    private RadialGrid _grid;
    private List<SnowBlock> _blocksAbove = new List<SnowBlock>();
    private SnowBlock _blockBelow;
    private Rigidbody _rigidbody;
    private bool _isFalling = false; // Flag per verificare se l'oggetto sta cadendo

    private void Awake()
    {
        _grid = FindAnyObjectByType<RadialGrid>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (_rigidbody != null)
        {
            // Verifica se l'oggetto sta cadendo (velocità lungo l'asse Y negativa)
            if (_rigidbody.linearVelocity.y < 0)
            {
                _isFalling = true; // Sta cadendo
            }

            // Se l'oggetto sta cadendo e si avvicina alla griglia
            if (_isFalling && Vector3.Distance(transform.position, _grid.transform.position) < _grid.GridSize)
            {
                SnapToGrid(); // Effettua lo snap
            }
        }
    }

    private void SnapToGrid()
    {
        // Trova la posizione più vicina sulla griglia
        Vector3 snappedPosition = _grid.GetClosestGridPosition(transform.position);

        // Se la distanza dalla posizione snappata è accettabile, applica lo snap
        if (Vector3.Distance(transform.position, snappedPosition) < _grid.GridSize)
        {
            transform.position = snappedPosition;
            CheckForBlockBelow();
            _isFalling = false; // Reset del flag di caduta
        }
    }

    private void CheckForBlockBelow()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.0f))
        {
            SnowBlock belowBlock = hit.collider.GetComponent<SnowBlock>();
            if (belowBlock != null)
            {
                _blockBelow = belowBlock;
                belowBlock.AddBlockAbove(this);
            }
        }
    }

    public void AddBlockAbove(SnowBlock block)
    {
        _blocksAbove.Add(block);
    }
}
