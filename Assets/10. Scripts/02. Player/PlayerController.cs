using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    #region Animator Key String
    // Controller에서 갱신
    private static readonly int SPEED = Animator.StringToHash("Speed");
    private static readonly int VELOCITY = Animator.StringToHash("Velocity");
    private static readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");
    private static readonly int IS_ONLADDER = Animator.StringToHash("IsOnLadder");
    
    // State에서 갱신
    private static readonly int TRIGGER_IDLE = Animator.StringToHash("Idle");
    private static readonly int TRIGGER_RUN = Animator.StringToHash("Run");
    private static readonly int TRIGGER_JUMP = Animator.StringToHash("Jump");
    private static readonly int TRIGGER_ATTACK = Animator.StringToHash("Attack");
    private static readonly int TRIGGER_DASH = Animator.StringToHash("Dash");
    private static readonly int TRIGGER_DASHATTACK = Animator.StringToHash("DashAttack");
    private static readonly int TRIGGER_SLIDE = Animator.StringToHash("Slide");
    private static readonly int TRIGGER_HURT = Animator.StringToHash("Hurt");
    private static readonly int TRIGGER_DEAD = Animator.StringToHash("Dead");
    
    public int AnimKeySpeed => SPEED;
    public int AnimKeyVelocity => VELOCITY;
    public int AnimKeyIsGround => IS_GROUNDED;
    public int AnimKeyIsOnLadder => IS_ONLADDER;
    public int AnimKeyIdle => TRIGGER_IDLE;
    public int AnimKeyRun => TRIGGER_RUN;
    public int AnimKeyJump => TRIGGER_JUMP;
    public int AnimKeyAttack => TRIGGER_ATTACK;
    public int AnimKeyDash => TRIGGER_DASH;
    public int AnimKeyDashAttack => TRIGGER_DASHATTACK;
    public int AnimKeySlide => TRIGGER_SLIDE;
    public int AnimKeyHurt => TRIGGER_HURT;
    public int AnimKeyDead => TRIGGER_DEAD;
    #endregion
    
    // Manager
    private InputManager input => Managers.Instance.Input;
    
    // State Machine
    [field: SerializeField] public PlayerStateMachine StateMachine { get; private set; }
    
    // ---------------------------------------------------------
    // Components
    // ---------------------------------------------------------
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D collider;
    [SerializeField] private Rigidbody2D rigidbody;
    public Animator Animator { get; set; }
    public SpriteRenderer SpriteRenderer { get; set; }
    public Collider2D Collider { get; set; }
    public Rigidbody2D Rigidbody { get; set; }


    // ---------------------------------------------------------
    // Basic Movement Settings
    // ---------------------------------------------------------
    [Header("Basic Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int facingDir = 1;
    [SerializeField] private Collider2D groundCheckerCollider;
    [SerializeField] private LayerMask groundLayer;
    public float MoveSpeed => moveSpeed;
    public float JumpForce => jumpForce;
    public int FacingDir { get => facingDir; set => facingDir = value; }

    #region Movement Settings
    // ---------------------------------------------------------
    // Dash Settings
    // ---------------------------------------------------------
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeedMultiplier = 2.5f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashAttackDuration = 1f;
    public float DashSpeedMultiplier => dashSpeedMultiplier;
    public float DashDuration { get => dashDuration; set => dashDuration = value; }
    public float DashAttackDuration => dashAttackDuration;
    
    // ---------------------------------------------------------
    // Slide Settings
    // ---------------------------------------------------------
    [Header("Slide Settings")]
    [SerializeField] private float slideSpeedMultiplier = 1.5f;
    [SerializeField] private float slideDuration = 0.2f;
    public float SlideDuration { get => slideDuration; set => slideDuration = value; }
    public float SlideSpeedMultiplier => slideSpeedMultiplier;
    
    // ---------------------------------------------------------
    // Climb Settings
    // ---------------------------------------------------------
    [Header("Climb Settings")]
    [SerializeField] private float climbSpeed = 3f;
    [SerializeField] private Collider2D ladderCheckerCollider;
    [SerializeField] private LayerMask ladderLayer;
     
    public float ClimbSpeed {  get => climbSpeed; set => climbSpeed = value; }
    public Collider2D CurrentLadder { get; private set; }
    #endregion
    
    #region Damage Settings
    // ---------------------------------------------------------
    // HitBox Settings
    // ---------------------------------------------------------
    [Header("HitBox Settings")] 
    [SerializeField] private GameObject[] hitBoxes;
    private PlayerHitBox[] hitBoxesComponent;
    private Collider2D[] hitBoxesCollider;
    
    // ---------------------------------------------------------
    // Damage, Hurt Settings
    // ---------------------------------------------------------
    [Header("Damage / Hurt Settings")]
    [SerializeField] private float damageInterval = 0.7f;  // 두 대 맞는 최소 간격(초)
    [SerializeField] private float hurtInvincibleTime = 0.3f; // 피격 후 잠시 무적시간
    [SerializeField] private float hurtDuration = 0.4f; // 피격 후 잠시 무적시간
    [SerializeField] private bool ignoreDamageWhileAttacking = true; // 공격 중 피격 무시 여부
    private float lastDamagedTime = 0f;
    public bool IsInvincible { get; private set; }
    
    public bool IsInAttackState()
    {
        var s = StateMachine.CurrentState;
        return s == AttackState || s == DashAttackState || s == SlideAttackState || s == SkillAttackState;
    }

    public bool CanReceiveDamage()
    {
        if(IsDead)
            return false;

        if (IsInvincible)
            return false;

        if (ignoreDamageWhileAttacking && IsInAttackState())
            return false;

        if (Time.time < lastDamagedTime + damageInterval)
            return false;

        return true;
    }

    public float HurtDurtaion => hurtDuration;
    #endregion
    
    // ---------------------------------------------------------
    // States
    // ---------------------------------------------------------
    public IdleState IdleState { get; set; }
    public RunState RunState { get; set; }
    public DashState DashState { get; set; }
    public JumpState JumpState { get; set; }
    public ClimbState ClimbState { get; set; }
    public AttackState AttackState { get; set; }
    public SlideAttackState SlideAttackState { get; set; }
    public DashAttackState DashAttackState { get; set; }
    public SkillAttackState SkillAttackState { get; set; }
    public HurtState HurtState { get; set; }
    public DeadState DeadState { get; set; }
    
    // Status Check
    public bool IsGrounded { get; private set; }
    public bool IsOnLadder { get; private set; }
    public bool IsHurt { get; set; }
    public bool IsDead { get; set; }
    
    // Input Cache
    public float InputX             => input.MoveX;
    public float InputY             => input.MoveY;
    public bool JumpPressed         => input.JumpDown;
    public bool AttackPressed       => input.AttackDown;
    public bool SlideAttackPressed  => input.SlideAttackDown;
    public bool DashRightDown       => input.DashRightDown;
    public bool DashLeftDown        => input.DashLeftDown;
    
    // Etc
    private Vector2 baseColliderOffset;
    private Vector2 ladderCheckColliderOffset; 
    
    private void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Collider = GetComponentInChildren<Collider2D>();
        Rigidbody = GetComponent<Rigidbody2D>();

        StateMachine = new PlayerStateMachine();

        baseColliderOffset = Collider.offset;
        ladderCheckColliderOffset = ladderCheckerCollider.offset;

        hitBoxesComponent = new PlayerHitBox[hitBoxes.Length];
        hitBoxesCollider = new Collider2D[hitBoxes.Length];
        for (int i = 0; i < hitBoxes.Length; ++i)
        {
            hitBoxesComponent[i] = hitBoxes[i].GetComponent<PlayerHitBox>();
            hitBoxesComponent[i].Owner = this;
            hitBoxesCollider[i] = hitBoxes[i].GetComponent<Collider2D>();
        }

        #region States Instance Create
        IdleState = new IdleState(this, StateMachine);
        RunState = new RunState(this, StateMachine);
        DashState = new DashState(this, StateMachine);
        JumpState = new JumpState(this, StateMachine);
        ClimbState = new ClimbState(this, StateMachine);
        AttackState = new AttackState(this, StateMachine);
        SlideAttackState = new SlideAttackState(this, StateMachine);
        DashAttackState = new DashAttackState(this, StateMachine);
        SkillAttackState = new SkillAttackState(this, StateMachine);
        HurtState = new HurtState(this, StateMachine);
        DeadState = new DeadState(this, StateMachine);
        #endregion
    }

    private void Start()
    {
        StateMachine.Init(IdleState);
    }

    void Update()
    {
        UpdateFacing();
        StateMachine.CurrentState.UpdateLogic();
        UpdateAnimationParams();
        CheckIfOnLadder();
        CheckIfGrounded();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.UpdatePhysics();
    }

    private void UpdateAnimationParams()
    {
        if (StateMachine.CurrentState != JumpState)
            Animator.ResetTrigger((AnimKeyJump));

        Animator.SetFloat(SPEED, Mathf.Abs(Rigidbody.linearVelocityX));
        Animator.SetFloat(VELOCITY, Rigidbody.linearVelocityY);
        Animator.SetBool(IS_GROUNDED, IsGrounded);
        Animator.SetBool(IS_ONLADDER, IsOnLadder);
    }
    
    public void UpdateFacing()
    {
        if (StateMachine.CurrentState == ClimbState)
            return;
        
        float inputX = InputX;
        
        if (Mathf.Abs(inputX) > 0.01f)
        {
            FacingDir = inputX < 0 ? -1 : 1;

            SpriteRenderer.flipX = (FacingDir == -1);

            FlipColliderOffset(baseColliderOffset, Collider);
            FlipColliderOffset(ladderCheckColliderOffset, ladderCheckerCollider);
            for (int i = 0; i < hitBoxesCollider.Length; ++i)
            {
                FlipColliderOffset(hitBoxesCollider[i].offset, hitBoxesCollider[i]);
            }
        }
    }

    public void FlipColliderOffset(Vector2 originOffset, Collider2D collider)
    {
        Vector2 newColOffset = originOffset;
        newColOffset.x = Mathf.Abs(originOffset.x) * FacingDir * -1;
        collider.offset = newColOffset;
    }

    private void CheckIfOnLadder()
    {
        BoxCollider2D col = ladderCheckerCollider as BoxCollider2D;
        Vector2 center = col.bounds.center;
        Vector2 size = col.bounds.size;
        
        Collider2D hit = Physics2D.OverlapBox(center, size, 0f, ladderLayer);
        IsOnLadder = hit != null;
        CurrentLadder = hit;
        // Debug.Log($"CheckIfOnLadder ::: IsOnLadder : {IsOnLadder}");
    }

    private void CheckIfGrounded()
    {
        CircleCollider2D col = groundCheckerCollider as CircleCollider2D;
        Vector2 center = col.bounds.center;
        float size = col.radius;

        IsGrounded = Physics2D.OverlapCircle(center, size, groundLayer);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        
        if (ladderCheckerCollider is BoxCollider2D col)
        {
            Gizmos.color = Color.cyan;
            Vector2 pos = col.bounds.center;
            Vector2 size = col.bounds.size;
            Gizmos.DrawWireCube(pos, size);
        }
    }
    
    public void OnBodyHit(Collider2D other)
    {
        if (!CanReceiveDamage())
            return;

        lastDamagedTime = Time.time;
        Debug.Log("OnTriggerEnter2D ::: Player Damaged!!!");
        
        // TODO : 여기서 데미지 계산 후 Hp 감소
        if (IsDead)
            return;
        
        StateMachine.ChangeState(HurtState);

        if (hurtInvincibleTime > 0f)
        {
            StartCoroutine(CoSetInvincible(hurtInvincibleTime));
        }
    }

    private IEnumerator CoSetInvincible(float duration)
    {
        IsInvincible = true;
        yield return new WaitForSeconds(duration);
        IsInvincible = false;
    }

    // TODO : Attack Info나 Stat 넣고나서 데미지 적용하기
    public void OnHitBoxTriggered(PlayerHitBox playerHitBox, Collider2D other)
    {
        switch (playerHitBox.HitBoxType)
        {
            case HitBoxType.DashAttack:
                Debug.Log("OnHitBoxTriggered ::: DashAttack Done!!!");
                break;
            case HitBoxType.SlideAttack:
                Debug.Log("OnHitBoxTriggered ::: SlideAttack Done!!!");
                break;
            case HitBoxType.NormalAttack:
                Debug.Log("OnHitBoxTriggered ::: Attack Done!!!");
                break;
        }
    }

    public void Die()
    {
        if (IsDead)
            return;
        
        IsDead = true;
        StateMachine.ChangeState(DeadState);
    }
}