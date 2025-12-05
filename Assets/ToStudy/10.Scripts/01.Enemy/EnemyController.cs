using System;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private static readonly int IS_MOVE = Animator.StringToHash("IsMove");

    private static readonly int ATTACK = Animator.StringToHash("Attack");
    // 코루틴을 이용한 FSM 만들기
    // 코루틴의 yield return StartCoroutine(코루틴); 을 이용해서
    // 코루틴들끼리 연결되어 끊임없이 순환하는 구조의
    // 소규모 인공지능 캐릭터를 만들어 봅시다

    //빈 코루틴 필드(빈 객체랑 동일)
    protected IEnumerator NextStateCoroutine;

    private void Awake()
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

    // 메인 코루틴 루프
    private IEnumerator FiniteStateMachineCoroutine()
    {
        NextStateCoroutine = IdleStateCoroutine();

        while (gameObject.activeInHierarchy)
        {
            yield return StartCoroutine(NextStateCoroutine);
        }
    }

    [SerializeField] private float moveSpeed;
    [SerializeField] private float traceRange = 10.0f;
    [SerializeField] private float attackRange = 3.0f;
    [SerializeField] private float baseUpdateTerm = 0.1f; 
    
    private Animator animator; // Awake()에서 GetComponent<Animator>()로 할당 
    
    [SerializeField] private Transform patrolPoint;
    
    private Vector2 pointA;
    private Vector2 pointB;
    private Vector2 goalPoint;

    private Vector3 originalScale;

    protected bool IsInRange(Vector2 point, float range)
    {
        // 내 기준(transform)으로 point가 range 범위 안에 있는지 체크합니다.
        // 범위 안이라면 true를 반환, 밖이라면 false를 반환
        return (Vector2.Distance(transform.position, point) <= range);
    }
    
    protected IEnumerator IdleStateCoroutine()
    {
        float waitTime = 0.0f;
        const float IDLE_WAIT_TIME = 3.0f;

        WaitForSeconds waitForBaseTerm = new WaitForSeconds(baseUpdateTerm);
        
        while (true)
        {
            // Idle 탈출조건
            // 1. 3초가 지났을때 PatrolState로 전환(Transition)
            // 2. DummyPlayer가 TraceRange 안에 있을때, AttackState로 전환

            if (IDLE_WAIT_TIME < waitTime)
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
            // PatrolState 탈출조건
            // 1. DummyPlayer가 TraceRange 안에 있을때, AttackState로 전환
            // 2. 목표 지점이 0.1f 보다 가까워 졌다면, IdleState로 전환

            if (IsInRange(Player.LocalPlayer.transform.position, traceRange))
            {
                NextStateCoroutine = AttackStateCoroutine();
                animator.SetBool(IS_MOVE, false);
                yield break;
            }

            
            if (IsInRange(goalPoint, 0.1f))
            {
                // 목표지점을 바꿔준다 A -> B, B-> A
                goalPoint = IsInRange(pointB, 0.1f) ? pointA : pointB;
                NextStateCoroutine = IdleStateCoroutine();
                animator.SetBool(IS_MOVE, false);
                yield break;
            }
            
            //-- PatrolState의 Update로직
            // 1. 목표지점을 향해 움직인다
           
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
                //NextStateCoroutine = IdleStateCoroutine();
                animator.SetBool(IS_MOVE, false);
                animator.SetTrigger(ATTACK);
                
                // 공격 주기만큼 대기하고 계속 타겟(플레이어)를 따라 붙게 한다
                yield return new WaitForSeconds(2.0f);
            }

            Move(Player.LocalPlayer.transform.transform.position);
            yield return null;
            waitTime += Time.deltaTime;
        }
    }
    
    protected IEnumerator DeadStateCoroutine()
    {
        yield return new WaitForSeconds(1.0f);
    }

    private void Move(Vector3 goalPosition)
    {
        animator.SetBool(IS_MOVE, true);
        
        // - 방향을구하는 방법 : (목표위치 - 나의위치)값을 정규화(Normalize) 시키면 방향이 된다
        //원래는 아래대로 쓰는게 정석입니다
        //Vector2 direction = (goalPoint - transform.position).normalized;
        float moveDirection = (goalPosition.x - transform.position.x); //moveDirection = 양수 또는 음수만 나옴
        moveDirection /= Mathf.Abs(moveDirection); // 같은 길이로 나눠주면 (-1, 1)등의 부호만 남는다
        transform.Translate(new Vector2(moveDirection, 0) * moveSpeed * Time.deltaTime);
        transform.localScale = new Vector3(moveDirection * originalScale.x, originalScale.y, originalScale.z);
    }
}
