using System.Collections;
using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [Header("Inventory Panel Root")] 
    [SerializeField] private RectTransform inventoryRoot;
    
    [Header("Inventory View / Input Handler")]
    [SerializeField] private UserInventoryView inventoryView;
    [SerializeField] private UIInputHandler uiInputHandler;

    [Header("Slide Settings")] 
    [SerializeField] private float slideDuration = 0.25f;

    [SerializeField] private Vector2 shownPosition = Vector2.zero;
    [SerializeField] private Vector2 hiddenPosition = new Vector2(600f, 0f);

    private bool isOpen = false;
    private Coroutine coSlide;

    private void Awake()
    {
        if (inventoryRoot != null)
            inventoryRoot.anchoredPosition = hiddenPosition;

        isOpen = false;
    }

    private void Update()
    {
        if (Managers.Instance.Input.InventoryToggleDown)
        {
            ToggleInventory();
        }
    }

    public void OnClickToggleInventory()
    {
        ToggleInventory();
    }

    public void OnClickMergeButton()
    {
        bool merged = Managers.Instance.CoreInventory.TryMergeAll();
        
        if (merged == false)
            Debug.Log("Merge ::: 머지 가능한 코어 조합이 없음");
    }

    private void ToggleInventory()
    {
        isOpen = !isOpen;

        if (uiInputHandler != null)
            uiInputHandler.enabled = isOpen;
        
        if (coSlide != null)
            StopCoroutine(coSlide);

        coSlide = StartCoroutine(SlideCoroutine(isOpen));
    }

    private IEnumerator SlideCoroutine(bool show)
    {
        if (inventoryRoot == null)
            yield break;

        Vector2 start = inventoryRoot.anchoredPosition;
        Vector2 target = show ? shownPosition :  hiddenPosition;

        float elapsed = 0f;
        
        while (elapsed < slideDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / slideDuration);
            
            inventoryRoot.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        inventoryRoot.anchoredPosition = target;

        coSlide = null;
    }
}