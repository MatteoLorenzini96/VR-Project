using UnityEngine;

public class OctagonBuilder : MonoBehaviour
{
    [Header("Prefab e Distanza")]
    public GameObject edgePrefab;
    public float radius = 2f;

    [Header("Target del centro (player)")]
    public Transform playerTransform;

    private const int sides = 8;

    void Start()
    {
        if (playerTransform == null && Camera.main != null)
        {
            playerTransform = Camera.main.transform;
        }

        BuildOctagon();
    }

    void BuildOctagon()
    {
        Vector3 center = playerTransform.position;
        Vector3[] vertices = new Vector3[sides];

        // Calcola i vertici dell’ottagono su un piano orizzontale
        for (int i = 0; i < sides; i++)
        {
            float angle = i * Mathf.PI * 2f / sides;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            vertices[i] = center + new Vector3(x, 0, z);
        }

        // Crea i lati tra i vertici
        for (int i = 0; i < sides; i++)
        {
            Vector3 start = vertices[i];
            Vector3 end = vertices[(i + 1) % sides];
            CreateEdge(start, end);
        }
    }

    void CreateEdge(Vector3 start, Vector3 end)
    {
        if (edgePrefab == null) return;

        Vector3 direction = end - start;
        Vector3 position = start + direction / 2f;

        // Alziamo di 0.01 sulla Y
        position.y += 0.01f;

        GameObject edge = Instantiate(edgePrefab, position, Quaternion.identity, transform);

        // Allinea il prefab tra i due punti
        edge.transform.forward = direction.normalized;

        // Ruota il prefab di 90° sull’asse X rispetto alla direzione calcolata
        edge.transform.Rotate(0f, 90f, 0f, Space.Self);

        // Scala il prefab per adattarlo alla distanza
        /*float length = direction.magnitude;
        edge.transform.localScale = new Vector3(length, edge.transform.localScale.y, edge.transform.localScale.z);*/
    }

}
