using System;
using System.Collections.Generic;

public class CoreInventoryData : IItemContainer
{
    private readonly int[] coreIds;
    public int Capacity => coreIds.Length;
    public event Action OnChanged; 
    
    public CoreInventoryData(int capacity)
    {
        coreIds = new int[capacity];
    }

    public void ResetInventory()
    {
        Array.Clear(coreIds, 0, coreIds.Length);
        OnChanged?.Invoke();
    }

    public void SortByTier()
    {
        // 1. 현재 인벤토리의 모든 아이템 정보를 가져와서
        List<int> items = new List<int>();
        for (int i = 0; i < coreIds.Length; ++i)
        {
            if (coreIds[i] != 0)
                items.Add(coreIds[i]);
        }
        
        // 2. 티어 기준으로 내림차순
        items.Sort((a, b) =>
        {
            CoreData dataA = Managers.Instance.CoreDB.GetCoreDataOrNull(a);
            CoreData dataB = Managers.Instance.CoreDB.GetCoreDataOrNull(b);

            if (dataA == null || dataB == null)
                return 0;
            
            int tierCompare = dataB.tier.CompareTo(dataA.tier);

            if (tierCompare == 0)
                return a.CompareTo(b);

            return tierCompare;
        });
        
        // 3. 인벤토리 배열 초기화 및 정렬된 아이템 재배치
        Array.Clear(coreIds, 0, coreIds.Length);
        for (int i = 0; i < items.Count; ++i)
        {
            coreIds[i] = items[i];
        }

        OnChanged?.Invoke();
    }

    private int GetTotalCount()
    {
        int total = 0;
        for (int i = 0; i < coreIds.Length; ++i)
        {
            if (coreIds[i] != 0)
                total++;
        }

        return total;
    }

    public bool TryAdd(int itemId, int count = 1)
    {
        if (count <= 0)
            return false;

        int emptySlots = 0;
        for (int i = 0; i < coreIds.Length; ++i)
        {
            if (coreIds[i] == 0)
                emptySlots++;
        }
        
        if (emptySlots < count)
            return false;

        int remain = count;
        for (int i = 0; i < coreIds.Length && remain > 0; ++i)
        {
            if (coreIds[i] == 0)
            {
                coreIds[i] = itemId;
                remain--;
            }
        }

        if (OnChanged != null) 
            OnChanged.Invoke();
        
        return true;
    }

    public bool TryRemove(int itemId, int count = 1)
    {
        if (count <= 0)
            return false;

        int have = GetCount(itemId);
        if (have < count)
            return false;

        int remain = count;
        for (int i = 0; i < coreIds.Length && remain > 0; ++i)
        {
            if (coreIds[i] == itemId)
            {
                coreIds[i] = 0;
                remain--;
            }
        }

        if (OnChanged != null) 
            OnChanged.Invoke();
        
        return true;
    }

    public int GetCount(int itemId)
    {
        int count = 0;
        for (int i = 0; i < coreIds.Length; ++i)
        {
            if (coreIds[i] == itemId)
                count++;
        }

        return count;
    }
    
    public IEnumerable<InventorySlotData> GetAllSlots()
    {
        for(int i = 0; i < coreIds.Length; ++i)
        {
            int itemId = coreIds[i];
            if (itemId == 0)
                continue;

            yield return new InventorySlotData(i, itemId, 1);
        }
    }

    public bool ClearSlot(int index)
    {
        if (index < 0 || index >= coreIds.Length)
            return false;
        
        if (coreIds[index] == 0)
            return false;

        coreIds[index] = 0;
        
        if (OnChanged != null) 
            OnChanged.Invoke();
        
        return true;
    }

    public bool TryMergeAll()
    {
        bool changed = false;

        var tierToSlots = new Dictionary<ECoreTier, List<int>>();

        for (int i = 0; i < coreIds.Length; ++i)
        {
            int coreId = coreIds[i];
            if (coreId == 0)
                continue;

            CoreData data = Managers.Instance.CoreDB.GetCoreDataOrNull(coreId);
            if (data == null)
                continue;

            ECoreTier tier = data.tier;

            if (tierToSlots.TryGetValue(tier, out var list) == false)
            {
                list = new List<int>();
                tierToSlots[tier] = list;
            }

            list.Add(i);
        }

        foreach (var pair in tierToSlots)
        {
            ECoreTier tier = pair.Key;
            var slots = pair.Value;

            int pairCount = slots.Count / 2;
            if (pairCount <= 0)
                continue;

            ECoreTier nextTier = (ECoreTier)((int)tier + 1);
            
            if (Enum.IsDefined(typeof(ECoreTier), nextTier) == false)
                continue;

            for (int p = 0; p < pairCount; p++)
            {
                int indexA = slots[p * 2];
                int indexB = slots[p * 2 + 1];

                int oldIdA = coreIds[indexA];
                int oldIdB = coreIds[indexB];

                if (oldIdA == 0 || oldIdB == 0)
                    continue;
                
                coreIds[indexA] = 0;
                coreIds[indexB] = 0;
                
                int upgradedCoreId = Managers.Instance.CoreDB.GetFirstCoreIdByTier(nextTier);
                coreIds[indexA] = upgradedCoreId;

                changed = true;
            }
        }

        SortByTier();

        return changed;
    }
    
    public int GetItemIdAt(int index)
    {
        if (index < 0 || coreIds.Length <= index)
            return 0;
        
        return coreIds[index];
    }

    public bool TryAssignAt(int index, int itemId)
    {
        if (index < 0 || coreIds.Length <= index)
            return false;

        coreIds[index] = itemId;

        if (OnChanged != null) 
            OnChanged.Invoke();

        return true;
    }
}

