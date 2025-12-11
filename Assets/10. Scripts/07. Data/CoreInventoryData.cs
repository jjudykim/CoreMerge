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

        if (changed)
            if (OnChanged != null)
                OnChanged.Invoke();

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