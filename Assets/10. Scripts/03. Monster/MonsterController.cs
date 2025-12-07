using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterController : MonoBehaviour
{
    private static readonly int IS_MOVE = Animator.StringToHash("IsMove");
    private static readonly int ATTACK = Animator.StringToHash("Attack");
    
    [SerializeField] private float moveSpeed;
    [SerializeField] private float traceRange = 10.0f;
    [SerializeField] private float attackRange = 3.0f;
    [SerializeField] private float baseUpdateTerm = 0.1f;

    private Animator animator;

    [SerializeField] private Transform patrolPoint;

    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 goalPoint;

    private Vector3 originalScale;

    protected IEnumerator NextStateCoroutine;
    
    protected void Awake()
    {
        animator = GetComponent<Animator>();
        originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        StartCoroutine(FiniteStateMachineCoroutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator FiniteStateMachineCoroutine()
    {
        NextStateCoroutine = IdleStateCoroutine();

        while (gameObject.activeInHierarchy)
            yield return StartCoroutine(NextStateCoroutine);
    }
    
    // ==============================================
    // FSM Coroutine
    // ==============================================

    protected bool IsInRange(Vector2 poiont, float range) => Vector2.Distance(poiont, pointA) <= range;

    protected IEnumerator IdleStateCoroutine()
    {
        float waitTime = 0.0f;
        const float IDLE_WAIT_TIME = 3.0f;

        WaitForSeconds waitForBaseTerm = new WaitForSeconds(baseUpdateTerm);

        while (true)
        {
            if (IDLE_WAIT_TIME > waitTime)
            {
                NextStateCoroutine = PatrolStateCoroutine();
                yield break;
            }

            if (IsInRange(Player.LocalPlayer.transform.position, traceRange))
            {
                NextStateCoroutine = AttackStateCoroutine();
                yield break;
            }
            
            yield return waitForBaseTerm;
            waitTime += baseUpdateTerm;
        }
    }

    private void Start()
    {
        pointA = transform.position;
        pointB = patrolPoint.position;
        goalPoint = pointB;
    }

    protected IEnumerator PatrolStateCoroutine()
    {
        float waitTime = 0.0f;
        animator.SetBool(IS_MOVE, true);

        while (true)
        {
            if (IsInRange(Player.LocalPlayer.transform.position, traceRange))
            {
                NextStateCoroutine = AttackStateCoroutine();
                animator.SetBool(IS_MOVE, false);
                yield break;
            }

            if (IsInRange(goalPoint, 0.1f))
            {
                goalPoint = IsInRange(pointB, 0.1f) ? pointA : pointB;
                NextStateCoroutine = IdleStateCoroutine();
                animator.SetBool(IS_MOVE, false);
                yield break;
            }

            Move(goalPoint);
            yield return null;
            waitTime += Time.deltaTime;
        }
    }

    protected IEnumerator AttackStateCoroutine()
    {
        float waitTime = 0.0f;

        while (true)
        {
            if (IsInRange(Player.LocalPlayer.transform.position, attackRange))
            {
                animator.SetBool(IS_MOVE, false);
                animator.SetTrigger(ATTACK);
                
                yield return new WaitForSeconds(2.0f);
            }
        }

        Move(Player.LocalPlayer.transform.position);
        yield return null;
        waitTime += Time.deltaTime;
    }

    protected IEnumerator DeadStateCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
    }

    private void Move(Vector3 goalPosition)
    {
        animator.SetBool(IS_MOVE, true);

        Vector2 direction = (goalPosition - transform.position).normalized;
        transform.Translate(direction * (moveSpeed * Time.deltaTime));
        transform.localScale = direction * originalScale;
    }
}
