using UnityEngine;

public class DetonationTrigger : MonoBehaviour
{
    public BombAI bombAI; // Riferimento all'oggetto BombAI

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("SnowWall"))
        {
            //Debug.Log("Trigger attivato con " + other.gameObject.name);

            bombAI.isMoving = false;  // Ferma il movimento
            bombAI.CreateExplosionCollider();
            bombAI.TargetReached();
        }
    }
}
