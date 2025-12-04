using UnityEngine;

public class HurtState : PlayerStateBase
{
    private float hurtTimer;
    public HurtState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entered HurtState");

        playercontroller.IsHurt = true;

        hurtTimer = playercontroller.HurtDurtaion;
        animator.SetTrigger(playercontroller.AnimKeyHurt);
        animator.Play("Hurt");
        
        // knockback
        int dirX = playercontroller.FacingDir * -1;
        Vector2 knockbackDir = new Vector2(dirX, playercontroller.KnockbackUpRatio).normalized;
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.AddForce(knockbackDir * playercontroller.KnockbackPower, ForceMode2D.Impulse);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        
        hurtTimer -= Time.deltaTime;
        if (hurtTimer <= 0f)
        {
            if (playercontroller.IsDead)
            {
                stateMachine.ChangeState(playercontroller.DeadState);
                return;
            }

            float x = playercontroller.InputX;
            
            if (Mathf.Abs(x) > 0.01f)
                stateMachine.ChangeState(playercontroller.RunState);
            else
                stateMachine.ChangeState(playercontroller.IdleState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        playercontroller.IsHurt = false;
        animator.ResetTrigger(playercontroller.AnimKeyHurt);
    }
}