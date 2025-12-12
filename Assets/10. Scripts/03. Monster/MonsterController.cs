using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

public enum MonsterState
{
    Idle,
    Patrol,
    Chase,
    Attack,
    Hurt,
    Death
}

public class MonsterController : MonoBehaviour
{
    private static readonly int IDLE = Animator.StringToHash("Idle");
    private static readonly int WALK = Animator.StringToHash("Walk");
    private static readonly int CHASE = Animator.StringToHash("Chase");
    private static readonly int ATTACK = Animator.StringToHash("Attack");
    private static readonly int HURT = Animator.StringToHash("Hurt");
    private static readonly int DEATH = Animator.StringToHash("Death");

    private const float NORMAL_ANIM_SPEED = 1.0f;
    
    [Header("Owner / Stat")]
    [SerializeField] private Monster owner;
    public Monster Owner => owner;
    
    [Header("Move Settings")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float traceRange = 10.0f;
    [SerializeField] private float attackRange = 3.0f;
    
    [Header("Patrol Settings")]
    [SerializeField] private Transform patrolPoint;
    [SerializeField] private float idleWaitTime = 2f;
    
    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 3;
    [SerializeField] private float chaseAnimSpeed = 2f;
    
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDuration = 0.6f;
    private float attackCooldownTimer;
    
    [Header("Hurt Settings")]
    [SerializeField] private float hurtDuration = 0.3f;
    [SerializeField] private Rigidbody2D rigidBody;
    
    private const float HURT_EFFECT_VALUE = 0.25f;
    private static readonly int HURT_EFFECT_BLEND = Shader.PropertyToID("_HitEffectBlend");
    
    private Animator animator;
    
    // FSM
    private MonsterState currentState;
    private float stateTimer;
    private float attackTimer;

    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 goalPoint;

    private Vector3 originalScale;

    private bool didAttackThisState;

    private Vector2 PlayerPos2D => Player.LocalPlayer != null 
                                 ? (Vector2)Player.LocalPlayer.transform.position 
                                 : (Vector2)transform.position;
    
    protected void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        originalScale = transform.localScale;

        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody2D>();

        pointA = transform.position;
        if (patrolPoint != null)
            pointB = patrolPoint.position;
        else
            pointB = pointA;

        goalPoint = pointB;
    }

    protected void OnEnable()
    {
        ChangeState(MonsterState.Idle);
    }

