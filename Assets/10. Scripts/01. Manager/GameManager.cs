using UnityEngine;

public class GameManager
{
    [SerializeField] private Core corePrefab;

    public void Init()
    {
        
    }

    public void SpawnCore(Vector3 position, int coreId)
    {
        Core pickup = Object.Instantiate(corePrefab, position, Quaternion.identity);
        pickup.Init(coreId);
    }

    public void OnMonsterDead(MonsterController monster)
    {
        if (monster == null)
            return;

        var grade = monster.Owner.Type;

        var droppedCores = Managers.Instance.CoreDrop.GetDropCores(grade, 2, 4);

        if (droppedCores != null)
        {
            foreach (var core in droppedCores)
            {
                if (core == null)
                    continue;

                Vector2 offset = Random.insideUnitCircle * 0.5f;
                Vector3 spawnPos = monster.transform.position + new Vector3(offset.x, offset.y, 0f);

                SpawnCore(spawnPos, core.id);
            }
        }
    }
}