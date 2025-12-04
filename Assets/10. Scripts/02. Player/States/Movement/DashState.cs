using UnityEngine;

public class DashState : PlayerStateBase
{
    private float dashTimer = 0f;
    private int dashDir = 1;
    
    public DashState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Entered DashState");

        if (playercontroller.DashRightDown) playercontroller.FacingDir = 1;
        else if (playercontroller.DashLeftDown) playercontroller.FacingDir = -1;
        dashDir = playercontroller.FacingDir;

        dashTimer = playercontroller.DashDuration;
        
        animator.SetTrigger(playercontroller.AnimKeyDash);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        if (playercontroller.AttackPressed)
        {
            stateMachine.ChangeState(playercontroller.DashAttackState);
            return;
        }

        dashTimer -= Time.deltaTime;

        if (dashTimer <= 0f)
        {
            float x = playercontroller.InputX;

            ApplyDashEndPenalty();
            
            if(Mathf.Abs(x) >= 0.1f)
                stateMachine.ChangeState(playercontroller.RunState);
            else
                stateMachine.ChangeState(playercontroller.IdleState);
            return;
        }
    }

    private void ApplyDashEndPenalty()
    {
        float penaltySpeed = rigidBody.linearVelocity.x * 0.2f;
        rigidBody.linearVelocity = new Vector2(penaltySpeed, rigidBody.linearVelocity.y);
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        float dashSpeed = playercontroller.MoveSpeed * playercontroller.DashSpeedMultiplier;

        float targetSpeed = dashDir * dashSpeed;

        float t = dashTimer / playercontroller.DashDuration;

        float smoothed = Mathf.Pow(t, 2f); 
        rigidBody.linearVelocity = new Vector2(targetSpeed * smoothed, 0);
    }

    public override void Exit()
    {
        base.Exit();
        
        animator.ResetTrigger(playercontroller.AnimKeyDash);
    }
}