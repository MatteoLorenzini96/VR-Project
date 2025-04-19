using UnityEngine;

public class DetonationTrigger : MonoBehaviour
{
    private BombAI _bombAI; // Riferimento all'oggetto BombAI

    private void Start()
    {
        _bombAI = GetComponentInParent<BombAI>();
        if (_bombAI == null)
        {
            Debug.LogError("BombAI non trovato nel parent!");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("SnowWall") || other.CompareTag("SnowBlock"))
        {
            //Debug.Log("Trigger attivato con " + other.gameObject.name);

            //_bombAI.CreateExplosionCollider();
            _bombAI.TargetReached();
        }
    }
}
