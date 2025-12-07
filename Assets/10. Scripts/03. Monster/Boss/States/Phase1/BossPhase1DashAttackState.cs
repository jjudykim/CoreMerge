using Unity.VisualScripting;
using UnityEngine;
public class BossPhase1DashAttackState : BossStateBase
{
    private enum DashStep
    {
        Telepgraph,        // 돌진 준비 (제자리, 이펙트)
        Dashing,           // 목표 지점으로 이동
        Attack,            // 근접 공격 모션
        Returning          // 원위치 복귀 + 알파 페이드
    }
    
    private DashStep currentStep;
    private Vector2 targetPosition;
    
    // Dash Control
    private float dashProgress = 0f;
    private float dashDistance;

    private float telegraphTimer;
    private float attackTimer;
    private float returnTimer;            // 경과 시간 누적용

    private float totalReturnDuration;

    public BossPhase1DashAttackState(BossController boss, BossStateMachine stateMachine) : base(boss, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        currentStep = DashStep.Telepgraph;
        telegraphTimer = bosscontroller.DashTelegraphTime;
        animator.SetTrigger(bosscontroller.AnimKeyForward);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        switch (currentStep)
        {
            case DashStep.Telepgraph:
                UpdateTelegraph();
                break;
            case DashStep.Dashing:
                break;
            case DashStep.Attack:
                UpdateAttack();
                break;
            case DashStep.Returning:
                UpdateReturn();
                break;
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        if (currentStep == DashStep.Dashing)
        {
            Vector2 current = bosscontroller.transform.position;

            Vector2 toTarget = targetPosition - current;
            float remaining = toTarget.magnitude;

            float reachThreshold = 0.2f;
            
            if (remaining <= reachThreshold)
            {
                bosscontroller.transform.position = targetPosition;
                
                currentStep = DashStep.Attack;
                attackTimer = bosscontroller.AttackDuration;
            
                animator.ResetTrigger(bosscontroller.AnimKeyForward);
                animator.SetTrigger(bosscontroller.AnimKeyAttack);
                return;
            }

            Vector2 dir = toTarget / remaining;
            float baseSpeed = bosscontroller.DashSpeed;
            float distanceRatio = Mathf.Clamp01(remaining / dashDistance);
            float speed = baseSpeed * Mathf.Pow(distanceRatio, 2);

            float minSpeed = baseSpeed * 0.3f;
            if (speed < minSpeed)
                speed = minSpeed;

            Vector2 newPos = current + dir * (speed * Time.fixedDeltaTime);
            bosscontroller.transform.position = newPos;
        }
    }
    
    public override void Exit()
    {
        base.Exit();

        animator.ResetTrigger(bosscontroller.AnimKeyAttack);
        ResetAlpha();
    }

    private void UpdateTelegraph()
    {
        telegraphTimer -= Time.deltaTime;

        if (telegraphTimer <= 0f)
        {
            Vector2 playerPos = bosscontroller.PlayerPosition;
            targetPosition = new Vector2(playerPos.x + bosscontroller.DashOffset.x
                                       , playerPos.y + bosscontroller.DashOffset.y);
            
            dashDistance = Vector2.Distance(bosscontroller.OriginPosition, targetPosition);
            dashProgress = 0f;
            
            currentStep = DashStep.Dashing;
            
            // TODO : 추후 Material 효과 변경 적용
        }
    }
    
    // private void CheckDashComplete()
    // {
    //    Vector2 current = bosscontroller.transform.position;
    //    float distance = Vector2.Distance(current, targetPosition);
    //    float arriveThreshold = 0.1f;
    //    
    //    if (distance <= arriveThreshold)
    //    {
    //        
    //    }
    //}
    
    private void UpdateAttack()
    {
        attackTimer -= Time.deltaTime;
        
        if (attackTimer <= 0f)
        {
            currentStep = DashStep.Returning;
            
            returnTimer = 0f;
            totalReturnDuration = Mathf.Max(0.01f, bosscontroller.ReturnDuration);
        }
    }
    
    private void UpdateReturn()
    {
        returnTimer += Time.deltaTime;
        float t = Mathf.Clamp01(returnTimer / totalReturnDuration);

        Vector2 pos = Vector2.Lerp(targetPosition, bosscontroller.OriginPosition, t);
        bosscontroller.transform.position = pos;

        float alpha;
        float fadeOutEnd = 0.2f;
        float fadeInStart = 0.8f;
        if (t < fadeOutEnd)
            alpha = Mathf.Lerp(1f, 0f, t / fadeOutEnd);
        else if (t < fadeInStart)
            alpha = 0f;
        else
        {
            float normalized = (t - fadeInStart) / (1f - fadeInStart);
            alpha = Mathf.Lerp(0f, 1f, normalized);
        }
        SetAlpha(alpha);

        if (t >= 1f)
        {
            SetAlpha(1f);
            stateMachine.ChangeState(bosscontroller.IdleState);
        }
    }

    private void SetAlpha(float alpha)
    {
        Color c = bosscontroller.SpriteRenderer.color;
        c.a = alpha;
        bosscontroller.SpriteRenderer.color = c;
    }

    private void ResetAlpha() => SetAlpha(1f);
}