using UnityEngine;

public class SnowBallDetach : MonoBehaviour
{
    private Vector3 initialPosition;
    private Transform originalParent;
    private bool hasDetached = false; 

    private void Start()
    {
        initialPosition = transform.position;
        originalParent = transform.parent;
    }

    public void DetachAndNotifyParent()
    {
        if (hasDetached)
            return;

        hasDetached = true;

        GameObject snowBallsParent = GameObject.Find("SnowBallsHolder");
        if (snowBallsParent == null)
        {
            snowBallsParent = new GameObject("SnowBallsHolder");
        }

        transform.SetParent(null);

        transform.SetParent(snowBallsParent.transform);

        SnowBallsHolder parentScript = snowBallsParent.GetComponent<SnowBallsHolder>();
        if (parentScript != null)
        {
            parentScript.SpawnPrefab(initialPosition, originalParent);
        }
        else
        {
            Debug.LogWarning("Componente SnowBallsParent non trovato su SnowBalls.");
        }
    }
}
