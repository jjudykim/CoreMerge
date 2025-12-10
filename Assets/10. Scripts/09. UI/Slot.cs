using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

public class Slot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    //[SerializeField] private TextMeshProUGUI countText;
    
    public CoreData Core { get; protected set; }
    public int Count { get; protected set; }

    public int Index { get; set; }

    public bool IsEmptySlot { get { return Core == null; } }
    public void SetIconImageEnable(bool enable) => iconImage.enabled = enable;
    
    private RectTransform rectTransform;

    private void Start()
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
        rectTransform.anchoredPosition = inputPosition - new Vector2(rectTransform.sizeDelta.x / 2, rectTransform.sizeDelta.y / 2);
    }
}
