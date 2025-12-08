using UnityEngine;

public class DeadState : PlayerStateBase
{
    public DeadState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entered DeadState");

        collider.enabled = false;
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.simulated = false;
        animator.SetBool(playercontroller.AnimKeyIsDead, true);
        animator.SetTrigger(playercontroller.AnimKeyDead);

        Managers.Instance.Input.SetEnable(true);
    }

    public override void Exit()
    {
        base.Exit();
        
        animator.SetBool(playercontroller.AnimKeyIsDead, false);
        animator.ResetTrigger(playercontroller.AnimKeyDead);
    }
}