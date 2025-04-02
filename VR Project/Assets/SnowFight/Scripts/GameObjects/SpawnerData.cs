using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerData", menuName = "Game Data/Spawner Data")]
public class SpawnerData : ScriptableObject
{
    [SerializeField] public GameObject[] _enemies;
    public int GetArrayLength()
    {
        return _enemies.Length;
    }
}