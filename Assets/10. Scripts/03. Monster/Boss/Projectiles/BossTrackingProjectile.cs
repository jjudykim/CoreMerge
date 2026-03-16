using UnityEngine;

public class BossTrackingProjectile : MonoBehaviour
{
    private enum ProjectileState
    {
        Spawn,
        Fly,
        Explode
    }

    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D hitCollider;
    
    [Header("Explode Settings")]
    [SerializeField] private float explodeDestroyDelayFallback = 0.6f;
    
    [Header("Boss")]
    [SerializeField] private Boss boss;
    
    private static readonly int HashSpawn = Animator.StringToHash("Spawn");
    private static readonly int HashFly = Animator.StringToHash("Fly");
    private static readonly int HashExplode = Animator.StringToHash("Explode");

    private ProjectileState state;
    
    private Transform target;
    private float moveSpeed;
    private float turnRateDegPerSec;
    private float lifeTime;

    private float lifeTimer;
    
    private RigidbodyType2D originalBodyType;
    private bool originalSimulated;
    
    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        hitCollider = GetComponent<Collider2D>();
    }

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
        
        if (animator == null)
            animator = GetComponent<Animator>();

        if (hitCollider == null)
            hitCollider = GetComponent<Collider2D>();
        
        originalBodyType = rb.bodyType;
        originalSimulated = rb.simulated;
    }

    public void Init(Transform target, float moveSpeed, float turnRateDegPerSec, float lifeTime, Boss boss)
    {
        this.target = target;
        this.moveSpeed = moveSpeed;
        this.turnRateDegPerSec = turnRateDegPerSec;
        this.lifeTime = lifeTime;
        this.boss = boss;

        lifeTimer = 0f;
        
        RestorePhysicsForMove();

        hitCollider.enabled = true;

        EnterSpawnState();
    }

    private void FixedUpdate()
    {
        if (state == ProjectileState.Explode)
            return;
        
        lifeTimer += Time.fixedDeltaTime;
        if (lifeTimer >= lifeTime)
        {
            EnterExplodeState();
            return;
        }

        if (state == ProjectileState.Spawn)
            return;

        UpdateHomingMove();
    }

    private void UpdateHomingMove()
    {
        if (state == ProjectileState.Explode)
            return;
        
        Vector2 currentDir = rb.linearVelocity.sqrMagnitude > 0.0001f ? rb.linearVelocity.normalized : Vector2.right;
        
        Vector2 desiredDir = GetDesiredDirection();

        float maxRotate = turnRateDegPerSec * Time.fixedDeltaTime;
        Vector2 newDir = RotateTowards(currentDir, desiredDir, maxRotate);

        rb.linearVelocity = newDir * moveSpeed;
    }

    private void EnterSpawnState()
    {
        state = ProjectileState.Spawn;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        
        animator.ResetTrigger(HashExplode);
        animator.ResetTrigger(HashFly);
        animator.SetTrigger(HashSpawn);
    }
    
    public void OnSpawnAnimationFinished()
    {
        if (state == ProjectileState.Explode)
            return;

        EnterFlyState();
    }
    
    private void EnterFlyState()
    {
        state = ProjectileState.Fly;
        
        RestorePhysicsForMove();
        
        animator.SetTrigger(HashFly);
        
        Vector2 dir = GetDesiredDirection();
        rb.linearVelocity = dir * moveSpeed;
    }
    
    private void EnterExplodeState()
    {
        if (state == ProjectileState.Explode)
            return;
        
        state = ProjectileState.Explode;
        
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        rb.simulated = false;
        hitCollider.enabled = false;
        
        animator.ResetTrigger(HashFly);
        animator.SetTrigger(HashExplode);
        
        Invoke(nameof(DestroySelf), explodeDestroyDelayFallback);
    }

    public void OnExplodeAnimationFinished()
    {
        DestroySelf();
    }

    private void DestroySelf()
    {
        CancelInvoke(nameof(DestroySelf));
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (state == ProjectileState.Explode)
            return;
        
        if (other.CompareTag("Player") == false)
            return;
        
        int finalDamage = boss.Stat.Attack;

        CombatEvent ev = new CombatEvent
        {
            Type = EventType.DamageEvent,
            Amount = finalDamage,
            Position = other.transform.position
        };
        
        CombatSystem.Instance.ToPlayer(ev);
        
        Debug.Log($"[BossHitBox] Hit Player :: damage={finalDamage}");
        
        EnterExplodeState();
    }

    private Vector2 GetDesiredDirection()
    {
        Vector2 toTarget = (Vector2)target.position - rb.position;
        if (toTarget.sqrMagnitude < 0.0001f)
            return Vector2.right;

        return toTarget.normalized;
    }
    
    private Vector2 RotateTowards(Vector2 current, Vector2 desired, float maxDegrees)
    {
        float currentAngle = Mathf.Atan2(current.y, current.x) * Mathf.Rad2Deg;
        float desiredAngle = Mathf.Atan2(desired.y, desired.x) * Mathf.Rad2Deg;

        float newAngle = Mathf.MoveTowardsAngle(currentAngle, desiredAngle, maxDegrees);
        float rad = newAngle * Mathf.Deg2Rad;

        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
    }
    
    private void RestorePhysicsForMove()
    {
        rb.simulated = originalSimulated;
        rb.bodyType = originalBodyType;
    }
}