using UnityEngine;

public class GameManager
{
    private Core corePrefab;
    
    public void Init()
    {
        corePrefab = Resources.Load<GameObject>("Prefabs/Core").GetComponent<Core>();    
    }

    public void ResetGame()
    {
        RespawnPlayer();

        if (Managers.Instance.CoreInventory != null)
            Managers.Instance.CoreInventory.ResetInventory();

        if (Managers.Instance.QuickSlots != null)
            Managers.Instance.QuickSlots.ClearAllSlots();
    }

    public void RespawnPlayer()
    {
        if (Player.LocalPlayer != null)
        {
            GameObject oldPlayer = Player.LocalPlayer.gameObject;
            Player.LocalPlayer = null;
            Object.Destroy(oldPlayer);
        }

        if (Managers.Instance.PlayerPrefab != null)
        {
            GameObject newPlayer = Object.Instantiate(Managers.Instance.PlayerPrefab);
        }
    }

    public void OnPlayerDead()
    {
        Managers.Instance.UI.ShowGameEndUI(false);
    }
    
    public void OnBossDead()
    {
        Managers.Instance.UI.ShowGameEndUI(true);
        Managers.Instance.Save.DeleteSave();
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