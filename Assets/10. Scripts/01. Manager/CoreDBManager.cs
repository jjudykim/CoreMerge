using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class TierDropEntry
{
    public ECoreTier tier;
    public float weight;
}

[Serializable]
public class MonsterTypeDropTable
{
    public MonsterType monsterType;

    [Range(0f, 1f)] 
    public float dropChance = 1f;

    public TierDropEntry[] tierEntries;
}

public class CoreDBManager
{
    private Dictionary<int, CoreData> coreDataById = new();
    private readonly Dictionary<ECoreTier, List<CoreData>> coreDataByTier = new();

    private bool isInit;
    
    public void Init()
    {
        if (isInit)
            return;
        
        coreDataById.Clear();
        coreDataByTier.Clear();

        // 1. Resources 폴더 내의 모든 코어 SO를 한 번에 로드
        CoreDataSO[] allCoreData = Resources.LoadAll<CoreDataSO>("SO/Core");
        if (allCoreData == null || allCoreData.Length == 0)
        {
            Debug.LogWarning("CoreDBManager ::: Resources/Core 경로에서 CoreDataSO를 찾지 못함");
            isInit = true;
            return;
        }
        
        // 2. ID를 키로 하여 Dictionary에 캐싱 (O(1) 검색용)
        foreach(var so in allCoreData)
        {
            CoreData data = so.data;
            int id = data.id;

            if (coreDataById.ContainsKey(id))
            {
                Debug.Log($"CoreDBManager ::: 중복된 Core ID : {id}");
                continue;
            }

            coreDataById.Add(id, so.data);
            
            // 3. 티어별로 그룹화 (랜덤 드랍 시스템용)
            if (coreDataByTier.TryGetValue(data.tier, out var list) == false)
            {
                list = new List<CoreData>();
                coreDataByTier.Add(data.tier, list);
            }
            
            list.Add(data);
        }

        isInit = true;
        Debug.Log($"CoreDBManager ::: Init done.");
    }

    public CoreData GetCoreDataOrNull(int id)
    {
        if (isInit == false)
        {
            Debug.LogError("CoreDBManager ::: Init Not Called.");
            return null;
        }

        if (coreDataById.TryGetValue(id, out var data))
            return data;
        
        Debug.LogWarning($"CoreDBManager ::: 해당 ID의 코어를 찾지 못함 : {id}");
        return null;
    }

    public int GetFirstCoreIdByTier(ECoreTier tier)
    {
        return coreDataByTier[tier][0].id;
    }

    public bool HasCore(int id)
    {
        return isInit && coreDataById.ContainsKey(id);
    }

    public CoreData GetRandomCoreByTier(ECoreTier tier)
    {
        if (isInit == false)
        {
            Debug.Log("CoreDBManager ::: Init Not Called.");
            return null;
        }

        if (coreDataByTier.TryGetValue(tier, out var list) == false || list.Count == 0)
        {
            Debug.Log($"CoreDBManager ::: {tier}티어의 코어 없음");
        }

        int index = Random.Range(0, list.Count);
        return list[index];
    }
}