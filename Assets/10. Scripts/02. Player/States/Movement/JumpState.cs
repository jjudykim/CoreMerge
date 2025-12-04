using UnityEngine;

public class JumpState : PlayerStateBase
{
    public JumpState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // Debug.Log("Entered JumpState");

        Vector2 velocity = rigidBody.linearVelocity;
        velocity.y = playercontroller.JumpForce;
        rigidBody.linearVelocity = velocity;
        
        animator.SetTrigger(playercontroller.AnimKeyJump);
        animator.Play("Jump");
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        float x = playercontroller.InputX;
        float y = playercontroller.InputY;

        if (playercontroller.IsGrounded && rigidBody.linearVelocity.y <= 0.01f)
        {
            if (Mathf.Abs(x) < 0.1f)
                stateMachine.ChangeState(playercontroller.IdleState);
            else
                stateMachine.ChangeState(playercontroller.RunState);

            return;
        }

        if (playercontroller.IsOnLadder && Mathf.Abs(y) >= 0.1f)
        {
            stateMachine.ChangeState(playercontroller.ClimbState);
            return;
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        float x = playercontroller.InputX * playercontroller.MoveSpeed * 0.7f;
        rigidBody.linearVelocity = new Vector2(x, rigidBody.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        animator.ResetTrigger(playercontroller.AnimKeyJump);
    }
}