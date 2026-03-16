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
    private static readonly int HURT = Animator.StringToHash("Hurt");
    private static readonly int DEAD = Animator.StringToHash("Dead");
    private static readonly int FORWARD = Animator.StringToHash("Forward");
    private static readonly int PH1ATTACK = Animator.StringToHash("Ph1Attack");
    private static readonly int PH1RANGE = Animator.StringToHash("Ph1Range");
    private static readonly int PHASECHANGE_DESPAWN = Animator.StringToHash("PhaseChange_Despawn");
    private static readonly int PHASECHANGE_SPAWN = Animator.StringToHash("PhaseChange_Spawn");
    private static readonly int PH2IDLE = Animator.StringToHash("Ph2Idle");
    private static readonly int PH2ATTACK = Animator.StringToHash("Ph2Attack");

    public int AnimKeyIdle => IDLE;
    public int AnimKeyHurt => HURT;
    public int AnimKeyDead => DEAD;
    public int AnimKeyForward => FORWARD;
    public int AnimKeyPh1Attack => PH1ATTACK;
    public int AnimKeyPh1Range => PH1RANGE;
    public int AnimKeyPhaseChange1 => PHASECHANGE_DESPAWN;
    public int AnimKeyPhaseChange2 => PHASECHANGE_SPAWN;
    public int AnimKeyPh2Idle => PH2IDLE;
    public int AnimKeyPh2Attack => PH2ATTACK;
    #endregion
    
    public Boss Boss { get; private set; }
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
    public float DashTelegraphTime => dashTelegraphTime;
    public float DashSpeed => dashSpeed;
    public Vector2 DashOffset => dashOffset;
    public float AttackDuration => attackDuration;
    public float ReturnDuration => returnDuration;
    
    [Header("Phase1 Range Settings")]
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private BossTrackingProjectile projectilePrefab;
    [SerializeField] private int rangeProjectileCount = 5;
    [SerializeField] private float rangeFireInterval = 0.08f;
    [SerializeField] private float rangeProjectileSpeed = 18f;
    [SerializeField] private float rangeTurnRate = 720f;
    [SerializeField] private float rangeLifeTime = 3.0f;
    [SerializeField] private float rangeAfterDelay = 0.2f;
    
    public Transform ProjectileSpawnPoint => projectileSpawnPoint;
    public BossTrackingProjectile ProjectilePrefab => projectilePrefab;
    public int RangeProjectileCount => rangeProjectileCount;
    public float RangeFireInterval => rangeFireInterval;
    public float RangeProjectileSpeed => rangeProjectileSpeed;
    public float RangeTurnRate => rangeTurnRate;
    public float RangeLifeTime => rangeLifeTime;
    public float RangeAfterDelay => rangeAfterDelay;


    [Header("Phase2 Hand settings")] 
    [SerializeField] private BossHandProjectile handPrefab;
    [SerializeField] private int minHandCount = 3;
    [SerializeField] private int maxHandCount = 5;
    [SerializeField] private float spawnInterval = 0.8f;
    [SerializeField] private float spawnY = 10f;
    [SerializeField] private float spawnMinX = -10f;
    [SerializeField] private float spawnMaxX = 10f;

    public BossHandProjectile HandPrefab => handPrefab;
    public int MinHandCount => minHandCount;
    public int MaxHandCount => maxHandCount;
    public float SpawnInterval => spawnInterval;
    public float SpawnY => spawnY;
    public float SpawnMinX => spawnMinX;
    public float SpawnMaxX => spawnMaxX;
    
    // ---------------------------------------------------------
    // States
    // ---------------------------------------------------------
    public BossIdleState IdleState { get; private set; }
    public BossPhase1DashAttackState Ph1DashAttackState { get; private set; }
    public BossPhase1RangeState Ph1RangeState { get; private set; }
    public BossPhaseTransitionState PhaseTransitionState  { get; private set; }
    public BossPhase2IdleState Ph2IdleState { get; private set; }
    public BossPhase2RangeState Ph2RangeState { get; private set; }
    public BossPhase2HandState Ph2HandState { get; private set; }
    public BossHurtState HurtState { get; private set; }
    public BossDeadState DeadState { get; private set; }

    public void Hurt() => StateMachine.ChangeState(HurtState);
    public void Die() => StateMachine.ChangeState(DeadState);
    
    private void Awake()
    {
        Boss = GetComponent<Boss>();
        StateMachine = new BossStateMachine();
        OriginPosition = transform.position;

        if (playerTarget == null && Player.LocalPlayer != null)
            playerTarget = Player.LocalPlayer.transform;
        
        // Create State
        IdleState = new BossIdleState(this, StateMachine);
        Ph1DashAttackState = new BossPhase1DashAttackState(this, StateMachine);
        Ph1RangeState = new BossPhase1RangeState(this, StateMachine);
        PhaseTransitionState  = new BossPhaseTransitionState(this, StateMachine);
        Ph2IdleState = new BossPhase2IdleState(this, StateMachine);
        Ph2RangeState = new BossPhase2RangeState(this, StateMachine);
        Ph2HandState = new BossPhase2HandState(this, StateMachine);
        HurtState = new BossHurtState(this, StateMachine);
        DeadState = new BossDeadState(this, StateMachine);
    }
    
    private void Start()
    {
        StateMachine.Init(IdleState);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
            StateMachine.ChangeState(PhaseTransitionState);
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

    // AnimEvent
    public void OnAnimEvent_Ph1Range_FireStart()
    {
        if (StateMachine.CurrentState == Ph1RangeState)
            Ph1RangeState.OnAnimEvent_FireStart();
    }

    public void OnAnimEvent_Ph1Range_FireEnd()
    {
        if (StateMachine.CurrentState == Ph1RangeState)
            Ph1RangeState.OnAnimEvent_FireEnd();
    }

    public void OnAnimEvent_HideStart()
    {
        if (StateMachine.CurrentState == PhaseTransitionState)
            PhaseTransitionState.OnAnimEvent_HideStart();
    }

    public void OnAnimEvent_HideEnd()
    {
        if (StateMachine.CurrentState == PhaseTransitionState)
            PhaseTransitionState.OnAnimEvent_HideEnd();
    }
}
