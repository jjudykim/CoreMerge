using UnityEngine;

public class ClimbState : PlayerStateBase
{
    private float originalGravityScale; // 원래의 중력값
    private float climbSpeed;
    
    public ClimbState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Entered ClimbState");
        
        originalGravityScale = rigidBody.gravityScale;
        rigidBody.gravityScale = 0;

        rigidBody.linearVelocity = Vector2.zero;

        climbSpeed = playercontroller.ClimbSpeed;
        animator.speed = 0f;
        
        animator.Play("Ladder");
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        float x = playercontroller.InputX;
        float y = playercontroller.InputY;

        if (playercontroller.IsGrounded)
        {
            if (y < 0.1f)
            {
                stateMachine.ChangeState(playercontroller.IdleState);
                return;
            }
        }

        if (!playercontroller.IsOnLadder)
        {
            animator.speed = 1f;

            if (playercontroller.IsGrounded)
            {
                if (Mathf.Abs(x) >= 0.1f)
                    stateMachine.ChangeState(playercontroller.RunState);
                else
                    stateMachine.ChangeState(playercontroller.IdleState);
            }
            else
            {
                stateMachine.ChangeState(playercontroller.JumpState);
            }

            return;
        }

        if (Mathf.Abs(y) >= 0.1f) 
            animator.speed = 1f;
        else
            animator.speed = 0f;
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        
        float y = playercontroller.InputY;

        float verticalVelocity = 0f;

        if (Mathf.Abs(y) >= 0.1f)
            verticalVelocity = y * climbSpeed;
        
        rigidBody.linearVelocity = new Vector2(0f, verticalVelocity);
    }

    public override void Exit()
    {
        base.Exit();
        
        rigidBody.gravityScale = originalGravityScale;
        animator.speed = 1f;
    }
}