using UnityEngine;
public class RunState : PlayerStateBase
{
    private bool firstFrame = true;
    
    public RunState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Entered RunState");
        animator.SetTrigger(playercontroller.AnimKeyRun);
        firstFrame = true;
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        
        float x = playercontroller.InputX;
        float y = playercontroller.InputY;
        
        if (playercontroller.AttackPressed)
        {
            stateMachine.ChangeState(playercontroller.AttackState);
            return;
        }

        if (playercontroller.JumpPressed && playercontroller.IsGrounded)
        {
            stateMachine.ChangeState(playercontroller.JumpState);
            return;
        }
        
        if (playercontroller.DashDown)
        {
            stateMachine.ChangeState(playercontroller.DashState);
            return;
        }

        if (playercontroller.IsOnLadder && Mathf.Abs(y) >= 0.1f)
        {
            stateMachine.ChangeState(playercontroller.ClimbState);
            return;
        }

        if (Mathf.Abs(x) < 0.1f)
        {
            stateMachine.ChangeState(playercontroller.IdleState);
            return;
        }

        if (playercontroller.SlideAttackPressed && Mathf.Abs(x) >= 0.1f)
        {
            stateMachine.ChangeState(playercontroller.SlideAttackState);
            return;
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        if (firstFrame)
        {
            firstFrame = false;
            return;
        }

        float inputX = playercontroller.InputX;
        float targetX = inputX * playercontroller.MoveSpeed;

        if (targetX > 0f && playercontroller.IsRightWall)
            targetX = 0f;
        else if (targetX < 0f && playercontroller.IsLeftWall)
            targetX = 0f;
        
        rigidBody.linearVelocity = new Vector2(targetX, rigidBody.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        animator.ResetTrigger(playercontroller.AnimKeyRun);
    }
}