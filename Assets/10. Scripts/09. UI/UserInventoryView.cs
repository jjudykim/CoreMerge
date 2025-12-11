using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserInventoryView : InventoryViewBase
{
    [Header("Grid Settings")]
    [SerializeField] private int maxStorageSize = 40;
    [SerializeField] private int sizeColumn = 5;

    [Header("Prefabs & Parents")]
    [SerializeField] private Slot slotPrefab;
    [SerializeField] private Transform slotParent;
    
    private int sizeRow;
    private Slot[,] slotsGrid;
    
    private void Awake()
    {
        container = Managers.Instance.CoreInventory;

    }
    
    private void Start()
    {
        sizeRow = maxStorageSize % sizeColumn == 0 ? maxStorageSize / sizeColumn
                                                   : maxStorageSize / sizeColumn + 1;

        foreach(Transform child in slotParent)
            Destroy(child.gameObject);
        
        slotsGrid = new Slot[sizeRow, sizeColumn];

        for (int i = 0; i < maxStorageSize; ++i)
        {
            Slot slot = Instantiate(slotPrefab, slotParent);
            
            int row = i / sizeColumn;
            int col = i % sizeColumn;

            if (row >= sizeRow)
                break;
            
            slotsGrid[row, col] = slot;
            slot.Index = i;
        }

        Refresh();
    }

    private void OnEnable()
    {
        if (Managers.Instance.CoreInventory != null)
            Managers.Instance.CoreInventory.OnChanged += Refresh;
    }
    
    public override void Refresh()
    {
        if (container == null)
            return;
        
        foreach (var slot in slotsGrid)
            slot.Clear();

        var allSlots = container.GetAllSlots();

        foreach (var slotData in allSlots)
        {
            int index = slotData.Index;
            if (index < 0 || index >= maxStorageSize)
                continue;
            
            int row = index / sizeColumn;
            int col = index % sizeColumn;

            Slot slot = slotsGrid[row, col];
            slot.SetItem(slotData.ItemId, slotData.Count);
        }
    }

    public bool TryGetIndexOfSlot(Slot slot, out int index)
    {
        index = -1;
        if (slot == null || slotsGrid == null)
            return false;

        for (int row = 0; row < sizeRow; ++row)
        {
            for (int col = 0; col < sizeColumn; ++col)
            {
                if (slotsGrid[row, col] == slot)
                {
                    index = row * sizeColumn + col;
                    return true;
                }
            }
        }

        return false;
    }
}
