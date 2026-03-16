using System.Collections.Generic;

public struct InventorySlotData
{
    public int Index;
    public int ItemId;
    public int Count;

    public InventorySlotData(int index, int itemId, int count)
    {
        Index = index;
        ItemId = itemId;
        Count = count;
    }
}

public interface IItemContainer
{
    int Capacity { get; }

    bool TryAdd(int itemId, int count = 1);
    bool TryRemove(int itemId, int count = 1);
    int GetCount(int itemId);

    IEnumerable<InventorySlotData> GetAllSlots();
}