using System;
using System.Collections.Generic;

public class QuickSlotData : IItemContainer
{
    private readonly int[] coreIds;

    public int Capacity => coreIds.Length;
    
    public event Action OnChanged;

    public QuickSlotData(int capacity)
    {
        coreIds = new int[capacity];
    }

    public void ClearAllSlots()
    {
        Array.Clear(coreIds, 0, coreIds.Length);
        OnChanged?.Invoke();
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
        for (int i = 0; i < coreIds.Length; ++i)
        {
            int itemId = coreIds[i];
            if (itemId == 0)
                continue;

            yield return new InventorySlotData(i, itemId, 1);
        }
    }

    public bool ClearSlot(int index)
    {
        if (index < 0 || coreIds.Length <= index)
            return false;

        if (coreIds[index] == 0)
            return false;

        coreIds[index] = 0;
        
        if (OnChanged != null) 
            OnChanged.Invoke();
        
        return true;
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

    public int GetItemIdAt(int index)
    {
        if (index < 0 || coreIds.Length <= index)
            return 0;

        return coreIds[index];
    }
}
