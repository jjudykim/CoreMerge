using UnityEngine;

public enum MonsterType
{
    NORMAL,
    EPIC,
    BOSS,
    END,
}

public abstract partial class Monster : MonoBehaviour
{
    [Header("Type")]
    [SerializeField] protected MonsterType type;
    
    [Header("Stat")]
    [SerializeField] protected BaseStat stat;
    
    [Header("References")]
    [SerializeField] protected Transform headUpPivot;
    [SerializeField] protected Collider2D mainCollider;
    [SerializeField] protected MonsterController monsterController;
    
    [Header("ContactDamage")]
    [SerializeField] protected bool contactDamageEnabled = true;
    [SerializeField] protected int contactDamageAmount = 1;

    public MonsterType Type => type;
    public Transform HeadUpPivot => headUpPivot;
    public Collider2D MainCollider => mainCollider;

    protected virtual void Awake()
    {
        if (stat == null)
        {
            stat = new BaseStat
            {
                MaxHp = 50,
                Attack = 1,
                Defense = 0
            };

            stat.CurrentHp = stat.MaxHp;
        }
        
        mainCollider = GetComponentInChildren<Collider2D>();
    }

    protected virtual void Start()
    {
        Monster.AddMonster(this);
    }

    private void OnDestroy()
    {
        Monster.RemoveMonster(this);
    }

    public virtual void TakeDamage(int damage)
    {
        int reduced = (int)Mathf.Max(1f, damage - stat.Defense);
        stat.CurrentHp -= reduced;
        
        Debug.Log($"[Monster] TakeDamage :: income={damage}, def={stat.Defense}, " +
                  $"final={reduced}, hp={stat.CurrentHp}/{stat.MaxHp}");

        if (stat.IsDead())
            OnDeath();
        else
            OnHurt();
    }

    protected virtual void OnHurt()
    {
        // monsterController.ChangeState(HurtState);
    }
    
    protected virtual void OnDeath()
    {
        // monsterController.ChangeState(DeadState);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (contactDamageEnabled == false)
            return;
        
        if (other.CompareTag("Player") == false)
            return;

        CombatEvent ev = new CombatEvent
        {
            Type = EventType.DamageEvent,
            Amount = contactDamageAmount,
            Position = Player.LocalPlayer.transform.position
        };

        CombatSystem.Instance.ToPlayer(ev);
    }
}
