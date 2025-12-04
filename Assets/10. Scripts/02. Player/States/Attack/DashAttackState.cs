using UnityEngine;

public class DashAttackState : PlayerStateBase
{
    private float attackTimer;
    public DashAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // Debug.Log("Entered DashAttackState");

        int dir = playercontroller.FacingDir;

        attackTimer = playercontroller.DashAttackDuration;
        animator.SetTrigger(playercontroller.AnimKeyDashAttack);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            float x = playercontroller.InputX;
            
            if (Mathf.Abs(x) >= 0.1f)
                stateMachine.ChangeState(playercontroller.RunState);
            else
                stateMachine.ChangeState(playercontroller.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        animator.ResetTrigger(playercontroller.AnimKeyDashAttack);
    }
}