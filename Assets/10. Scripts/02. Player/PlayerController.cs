using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    // Controller에서 갱신
    private static readonly int SPEED = Animator.StringToHash("Speed");
    private static readonly int VELOCITY = Animator.StringToHash("Velocity");
    private static readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");
    private static readonly int IS_ONLADDER = Animator.StringToHash("IsOnLadder");
    
    // State에서 갱신
    private static readonly int TRIGGER_IDLE = Animator.StringToHash("Idle");
    private static readonly int TRIGGER_RUN = Animator.StringToHash("Run");
    private static readonly int TRIGGER_JUMP = Animator.StringToHash("Jump");
    private static readonly int TRIGGER_DASH = Animator.StringToHash("Dash");
    private static readonly int TRIGGER_SLIDE = Animator.StringToHash("Slide");
    
    
    public int AnimKeySpeed => SPEED;
    public int AnimKeyVelocity => VELOCITY;
    public int AnimKeyIsGround => IS_GROUNDED;
    public int AnimKeyIsOnLadder => IS_ONLADDER;
    public int AnimKeyIdle => TRIGGER_IDLE;
    public int AnimKeyRun => TRIGGER_RUN;
    public int AnimKeyJump => TRIGGER_JUMP;
    public int AnimKeyDash => TRIGGER_DASH;
    public int AnimKeySlide => TRIGGER_SLIDE;
    
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

    // ---------------------------------------------------------
    // Dash Settings
    // ---------------------------------------------------------
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeedMultiplier = 2.5f;
    [SerializeField] private float dashDuration = 0.2f;
    public float DashSpeedMultiplier => dashSpeedMultiplier;
    public float DashDuration { get => dashDuration; set => dashDuration = value; }
    
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
    public bool IsHurt { get; private set; }
    public bool IsDead { get; private set; }
    
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
        }
    }

    public void FlipColliderOffset(Vector2 originOffset, Collider2D collider)
    {
        Vector2 newColOffset = originOffset;
        newColOffset.x = Mathf.Abs(originOffset.x) * FacingDir * -1;
        collider.offset = newColOffset;
    }

    public void Die()
    {
        if (IsDead)
            return;
        IsDead = true;
        
        StateMachine.ChangeState(DeadState);
    }

    // OnCollision
    private void OnCollisionEnter2D(Collision2D other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        //{
        //    IsGrounded = true;
        //}
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        //{
        //    IsGrounded = false;
        //}
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

    // OnTrigger
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
    //        IsOnLadder = true;
    //}
    //
    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    if(other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
    //        IsOnLadder = false;
    //}
}