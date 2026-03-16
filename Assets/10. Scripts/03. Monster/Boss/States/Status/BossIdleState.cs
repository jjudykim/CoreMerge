using UnityEngine;

public class BossIdleState : BossStateBase
{
    private float waitTimer;

    private float minWaitTime = 2f;
    private float maxWaitTime = 5f;
    public BossIdleState(BossController boss, BossStateMachine stateMachine) : base(boss, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("[Boss] Enter IdleState");
        
        animator.Play("Idle");
        animator.SetTrigger(bosscontroller.AnimKeyIdle);
        waitTimer = Random.Range(minWaitTime, maxWaitTime);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        
        waitTimer -= Time.deltaTime;

        if (waitTimer <= 0f)
            SelectNextPattern();
    }
    
    public override void Exit()
    {
        base.Exit();
        animator.ResetTrigger(bosscontroller.AnimKeyIdle);
    }
    
    private void SelectNextPattern()
    {
        // Test용
        //SelectPhase1Pattern();
        
        switch (bosscontroller.CurrentPhase)
        {
            case BossPhase.Phase1:
                SelectPhase1Pattern();
                break;
            case BossPhase.Phase2:
                SelectPhase2Pattern();
                break;
            case BossPhase.Dead:
                // TODO : Dead Phase라면 실제 DeadState로 넘길까?? 고민중
                break;
                
        }
    }

    private void SelectPhase1Pattern()
    {
        int pattern = Random.Range(0, 2);
        if (pattern == 0)
            stateMachine.ChangeState(bosscontroller.Ph1DashAttackState);
        else
            stateMachine.ChangeState(bosscontroller.Ph1RangeState);
    }
    
    private void SelectPhase2Pattern()
    {
        int pattern = Random.Range(0, 2);
        
        if (pattern == 0)
            stateMachine.ChangeState(bosscontroller.Ph2RangeState);
        else
            stateMachine.ChangeState(bosscontroller.Ph2HandState);
    }
}