using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotOwner
{
    Inventory,
    QuickSlot
}

public class UIInputHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Components")]
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private ScrollRect scrollRect;
    
    [Header("Slot & Popup")]
    [SerializeField] private Slot cursorSlot;
    [SerializeField] private CoreInfoPopupUI coreInfoPopup;
    
    [Header("Hover Setting")]
    [SerializeField] private float hoverDelay = 0.3f;
    
    [Header("Reference Setting")]
    [SerializeField] private UserInventoryView inventoryView;
    [SerializeField] private QuickSlotView quickSlotView;

    private CoreInventoryData coreInventory => Managers.Instance.CoreInventory;
    private QuickSlotData quickSlots => Managers.Instance.QuickSlots;

    private bool hasSelection;
    private SlotOwner selectionOwner;
    private int selectionIndex;
    private int selectionItemId;

    private Slot startSlot;
    private SlotOwner startOwner;
    private int startIndex;
    
    private bool isCoreDragging = false;

    private Slot hoverSlot;
    private float hoverTimer = 0f;

    private void Start()
    {
        if (scrollRect == null) 
            scrollRect = GetComponentInChildren<ScrollRect>();
    }

    private void Update()
    {
        HandleHover();
    }

    #region Hover 처리
    private void HandleHover()
    {
        if (isCoreDragging)
        {
            HideCoreInfo();
            return;
        }

        PointerEventData data = new PointerEventData(EventSystem.current) { position =  Input.mousePosition };
        Slot slotUnderMouse = RaycastSlot(data);

        if (slotUnderMouse != hoverSlot)
        {
            hoverSlot = slotUnderMouse;
            hoverTimer = 0f;
            HideCoreInfo();
        }

        if (hoverSlot != null && hoverSlot.Core != null && hoverSlot != cursorSlot)
        {
            hoverTimer += Time.deltaTime;

            if (hoverTimer >= hoverDelay)
                ShowCoreInfo(hoverSlot);
        }
        else
        {
            HideCoreInfo();
        }
    }

    private void ShowCoreInfo(Slot slot)
    {
        if (coreInfoPopup == null)
            return;

        if (slot == null || slot.Core == null)
            return;

        coreInfoPopup.Show(slot.Core);
    }

    private void HideCoreInfo()
    {
        if (coreInfoPopup == null)
            return;

        if (coreInfoPopup.gameObject.activeSelf)
            coreInfoPopup.Hide();
    }
    #endregion
    
    #region Drag & Drop

    public void OnBeginDrag(PointerEventData eventData)
    {
        HideCoreInfo();

        startSlot = RaycastSlot(eventData);
        
        if (startSlot == null || startSlot.Core == null)
        {
            isCoreDragging = false;

            if (scrollRect != null)
                scrollRect.OnBeginDrag(eventData);

            return;
        }
        
        if (TryGetOwnerAndIndex(startSlot, out startOwner, out startIndex) == false)
        {
            isCoreDragging = false;
            return;
        }

        isCoreDragging = true;

        cursorSlot.SetItem(startSlot.Core.id);
        cursorSlot.SetIconImageEnable(true);
        cursorSlot.SetAlpha(0.5f);

        if (scrollRect != null)
            scrollRect.enabled = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isCoreDragging)
            cursorSlot.SetPosition(eventData.position);
        else
        {
            if (scrollRect != null)
                scrollRect.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isCoreDragging)
        {
            if (startSlot == null)
            {
                ResetCursor();
            }
            else
            {
                Slot endSlot = RaycastSlot(eventData);

                if (endSlot == null)
                    ResetCursor();
                else
                {
                    if (TryGetOwnerAndIndex(endSlot, out var endOwner, out var endIndex) == false)
                        ResetCursor();
                    else
                    {
                        HandleDragDrop(startOwner, startIndex, endOwner, endIndex);
                        ResetCursor();
                    }
                }
            }

            if (scrollRect != null)
                scrollRect.enabled = true;

            isCoreDragging = false;

            hoverSlot = null;
            hoverTimer = 0f;
            HideCoreInfo();
        }
        else
        { 
            if (scrollRect != null)
                scrollRect.OnEndDrag(eventData);
        }
    }

    private void HandleDragDrop(SlotOwner fromOwner, int fromIndex, SlotOwner toOwner, int toIndex)
    {
        if (fromOwner == toOwner && fromIndex == toIndex)
            return;

        // 인벤토리 → 인벤토리 (슬롯 스왑)
        if (fromOwner == SlotOwner.Inventory && toOwner == SlotOwner.Inventory)
        {
            int itemId = coreInventory.GetItemIdAt(fromIndex);
            MoveOrSwapInventorySlots(fromIndex, toIndex, itemId);
        }
        // 인벤토리 → 퀵슬롯
        else if (fromOwner == SlotOwner.Inventory && toOwner == SlotOwner.QuickSlot)
        {
            int itemId = coreInventory.GetItemIdAt(fromIndex);
            MoveCoreFromInventoryToQuickSlot(fromIndex, toIndex, itemId);
        }
        // 퀵슬롯 → 인벤토리
        else if (fromOwner == SlotOwner.QuickSlot && toOwner == SlotOwner.Inventory)
        {
            int itemId = quickSlots.GetItemIdAt(fromIndex);
            MoveCoreFromQuickSlotToInventory(fromIndex, toIndex, itemId);
        }
        // 퀵슬롯 ↔ 퀵슬롯 (슬롯 스왑)
        else if (fromOwner == SlotOwner.QuickSlot && toOwner == SlotOwner.QuickSlot)
        {
            SwapQuickSlot(fromIndex, toIndex);
        }
    }


    #endregion
    
    #region Slot 유틸 (Move / Swap / Raycast)
    
    private bool TryGetOwnerAndIndex(Slot slot, out SlotOwner owner, out int index)
    {
        if (inventoryView != null && inventoryView.TryGetIndexOfSlot(slot, out index))
        {
            owner = SlotOwner.Inventory;
            return true;
        }
        
        if (quickSlotView != null && quickSlotView.TryGetIndexOfSlot(slot, out index))
        {
            owner = SlotOwner.QuickSlot;
            return true;
        }

        owner = default;
        index = -1;
        return false;
    }

    private void MoveOrSwapInventorySlots(int fromIndex, int toIndex, int itemId)
    {
        if (fromIndex == toIndex)
            return;
        
        if (coreInventory == null)
            return;

        int idA = coreInventory.GetItemIdAt(fromIndex);
        int idB = coreInventory.GetItemIdAt(toIndex);

        coreInventory.TryAssignAt(fromIndex, idB);
        coreInventory.TryAssignAt(toIndex, idA);
    }
    
    private void MoveCoreFromInventoryToQuickSlot(int fromIndex, int toIndex, int itemId)
    {
        bool cleared = coreInventory.ClearSlot(fromIndex);
        if (cleared == false)
            return;

        int existingId = quickSlots.GetItemIdAt(toIndex);
        if (existingId != 0)
        {
            bool reAdded = coreInventory.TryAdd(existingId, 1);
            if (reAdded == false)
                return;
        }

        quickSlots.TryAssignAt(toIndex, itemId);
    }
    
    private void MoveCoreFromQuickSlotToInventory(int fromIndex, int toIndex, int itemId)
    {
        bool cleared = quickSlots.ClearSlot(fromIndex);
        if (cleared == false)
            return;
        
        if (coreInventory == null)
            return;
        
        int existingId = coreInventory.GetItemIdAt(toIndex);
        coreInventory.TryAssignAt(toIndex, itemId);

        if (existingId != 0)
            quickSlots.TryAssignAt(fromIndex, existingId);
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

    private void ResetCursor()
    {
        cursorSlot.SetCoreClear();
        cursorSlot.SetIconImageEnable(false);
        startSlot = null;
    }

    private Slot RaycastSlot(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        foreach (RaycastResult result in results)
        {
            Slot slot = result.gameObject.GetComponent<Slot>();
            if (slot != null)
                return slot;
        }

        return null;
    }

    #endregion

    public void OnDrop(PointerEventData eventData)
    {
    }
}