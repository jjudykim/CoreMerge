using UnityEngine;

public class AttackState : PlayerStateBase
{
    private float attackTimer;

    public AttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        // Debug.Log("Entered AttackState");

        attackTimer = playercontroller.DashAttackDuration;

        animator.SetTrigger(playercontroller.AnimKeyAttack);
        animator.Play("Attack");
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

            return;
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
        
        rigidBody.linearVelocity = new Vector2(0f, rigidBody.linearVelocity.y);
    }

    public override void Exit()
    {
        base.Exit();
        
        animator.ResetTrigger(playercontroller.AnimKeyAttack);
    }
}