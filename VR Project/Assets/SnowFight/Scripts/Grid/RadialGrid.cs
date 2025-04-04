using System.Collections.Generic;
using UnityEngine;

public class RadialGrid : MonoBehaviour
{
    [SerializeField] private float _radius = 2.0f; // Distanza della griglia dal player
    [SerializeField] public float GridSize = 0.5f; // Dimensione delle celle della griglia
    [SerializeField] private Color _gridColor = Color.green; // Colore della griglia

    private List<GameObject> _gridLines = new List<GameObject>();
    private Transform _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (_player == null)
        {
            Debug.LogError("Player tag not found! Make sure there is a GameObject with the tag 'Player'.");
            return;
        }
        GenerateGrid();
    }

    private void Update()
    {
        if (_player != null)
        {
            transform.position = _player.position;
            GenerateGrid();
        }
    }

    private void GenerateGrid()
    {
        ClearGrid();

        for (float x = -_radius; x <= _radius; x += GridSize)
        {
            for (float z = -_radius; z <= _radius; z += GridSize)
            {
                Vector3 worldPos = transform.position + new Vector3(x, 0, z);
                if ((worldPos - transform.position).magnitude <= _radius && IsOnGridEdge(worldPos))
                {
                    CreateGridCell(worldPos);
                }
            }
        }
    }

    private void ClearGrid()
    {
        foreach (var line in _gridLines)
        {
            Destroy(line);
        }
        _gridLines.Clear();
    }

    private void CreateGridCell(Vector3 position)
    {
        GameObject gridCell = new GameObject("GridCell");
        gridCell.transform.position = position;
        LineRenderer lineRenderer = gridCell.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = _gridColor;
        lineRenderer.endColor = _gridColor;
        lineRenderer.positionCount = 5;

        Vector3[] corners = new Vector3[5]
        {
            position + new Vector3(-GridSize / 2, 0, -GridSize / 2),
            position + new Vector3(-GridSize / 2, 0, GridSize / 2),
            position + new Vector3(GridSize / 2, 0, GridSize / 2),
            position + new Vector3(GridSize / 2, 0, -GridSize / 2),
            position + new Vector3(-GridSize / 2, 0, -GridSize / 2)
        };

        lineRenderer.SetPositions(corners);
        _gridLines.Add(gridCell);
    }

    private bool IsOnGridEdge(Vector3 position)
    {
        Vector3 localPos = transform.InverseTransformPoint(position);
        float distance = new Vector2(localPos.x, localPos.z).magnitude;
        return Mathf.Abs(distance - _radius) < GridSize;
    }

    public Vector3 GetClosestGridPosition(Vector3 position)
    {
        Vector3 direction = position - transform.position;
        float distance = direction.magnitude;

        if (distance > _radius)
        {
            direction = direction.normalized * _radius;
        }

        Vector3 localPosition = transform.InverseTransformPoint(transform.position + direction);
        localPosition.x = Mathf.Round(localPosition.x / GridSize) * GridSize;
        localPosition.z = Mathf.Round(localPosition.z / GridSize) * GridSize;

        Vector3 snappedPosition = transform.TransformPoint(localPosition);

        if (!IsOnGridEdge(snappedPosition))
        {
            snappedPosition = FindNearestEdgePosition(snappedPosition);
        }

        return snappedPosition;
    }

    private Vector3 FindNearestEdgePosition(Vector3 position)
    {
        Vector3 direction = (position - transform.position).normalized;
        return transform.position + direction * _radius;
    }
}
