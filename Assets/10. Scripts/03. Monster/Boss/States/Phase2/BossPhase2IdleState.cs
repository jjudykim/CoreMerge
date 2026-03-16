using UnityEngine;

public class BossPhase2IdleState : BossStateBase
{
    private float waitTimer;

    private const float MinWaitTime = 3.0f;
    private const float MaxWaitTime = 5.0f;
    
    public BossPhase2IdleState(BossController boss, BossStateMachine stateMachine) : base(boss, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        animator.SetTrigger(bosscontroller.AnimKeyPh2Idle);
        waitTimer = Random.Range(MinWaitTime, MaxWaitTime);
    }

    public override void Exit()
    {
        base.Exit();
        animator.ResetTrigger(bosscontroller.AnimKeyPh2Idle);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
        {
            stateMachine.ChangeState(bosscontroller.Ph2HandState);
        }
    }
}