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
    private static readonly int IS_DEAD = Animator.StringToHash("IsDead");
    
    
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
    public int AnimKeyIsDead => IS_DEAD;
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
    [field: SerializeField] 
    public PlayerStateMachine StateMachine { get; private set; }
    
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
    [SerializeField] private Collider2D[] wallCheckerCollider;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private int groundPhysicsLayer = 6;
    public int GroundPhysicsLayer => groundPhysicsLayer;
    
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
    [Tooltip("피격 사이의 최소 간격")]
    [SerializeField] private float hurtDuration = 0.4f;
    [Tooltip("피격 후 잠시 무적 시간")]
    [SerializeField] private float hurtInvincibleTime = 0.3f;
    [Tooltip("공격 중 피격 무시 여부")]
    [SerializeField] private bool ignoreDamageWhileAttacking = true;
    [Tooltip("피격 시 넉백되는 힘")]
    [SerializeField] private float knockbackPower = 7f;
    [Tooltip("피격 시 넉백 중 위로 치솟는 힘")]
    [SerializeField] private float knockbackUpRatio = 7f;
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

        if (Time.time < lastDamagedTime + hurtDuration)
            return false;

        return true;
    }

    public void ApplyDamageGating()
    {
        lastDamagedTime = Time.time;

        if (hurtInvincibleTime > 0f)
            StartCoroutine(CoSetInvincible(hurtInvincibleTime));
    }

    public float HurtDurtaion => hurtDuration;
    public float KnockbackPower => knockbackPower;
    public float KnockbackUpRatio => knockbackUpRatio;

    #endregion
    
    // ---------------------------------------------------------
    // States
    // ---------------------------------------------------------
    public IdleState IdleState { get; private set; }
    public RunState RunState { get; private set; }
    public DashState DashState { get; private set; }
    public JumpState JumpState { get; private set; }
    public ClimbState ClimbState { get; private set; }
    public AttackState AttackState { get; private set; }
    public SlideAttackState SlideAttackState { get; private set; }
    public DashAttackState DashAttackState { get; private set; }
    public SkillAttackState SkillAttackState { get; private set; }
    public HurtState HurtState { get; private set; }
    public DeadState DeadState { get; private set; }
    
    // Status Check
    public bool IsGrounded { get; private set; }
    public bool IsOnLadder { get; private set; }
    public bool IsLadderBelow { get; private set; }
    public bool IsLeftWall { get; private set; }
    public bool IsRightWall { get; private set; }
    public bool IsHurt { get; set; }
    public bool IsDead { get; set; }
    
    // Input Cache
    public float InputX             => input.MoveX;
    public float InputY             => input.MoveY;
    public bool JumpPressed         => input.JumpDown;
    public bool AttackPressed       => input.AttackDown;
    public bool SlideAttackPressed  => input.SlideAttackDown;
    public bool DashDown       => input.DashDown;
    
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
        // Status Check
        CheckIfOnLadder();
        CheckIfLadderBellow();
        CheckIfGrounded();
        CheckIfLeftWall();
        CheckIfRightWall();

        // State Logic
        UpdateFacing();
        StateMachine.CurrentState.UpdateLogic();
        
        // Animation Param
        UpdateAnimationParams();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.UpdatePhysics();
    }

    private void UpdateAnimationParams()
    {
        if (StateMachine.CurrentState != JumpState)
            Animator.ResetTrigger(AnimKeyJump);

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
    }
    
    private void CheckIfLadderBellow()
    {
        CircleCollider2D col = groundCheckerCollider as CircleCollider2D;
        Vector2 center = col.bounds.center;
        float radius = Mathf.Max(col.bounds.extents.x, col.bounds.extents.y);
        
        Collider2D hit = Physics2D.OverlapCircle(center, radius, ladderLayer);
        IsLadderBelow = hit != null;
        CurrentLadder = hit;
    }

    private void CheckIfGrounded()
    {
        CircleCollider2D col = groundCheckerCollider as CircleCollider2D;
        Vector2 center = col.bounds.center;
        float size = col.radius;

        IsGrounded = Physics2D.OverlapCircle(center, size, groundLayer);
    }

    private void CheckIfLeftWall()
    {
        BoxCollider2D leftCol = wallCheckerCollider[0] as BoxCollider2D;

        Vector2 center = leftCol.bounds.center;
        Vector2 size = leftCol.bounds.size;

        IsLeftWall = Physics2D.OverlapBox(center, size, 0f, groundLayer);
    }

    private void CheckIfRightWall()
    {
        BoxCollider2D rightCol = wallCheckerCollider[1] as BoxCollider2D;

        Vector2 center = rightCol.bounds.center;
        Vector2 size = rightCol.bounds.size;

        IsRightWall = Physics2D.OverlapBox(center, size, 0f, groundLayer);
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

    private IEnumerator CoSetInvincible(float duration)
    {
        IsInvincible = true;
        Debug.Log("무적상태");
        yield return new WaitForSeconds(duration);
        Debug.Log("무적해제");
        IsInvincible = false;
    }

    // TODO : Attack Info나 Stat 넣고나서 데미지 적용하기
    public void OnHitBoxTriggered(PlayerHitBox playerHitBox, Collider2D other)
    {
        switch (playerHitBox.HitBoxType)
        {
            case HitBoxType.DashAttack:   Debug.Log("OnHitBoxTriggered ::: DashAttack Done!!!");  break;
            case HitBoxType.SlideAttack:  Debug.Log("OnHitBoxTriggered ::: SlideAttack Done!!!"); break;
            case HitBoxType.NormalAttack: Debug.Log("OnHitBoxTriggered ::: Attack Done!!!"); break;
        }
        
        string attackKey = playerHitBox.HitBoxType.ToString();
        
        Player.LocalPlayer.DamageToEnemy(other, attackKey);
    }

    public void Die()
    {
        if (IsDead)
            return;
        
        IsDead = true;
        StateMachine.ChangeState(DeadState);
    }

    public void Hurt() => StateMachine.ChangeState(HurtState);
}