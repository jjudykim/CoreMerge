using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoreDropManager
{
    private readonly Dictionary<MonsterType, MonsterTypeDropTable> dropTableByType = new();
    
    private bool isInit;

    public void Init(MonsterTypeDropTable[] tables)
    {
        dropTableByType.Clear();

        foreach (var table in tables)
        {
            if (table == null)
                continue;

            if (dropTableByType.ContainsKey(table.monsterType))
            {
                Debug.Log($"CoreDropManager ::: 중복된 몬스터 등급 드랍테이블 : {table.monsterType}");
                continue;
            }
            
            dropTableByType.Add(table.monsterType, table);
        }

        isInit = true;
        Debug.Log($"CoreDropManager ::: Init done.");
    }

    // 1개 드롭용 (for test)
    public CoreData GetDropCore(MonsterType type)
    {
        if (isInit == false)
        {
            Debug.Log("CoreDropManager ::: Init NOT Called");
            return null;
        }

        if (dropTableByType.TryGetValue(type, out var table))
        {
            Debug.Log($"CoreDropManager ::: {type}에 해당하는 드랍테이블 없음");
            return null;
        }

        if (Random.value > table.dropChance)
            return null;

        var tier = RollTierFromTable(table);
        if (tier == null)
            return null;
        
        return Managers.Instance.CoreDB.GetRandomCoreByTier(tier);
    }
    
    // 여러개 드롭용
    public List<CoreData> GetDropCores(MonsterType type, int minCount, int maxCount)
    {
        var result = new List<CoreData>();

        if (isInit == false)
        {
            Debug.Log("CoreDropManager ::: Init NOT Called");
            return result;
        }

        if (dropTableByType.TryGetValue(type, out var table))
        {
            Debug.Log($"CoreDropManager ::: {type}에 해당하는 드랍테이블 없음");
            return null;
        }

        if (minCount < 0)
            minCount = 0;
        if (maxCount < minCount)
            maxCount = minCount;

        if (Random.value > table.dropChance)
            return result;

        int dropCount = Random.Range(minCount, maxCount + 1);

        for (int i = 0; i < dropCount; ++i)
        {
            ECoreTier tier = RollTierFromTable(table);
            
            CoreData core = Managers.Instance.CoreDB.GetRandomCoreByTier(tier);
            if (core != null)
                result.Add(core);
        }

        return result;
    }

    private ECoreTier RollTierFromTable(MonsterTypeDropTable table)
    {
        float totalWeight = 0f;
        foreach (var entry in table.tierEntries)
        {
            if (entry.weight <= 0f)
                continue;

            totalWeight += entry.weight;
        }

        float roll = UnityEngine.Random.Range(0f, totalWeight);
        float cumulative = 0f;

        foreach (var entry in table.tierEntries)
        {
            if (entry.weight <= 0f)
                continue;

            cumulative += entry.weight;

            if (roll <= cumulative)
                return entry.tier;
        }

        return table.tierEntries[table.tierEntries.Length - 1].tier;
    }

}