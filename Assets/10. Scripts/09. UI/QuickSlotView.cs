using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class QuickSlotView : InventoryViewBase
{
    [Header("QuickSlot Settings")] 
    [SerializeField] private int quickSlotSize = 3;
    
    [Header("Prefabs & Parents")]
    [SerializeField] private Slot slotPrefab;
    [SerializeField] private Transform slotParent;

    public Slot[] Slots { get; private set; }

    private InventoryQuickSlotController quickSlotCtrl;

    private void Awake()
    {
        container = Managers.Instance.QuickSlots;

        if (quickSlotCtrl == null)
            quickSlotCtrl = GetComponent<InventoryQuickSlotController>();
        CreateSlots();
    }
    
    private void OnEnable()
    {
        if (Managers.Instance.QuickSlots != null)
            Managers.Instance.QuickSlots.OnChanged += Refresh;
    }

    private void OnDisable()
    {
        if (Managers.Instance.QuickSlots != null)
            Managers.Instance.QuickSlots.OnChanged -= Refresh;
    }

    private void CreateSlots()
    {
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        Slots = new Slot[quickSlotSize];

        for (int i = 0; i < quickSlotSize; i++)
        {
            Slot slot = Instantiate(slotPrefab, slotParent);
            slot.Index = i;
            slot.Clear();

            int index = i;
            slot.OnClicked += (_ => quickSlotCtrl.UnequipQuickSlot(index));

            Slots[i] = slot;
        }
    }
    
    public override void Refresh()
    {
        if (container == null || Slots == null)
            return;

        foreach (var slot in Slots)
            slot.Clear();

        var allSlots = container.GetAllSlots();

        foreach (var slotData in allSlots)
        {
            int index = slotData.Index;
            if (index < 0 || Slots.Length <= index)
                continue;
            
            Slot slot = Slots[index];
            slot.SetItem(slotData.ItemId, slotData.Count);
        }
    }

    public Slot GetSlot(int index)
    {
        if (index < 0 || Slots.Length <= index)
            return null;
        
        return Slots[index];
    }
    
    public bool TryGetIndexOfSlot(Slot slot, out int index)
    {
        index = -1;
        if (slot == null || Slots == null)
            return false;

        for (int i = 0; i < Slots.Length; ++i)
        {
            if (Slots[i] == slot)
            {
                index = i;
                return true;
            }
        }

        return false;
    }
}