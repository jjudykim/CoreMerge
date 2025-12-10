using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

    private Slot startSlot;
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
        
        isCoreDragging = true;
        
        cursorSlot.SetItem(startSlot.Core.id);
        cursorSlot.SetIconImageEnable(true);

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
                ResetCursor();
            else
            {
                Slot endSlot = RaycastSlot(eventData);

                if (endSlot == null)
                    ResetCursor();
                else
                {
                    if (endSlot.IsEmptySlot)
                        MoveCore(endSlot);
                    else
                        SwapCore(startSlot, endSlot);
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
    #endregion
    
    #region Slot 유틸 (Move / Swap / Raycast)

    private void MoveCore(Slot endSlot)
    {
        endSlot.SetItem(startSlot.Core.id, startSlot.Count);

        startSlot.SetCoreClear();
        ResetCursor();
    }

    private void SwapCore(Slot fromSlot, Slot toSlot)
    {
        int toCoreId = toSlot.Core.id;
        int toCoreCount = Managers.Instance.CoreInventory.GetCount(toCoreId);

        int fromCoreId = cursorSlot.Core.id;
        int fromCoreCount = Managers.Instance.CoreInventory.GetCount(fromCoreId);

        fromSlot.SetItem(toCoreId, toCoreCount);
        toSlot.SetItem(fromCoreId, fromCoreCount);

        ResetCursor();
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