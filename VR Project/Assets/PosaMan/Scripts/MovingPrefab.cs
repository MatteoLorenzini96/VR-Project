using UnityEngine;
using System.Collections;

public class MovingPrefab : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;

    private Transform _playerTransform;
    private Vector3 _playerLastPosition;
    private bool _hasPassedPlayer = false;

    public void Initialize(Transform playerTransform)
    {
        _playerTransform = playerTransform;
        _playerLastPosition = _playerTransform.position;  // Salva la posizione iniziale del Player
    }

    public void StartMoving()
    {
        StartCoroutine(MoveTowardsPlayer());
    }

    private IEnumerator MoveTowardsPlayer()
    {
        Vector3 directionToPlayer = (_playerLastPosition - transform.position).normalized;

        while (!_hasPassedPlayer)
        {
            transform.position += directionToPlayer * _moveSpeed * Time.deltaTime;

            // Controllo se il prefab ha superato il Player sulla Z o sulla X (a seconda della direzione principale)
            if (Mathf.Abs(transform.position.z - _playerLastPosition.z) < 0.5f &&
                Mathf.Abs(transform.position.x - _playerLastPosition.x) < 0.5f)
            {
                _hasPassedPlayer = true;
            }

            yield return null;
        }

        // Attendi 1 secondo prima di distruggere l'oggetto
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
