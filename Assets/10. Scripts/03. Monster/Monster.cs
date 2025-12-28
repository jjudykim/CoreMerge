using System.Collections;
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
    [SerializeField] protected Animator animator;
    
    [Header("ContactDamage")]
    [SerializeField] protected bool contactDamageEnabled = true;
    [SerializeField] protected int contactDamageAmount = 1;

    public MonsterType Type => type;
    public Transform HeadUpPivot => headUpPivot;
    public Collider2D MainCollider => mainCollider;
    public Animator Animator => animator;
    public SpriteRenderer SpriteRenderer { get; private set; }
    public Material InstanceMaterial { get; private set; }
    public int Attack => stat.Attack;

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
        InstanceMaterial = GetComponentInChildren<SpriteRenderer>().material;
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

        if (stat.IsDead())
            OnDeath();
        else
            OnHurt();
    }

    protected virtual void OnHurt()
    {
        if (monsterController != null)
            monsterController.OnHurt();
    }
    
    protected virtual void OnDeath()
    {
        if (monsterController != null)
            monsterController.OnDeath();

        DropCores();
        
        Managers.Instance.Game.OnMonsterDead(this.monsterController);
    }
    
    public virtual void PerformAttack(Vector2 targetPos)
    {
    }

    private void DropCores()
    {
        int minDrop = 2;
        int maxDrop = 4;

        var cores = Managers.Instance.CoreDrop.GetDropCores(type, minDrop, maxDrop);
        if (cores == null || cores.Count == 0)
            return;

        foreach (var core in cores)
        {
            Managers.Instance.Game.SpawnCore(transform.position, core.id);
        }
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
