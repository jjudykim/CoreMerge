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
        
        if (playercontroller.DashRightDown || playercontroller.DashLeftDown)
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

        float x = playercontroller.InputX * playercontroller.MoveSpeed;
        rigidBody.linearVelocity = new Vector2(x, rigidBody.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();

        animator.ResetTrigger(playercontroller.AnimKeyRun);
    }
}