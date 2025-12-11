using UnityEngine;

public class IdleState : PlayerStateBase
{
    private float dashBufferTime = 0.08f;
    private float dashBufferTimer = 0f;
    private bool dashBufferActive = false;
    public IdleState(PlayerController player, PlayerStateMachine stateMachine) 
        : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // Debug.Log("Entered IdleState");
        dashBufferTimer = 0f;
        dashBufferActive = false;
        ResetAnimationTriggers();
        
        animator.SetTrigger(playercontroller.AnimKeyIdle);
    }

    private void ResetAnimationTriggers()
    {
        animator.ResetTrigger("Jump");
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Dash");
        animator.ResetTrigger("Slide");
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        float x = playercontroller.InputX;
        float y = playercontroller.InputY;
        //Debug.Log($"IdleState ::: InputX : {x} ::: InputY : {y}");
        
        if (playercontroller.JumpPressed && playercontroller.IsGrounded && y > 0f)
        {
            stateMachine.ChangeState(playercontroller.JumpState);
            return;
        }
        
        if (playercontroller.DashDown)
        {
            dashBufferActive = false;
            dashBufferTimer = 0f;
            
            stateMachine.ChangeState(playercontroller.DashState);
            return;
        }
        
        if (playercontroller.IsOnLadder && y > 0f)
        {
            stateMachine.ChangeState(playercontroller.ClimbState);
            return;
        }
        
        if (playercontroller.IsOnLadder == false && playercontroller.IsLadderBelow && y < -0.1f)
        {
            stateMachine.ChangeState(playercontroller.ClimbState);
            return;
        }
        
        if (playercontroller.AttackPressed)
        {
            stateMachine.ChangeState(playercontroller.AttackState);
            return;
        }

        if (Mathf.Abs(x) >= 0.1f)
        {
            if (!dashBufferActive)
            {
                dashBufferActive = true;
                dashBufferTimer = dashBufferTime;
            }
            else
            {
                dashBufferTimer -= Time.deltaTime;

                if (dashBufferTimer <= 0f)
                {
                    stateMachine.ChangeState(playercontroller.RunState);
                    return;
                }
            }
            
        }
        else
        {
            dashBufferActive = false;
            dashBufferTimer = 0f;
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        rigidBody.linearVelocity = new Vector2(0, rigidBody.linearVelocity.y);
    }
    
    public override void Exit()
    {
        animator.ResetTrigger(playercontroller.AnimKeyIdle);
    }
}