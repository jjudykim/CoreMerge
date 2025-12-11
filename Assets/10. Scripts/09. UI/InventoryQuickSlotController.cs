using UnityEngine;

public class InventoryQuickSlotController : MonoBehaviour
{
    [SerializeField] private UserInventoryView inventoryView;
    [SerializeField] private QuickSlotView quickSlotView;

    private CoreInventoryData coreInventory => Managers.Instance.CoreInventory;
    private QuickSlotData quickSlots => Managers.Instance.QuickSlots;

    private bool hasSelection;
    private bool selectionIsInventory;
    private int selectionIndex;
    private int selectionItemId;

    public void UnequipQuickSlot(int quickSlotIndex)
    {
        int itemId = quickSlots.GetItemIdAt(quickSlotIndex);
        if (itemId == 0)
            return;

        bool added = coreInventory.TryAdd(itemId, 1);
        if (added == false)
        {
            Debug.Log("QuickSlotCtrl ::: Failed to add slot");
            return;
        }

        quickSlots.ClearSlot(quickSlotIndex);
    }

    #region Select

    public void OnInventorySlotClicked(int index, Slot slot)
    {
        if (slot.IsEmptySlot)
        {
            ClearSelection();
            return;
        }

        hasSelection = true;
        selectionIsInventory = true;
        selectionIndex = index;
        selectionItemId = slot.Core.id;
    }

    public void OnQuickSlotClicked(int index, Slot slot)
    {
        if (!hasSelection)
        {
            if (slot.IsEmptySlot)
                return;

            hasSelection = true;
            selectionIsInventory = false;
            selectionIndex = index;
            selectionItemId = slot.Core.id;

            return;
        }

        if (selectionIsInventory)
            MoveCoreFromInventoryToQuickSlot(selectionIndex, index, selectionItemId);
        else
            SwapQuickSlot(selectionIndex, index);
        
        ClearSelection();
    }
    #endregion
    
    #region Move

    private void MoveCoreFromInventoryToQuickSlot(int fromIndex, int toIndex, int itemId)
    {
        bool cleared = coreInventory.ClearSlot(fromIndex);
        if (cleared == false)
        {
            Debug.Log("QuickSlotCtrl ::: Failed to clear slot");
            return;
        }

        int existingId = quickSlots.GetItemIdAt(toIndex);
        if (existingId != 0)
        {
            bool reAdded = coreInventory.TryAdd(existingId, 1);
            if (reAdded == false)
            {
                Debug.Log("QuickSlotCtrl ::: Failed to reAdd slot");
            }
        }

        quickSlots.TryAssignAt(toIndex, itemId);
    }
    
    private void SwapQuickSlot(int indexA, int indexB)
    {
        if (indexA == indexB)
            return;
        
        int idA = quickSlots.GetItemIdAt(indexA);
        int idB = quickSlots.GetItemIdAt(indexB);

        quickSlots.TryAssignAt(indexA, idB);
        quickSlots.TryAssignAt(indexB, idA);
    }
    
    private void ClearSelection()
    {
        hasSelection = false;
        selectionItemId = 0;
    }
    #endregion

}