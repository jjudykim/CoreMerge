using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Color = System.Drawing.Color;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private RectTransform canvas;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    //[SerializeField] private TextMeshProUGUI countText;

    public event Action<Slot> OnClicked;
    
    public CoreData Core { get; protected set; }
    public int Count { get; protected set; }
    public int Index { get; set; }

    public bool IsEmptySlot { get { return Core == null; } }
    public void SetIconImageEnable(bool enable) => iconImage.enabled = enable;
    
    private RectTransform rectTransform;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetCoreClear()
    {
        Clear();
    }

    public void SetItem(int coreID, int count = 1)
    {
        Core = Managers.Instance.CoreDB.GetCoreDataOrNull(coreID);
        backgroundImage.enabled = true;
        iconImage.enabled = true;

        if (Core == null)
        {
            Debug.LogWarning("Core not found: " + coreID);
            Clear();
            return;
        }

        Count = 1;

        if (iconImage != null)
            iconImage.sprite = Core.icon;

        //if (countText != null)
        //    countText.text = string.Empty;
    }

    public void Clear()
    {
        Core = null;
        Count = 0;

        backgroundImage.enabled = false;
        iconImage.enabled = false;
        
        if (iconImage != null)
            iconImage.sprite = null;

        //if (countText != null)
        //    countText.text = string.Empty;
    }
    
    public void SetPosition(Vector2 inputPosition)
    {
        if (rectTransform == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            inputPosition,
            null,                     
            out var localPos);

        rectTransform.anchoredPosition = localPos;
    }

    public void SetAlpha(float alpha)
    {
        backgroundImage.color = new UnityEngine.Color(1, 1, 1, alpha);
        iconImage.color = new UnityEngine.Color(1, 1, 1, alpha);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClicked != null) 
            OnClicked.Invoke(this);
    }
}
