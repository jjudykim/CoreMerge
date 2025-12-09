using UnityEngine;

public class Core : MonoBehaviour
{
    private static readonly int TIER = Animator.StringToHash("Tier");
    
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private int coreId;

    public void Init(int coreId)
    {
        this.coreId = coreId;

        CoreData data = Managers.Instance.CoreDB.GetCoreDataOrNull(coreId);
        if (data == null)
        {
            Debug.Log($"Core ::: 유효하지 않은 CoreID : {coreId}");
            return;
        }

        spriteRenderer.sprite = data.icon;

        if (animator != null)
        {
            animator.SetInteger(TIER, (int)data.tier);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // TODO : Inventory Manager 추가 후 주석 해제
            //Managers.Instance.Inventory.AddCore(coreId);
            
            Destroy(gameObject);
        }
    }
}