    protected void Update()
    {
        stateTimer += Time.deltaTime;

        if (attackCooldownTimer > 0f)
            attackCooldownTimer -= Time.deltaTime;

        switch (currentState)
        {
            case MonsterState.Idle:
                UpdateIdle();
                break;
            case MonsterState.Patrol:
                UpdatePatrol();
                break;
            case MonsterState.Chase:
                UpdateChase();
                break;
            case MonsterState.Attack:
                UpdateAttack();
                break;
            case MonsterState.Hurt:
                UpdateHurt();
                break;
            case MonsterState.Death:
                UpdateDeath();
                break;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void ChangeState(MonsterState newState)
    {
        if (currentState == newState)
            return;
        
        // Exit
        switch (currentState)
        {
            case MonsterState.Chase:
                animator.speed = NORMAL_ANIM_SPEED;
                break;
        }

        currentState = newState;
        stateTimer = 0f;

        ResetAnimTrigger();

        switch (currentState)
        {
            case MonsterState.Idle:
                Debug.Log("Idle State Entry");
                animator.SetTrigger(IDLE);
                break;
            case MonsterState.Patrol:
                Debug.Log("Patrol State Entry");
                animator.SetTrigger(WALK);
                break;
            case MonsterState.Chase:
                Debug.Log("Chase State Entry");
                animator.SetTrigger(CHASE);
                break;
            case MonsterState.Attack:
                Debug.Log("Chase State Attack");
                attackCooldownTimer = attackCooldown;
                animator.SetTrigger(ATTACK);
                didAttackThisState = false;
                break;
            case MonsterState.Hurt:
                animator.SetTrigger(HURT);
                break;
            case MonsterState.Death:
                animator.SetTrigger(DEATH);
                Owner.MainCollider.enabled = false;
                break;
        }
    }

    private void ResetAnimTrigger()
    {
        animator.ResetTrigger(IDLE);
        animator.ResetTrigger(WALK);
        animator.ResetTrigger(CHASE);
        animator.ResetTrigger(ATTACK);
        animator.ResetTrigger(HURT);
        animator.ResetTrigger(DEATH);
    }

    private bool TryGetPlayer(out Vector2 playerPos, out float dist)
    {
        playerPos = PlayerPos2D;
        if (Player.LocalPlayer == null)
        {
            dist = Mathf.Infinity;
            return false;
        }

        dist = Vector2.Distance(playerPos, transform.position);
        return true;
    }

    private void MoveTowardsX(Vector2 targetPos, float speed)
    {
        float dirX = Mathf.Sign(targetPos.x - transform.position.x);
        transform.position += new Vector3(dirX * speed * Time.deltaTime, 0f, 0f);
        
        if (dirX != 0)
            transform.localScale = new Vector3(Mathf.Sign(dirX) * originalScale.x
                                             , originalScale.y, originalScale.z);
    }

    private void UpdateIdle()
    {
        if (TryGetPlayer(out Vector2 playerPos, out float dist))
        {
            if (dist <= attackRange && attackCooldownTimer <= 0f)
            {
                ChangeState(MonsterState.Attack);
                return;
            }

            if (dist <= traceRange)
            {
                ChangeState(MonsterState.Chase);
                return;
            }
        }
        
        if (stateTimer >= idleWaitTime)
            ChangeState(MonsterState.Patrol);
    }
    
    private void UpdatePatrol()
    {
        if (TryGetPlayer(out Vector2 playerPos, out float dist))
        {
            if (dist <= attackRange && attackCooldownTimer <= 0f)
            {
                ChangeState(MonsterState.Attack);
                return;
            }

            if (dist <= traceRange)
            {
                ChangeState(MonsterState.Chase);
                return;
            }
        }

        MoveTowardsX(goalPoint, moveSpeed);

        if (Mathf.Abs(transform.position.x - goalPoint.x) <= 0.1f)
        {
            goalPoint = (goalPoint == pointA) ? pointB : pointA;
            ChangeState(MonsterState.Idle);
        }
    }
    
    private void UpdateChase()
    {
        if (TryGetPlayer(out Vector2 playerPos, out float dist) == false)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        if (dist > traceRange)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        if (dist <= attackRange)
        {
            ChangeState(MonsterState.Attack);
            return;
        }

        MoveTowardsX(playerPos, chaseSpeed);
    }

    private void UpdateAttack()
    {
        if (TryGetPlayer(out Vector2 playerPos, out float dist) == false)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        if (dist > traceRange)
        {
            ChangeState(MonsterState.Idle);
            return;
        }

        if (dist > attackRange)
        {
            ChangeState(MonsterState.Chase);
            return;
        }

        float fireTime = attackDuration * 0.5f;

        if (didAttackThisState == false && stateTimer >= fireTime)
        {
            Owner.PerformAttack(playerPos + new Vector2(0, 0.5f));
            didAttackThisState = true;
        }

        if (stateTimer >= attackDuration)
        {
            if (dist <= traceRange)
                ChangeState(MonsterState.Chase);
            else
                ChangeState(MonsterState.Idle);
        }
    }
    
    private void UpdateHurt()
    {
        if (stateTimer >= hurtDuration)
        {
            if (TryGetPlayer(out _, out float dist))
            {
                if (dist <= attackRange)
                    ChangeState(MonsterState.Attack);
                else if (dist <= traceRange)
                    ChangeState(MonsterState.Chase);
                else
                    ChangeState(MonsterState.Idle);
            }
            else
            {
                ChangeState(MonsterState.Idle);
            }
        }
    }
    
    private void UpdateDeath()
    {
        if (stateTimer >= 1.5f)
            gameObject.SetActive(false);
    }

    public void OnHurt()
    {
        if (currentState == MonsterState.Death)
            return;

        StartCoroutine(HitEffectCoroutine());
        ApplyKnockback();
        ChangeState(MonsterState.Hurt);
    }
    
    public void OnDeath()
    {
        if (currentState == MonsterState.Death)
            return;
        
        ChangeState(MonsterState.Death);
    }

    private void ApplyKnockback()
    {
        if (Player.LocalPlayer == null)
            return;

        float deltaX = transform.position.x - Player.LocalPlayer.transform.position.x;
        int dirX = deltaX >= 0 ? 1 : -1;
    }

    private IEnumerator HitEffectCoroutine()
    {
        float waitTime = 0.0f;

        while (waitTime < hurtDuration)
        {
            float currentBlendValue = Mathf.Lerp(HURT_EFFECT_VALUE, 0.0f, waitTime / hurtDuration);

            owner.InstanceMaterial.SetFloat(HURT_EFFECT_BLEND, currentBlendValue);
            
            yield return null;
            waitTime += Time.deltaTime;
        }

        owner.InstanceMaterial.SetFloat(HURT_EFFECT_BLEND, 0.0f);
    }
}
