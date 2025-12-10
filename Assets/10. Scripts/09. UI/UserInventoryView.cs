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
    public Slot[,] SlotsGrid;
    
    private void Awake()
    {
        container = Managers.Instance.CoreInventory;

        if (Managers.Instance.CoreInventory != null)
            Managers.Instance.CoreInventory.OnChanged += Refresh;
    }

    private void OnDestroy()
    {
        if (Managers.Instance.CoreInventory != null)
            Managers.Instance.CoreInventory.OnChanged -= Refresh;
    }

    private void Start()
    {
        sizeRow = maxStorageSize % sizeColumn == 0 ? maxStorageSize / sizeColumn
            : maxStorageSize / sizeColumn + 1;

        foreach(Transform child in slotParent)
            Destroy(child.gameObject);
        
        SlotsGrid = new Slot[sizeRow, sizeColumn];

        for (int i = 0; i < maxStorageSize; ++i)
        {
            Slot slot = Instantiate(slotPrefab, slotParent);
            
            int row = i / sizeColumn;
            int col = i % sizeColumn;

            if (row >= sizeRow)
                break;
            
            SlotsGrid[row, col] = slot;
            slot.Index = i;
        }

        Refresh();
    }
    
    public override void Refresh()
    {
        if (container == null)
            return;
        
        foreach (var slot in SlotsGrid)
            slot.Clear();

        var allSlots = container.GetAllSlots();

        foreach (var slotData in allSlots)
        {
            int index = slotData.Index;
            if (index < 0 || index >= maxStorageSize)
                continue;
            
            int row = index / sizeColumn;
            int col = index % sizeColumn;

            Slot slot = SlotsGrid[row, col];
            slot.SetItem(slotData.ItemId, slotData.Count);
        }
    }
}