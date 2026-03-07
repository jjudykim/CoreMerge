using UnityEngine;

public class MonsterHitBox : MonoBehaviour
{
    [Header("Owner")] 
    [SerializeField] private Monster owner;

    [Header("Damage Settings")] 
    [SerializeField] private bool useOwnerAttackStat = true;
    [SerializeField] private int fixedDamage = 1;

    [Header("Runtime (Read-only)")] 
    [SerializeField] private bool startDisabled = true;
    [SerializeField] private Collider2D hitCollider; 
    
    
    private void Awake()
    {
        if (owner == null)
            owner = GetComponentInParent<Monster>();
        
        if (hitCollider = null)
            hitCollider = GetComponent<Collider2D>();

        if (startDisabled)
            DisableHitBox();
    }

    private void OnEnable()
    {
        if (startDisabled)
            DisableHitBox();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false)
            return;
        
        int damage = fixedDamage;

        if (useOwnerAttackStat && owner != null)
            damage = owner.Attack;

        CombatEvent ev = new CombatEvent
        {
            Type = EventType.DamageEvent,
            Amount = damage,
            Position = other.transform.position
        };
        
        CombatSystem.Instance.ToPlayer(ev);
    }

    public void SetOwner(Monster monster)
    {
        owner = monster;
    }

    protected internal void EnableHitBox()
    {
        if (hitCollider != null) 
            hitCollider.enabled = true;
    }

    protected internal void DisableHitBox()
    {
        if (hitCollider != null)
            hitCollider.enabled = false;
    }

    public void ForceDisableHitBox()
    {
        if (hitCollider != null)
            DisableHitBox();
    }
}
