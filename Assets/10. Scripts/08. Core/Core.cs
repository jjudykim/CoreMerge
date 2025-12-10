using System.Linq;
using UnityEngine;

public class Core : MonoBehaviour
{
    private static readonly int TIER = Animator.StringToHash("Tier");
    
    [Header("테스트용 TIER 설정")]
    [SerializeField] ECoreTier tier = ECoreTier.Tier1;
    
    [Header("COmponents")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D hitCollider;

    public int CoreId { get; private set; }
    public CoreData Data { get; private set; }

    private void Reset()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
        if (hitCollider == null)
            hitCollider = GetComponent<Collider2D>();
    }

    public void Init(int coreId)
    {
        CoreId = coreId;
        
        CoreData data = Managers.Instance.CoreDB.GetCoreDataOrNull(coreId);
        if (data == null)
        {
            Debug.Log($"Core ::: 유효하지 않은 CoreID : {coreId}");
            Destroy(gameObject);
            return;
        }
        
        spriteRenderer.sprite = data.icon;
        animator.SetInteger(TIER, (int)data.tier);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Managers.Instance.CoreInventory.TryAdd(CoreId, 1);
            
            // Debug용
            int afterCount = Managers.Instance.CoreInventory.GetCount(CoreId);
            Debug.Log($"TryAdd 성공 ::: {CoreId} : {afterCount}");
            
            Destroy(gameObject);
        }
    }
}
