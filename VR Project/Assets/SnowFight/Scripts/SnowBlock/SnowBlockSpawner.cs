using UnityEngine;

public class SnowBlockSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab; // Prefab del blocco di neve
    private Vector3 _initialPosition; // Posizione iniziale dello spawner

    private void Start()
    {
        _initialPosition = transform.position;
        CheckAndRespawn();
    }

    private void Update()
    {
        CheckAndRespawn();
    }

    private void CheckAndRespawn()
    {
        if (transform.childCount == 0)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (_prefab != null)
        {
            GameObject newBlock = Instantiate(_prefab, _initialPosition, Quaternion.identity);
            newBlock.transform.parent = transform; // Assegna il nuovo blocco come figlio dello spawner
        }
        else
        {
            Debug.LogWarning("Prefab non assegnato in " + gameObject.name);
        }
    }
}
