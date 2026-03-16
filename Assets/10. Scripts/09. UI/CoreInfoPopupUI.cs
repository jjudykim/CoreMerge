using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoreInfoPopupUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;

    private Canvas rootCanvas;

    private void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        gameObject.SetActive(false);
    }

    public void Show(CoreData core)
    {
        if (core == null)
            return;

        if (itemIcon != null)
            itemIcon.sprite = core.icon;
        
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
