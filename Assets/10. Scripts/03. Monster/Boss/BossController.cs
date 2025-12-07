using UnityEngine;

public enum BossPhase
{
    Phase1,
    Phase2,
    Dead
}

public class BossController : MonoBehaviour
{
    #region Animation Key String
    private static readonly int IDLE = Animator.StringToHash("Idle");
    private static readonly int FORWARD = Animator.StringToHash("Forward");
    private static readonly int ATTACK = Animator.StringToHash("Attack");

    public int AnimKeyIdle => IDLE;
    public int AnimKeyForward => FORWARD;
    public int AnimKeyAttack => ATTACK;
    #endregion
    
    public BossStateMachine StateMachine { get; private set; }
    public BossPhase CurrentPhase { get; private set; } = BossPhase.Phase1;

    public Vector2 OriginPosition { get; private set; }

    // ---------------------------------------------------------
    // Components
    // ---------------------------------------------------------
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D collider;
    public Animator Animator => animator;
    public SpriteRenderer SpriteRenderer => spriteRenderer;
    public Collider2D Collider => collider;
    
    
    // ---------------------------------------------------------
    // Pattern Settings
    // ---------------------------------------------------------
    [Header("Pattern Settings  ──────────────")]
    [Space(5)]
    
    [Header("Target")]
    public Transform playerTarget;
    public Vector2 PlayerPosition => playerTarget.position;
    
    [Header("Phase1 Dash Settings")]
    [SerializeField] private float dashTelegraphTime = 2f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private Vector2 dashOffset;
    [SerializeField] private float attackDuration = 0.5f;
    [SerializeField] private float returnDuration = 3f;
    [SerializeField] private float projectileCooldown;
    [SerializeField] private float handPatternCooldown;
    
    public float DashTelegraphTime => dashTelegraphTime;
    public float DashSpeed => dashSpeed;
    public Vector2 DashOffset => dashOffset;
    public float AttackDuration => attackDuration;
    public float ReturnDuration => returnDuration;
    public float ProjectileCooldown => projectileCooldown;
    public float HandPatternCooldown => handPatternCooldown;
    
    
    
    // ---------------------------------------------------------
    // States
    // ---------------------------------------------------------
    public BossIdleState IdleState { get; private set; }
    public BossPhase1DashAttackState P1DashAttackState { get; private set; }
    public BossPhase1RangeState P1RangeState { get; private set; }
    public BossPhaseTransitionState PhaseTransitionState  { get; private set; }
    public BossPhase2IdleState P2IdleState { get; private set; }
    public BossPhase2RangeState P2RangeState { get; private set; }
    public BossPhase2HandState P2HandState { get; private set; }
    public BossHurtState HurtState { get; private set; }
    public BossDeadState DeadState { get; private set; }

    public void Hurt() => StateMachine.ChangeState(HurtState);
    public void Die() => StateMachine.ChangeState(DeadState);
    
    private void Awake()
    {
        StateMachine = new BossStateMachine();
        
        OriginPosition = transform.position;
        
        // Create State
        IdleState = new BossIdleState(this, StateMachine);
        P1DashAttackState = new BossPhase1DashAttackState(this, StateMachine);
        P1RangeState = new BossPhase1RangeState(this, StateMachine);
        PhaseTransitionState  = new BossPhaseTransitionState(this, StateMachine);
        P2IdleState = new BossPhase2IdleState(this, StateMachine);
        P2RangeState = new BossPhase2RangeState(this, StateMachine);
        P2HandState = new BossPhase2HandState(this, StateMachine);
        HurtState = new BossHurtState(this, StateMachine);
        DeadState = new BossDeadState(this, StateMachine);
    }
    
    private void Start()
    {
        StateMachine.Init(IdleState);
    }

    private void Update()
    {
        StateMachine.CurrentState.UpdateLogic();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.UpdatePhysics();
    }

    public void ChangePhase(BossPhase newPhase)
    {
        CurrentPhase = newPhase;
    }
    
    
    
    
}
