using UnityEngine;

public class GameManager
{
    [SerializeField] private Core corePrefab;

    public void SpawnCore(Vector3 position, int coreId)
    {
        Core pickup = Object.Instantiate(corePrefab, position, Quaternion.identity);
        pickup.Init(coreId);
    }
}