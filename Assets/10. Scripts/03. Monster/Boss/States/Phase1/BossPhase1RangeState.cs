using UnityEngine;

public class BossPhase1RangeState : BossStateBase
{
    private bool canStartFiringByAnimEvent;
    private bool canEndFiringByAnimEvent;
    private bool isAnimPaused; 
    
    private enum RangeStep
    {
        WaitingAnim,
        Firing,
        AfterDelay
    }

    private RangeStep step;

    private int firedCount;
    private float fireTimer;
    private float afterTimer;
    
    public BossPhase1RangeState(BossController boss, BossStateMachine stateMachine) : base(boss, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        step = RangeStep.WaitingAnim;
        firedCount = 0;
        fireTimer = 0f;
        afterTimer = 0f;
        
        canStartFiringByAnimEvent = false;
        canEndFiringByAnimEvent = false;
        isAnimPaused = false;

        animator.speed = 1f;
        animator.SetTrigger(bosscontroller.AnimKeyPh1Range);
    }

    public override void Exit()
    {
        base.Exit();

        animator.speed = 1f;
        animator.ResetTrigger(bosscontroller.AnimKeyPh1Range);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        switch (step)
        {
            case RangeStep.WaitingAnim:
                UpdateWaitingAnim();
                break;
            case RangeStep.Firing:
                UpdateFiring();
                break;
            case RangeStep.AfterDelay:
                UpdateAfterDelay();
                break;
        }
    }
    
    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
    }
    
    private void UpdateWaitingAnim()
    {
        if (canStartFiringByAnimEvent == false)
            return;

        step = RangeStep.Firing;
        fireTimer = 0f;
    }
    
    private void UpdateFiring()
    {
        if (canEndFiringByAnimEvent && isAnimPaused == false)
        {
            animator.speed = 0f;
            isAnimPaused = true;
        }

        fireTimer -= Time.deltaTime;

        if (fireTimer > 0f)
            return;
        
        FireOneProjectile();
        
        firedCount++;
        fireTimer = bosscontroller.RangeFireInterval;

        if (firedCount >= bosscontroller.RangeProjectileCount)
        {
            if (isAnimPaused)
            {
                animator.speed = 1.0f;
                isAnimPaused = false;
            }

            step = RangeStep.AfterDelay;
            canStartFiringByAnimEvent = false;
            canEndFiringByAnimEvent = false;
            afterTimer = bosscontroller.RangeAfterDelay;
        }
    }

    private void UpdateAfterDelay()
    {
        afterTimer -= Time.deltaTime;

        if (afterTimer <= 0f)
        {
            stateMachine.ChangeState(bosscontroller.IdleState);
        }
    }

    private void FireOneProjectile()
    {
        Debug.Log("Ph1Range ::: Fire Projectile!");
        Vector3 spawnPos = bosscontroller.ProjectileSpawnPoint.position;

        BossTrackingProjectile proj =
            GameObject.Instantiate(bosscontroller.ProjectilePrefab, spawnPos, Quaternion.identity);

        proj.Init(bosscontroller.playerTarget, bosscontroller.RangeProjectileSpeed, bosscontroller.RangeTurnRate,
            bosscontroller.RangeLifeTime, bosscontroller.Boss);
    }

    public void OnAnimEvent_FireStart() => canStartFiringByAnimEvent = true;

    public void OnAnimEvent_FireEnd() => canEndFiringByAnimEvent = true;
}