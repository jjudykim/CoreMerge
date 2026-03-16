using System.Collections;
using UnityEngine;

public class DeadState : PlayerStateBase
{
    private Coroutine deadCoroutine;
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
        
        deadCoroutine = playercontroller.StartCoroutine(DeadSequence());
    }

    public override void Exit()
    {
        base.Exit();

        if (deadCoroutine != null)
        {
            playercontroller.StopCoroutine(deadCoroutine);
            deadCoroutine = null;
        }

        animator.SetBool(playercontroller.AnimKeyIsDead, false);
        animator.ResetTrigger(playercontroller.AnimKeyDead);
    }

    private IEnumerator DeadSequence()
    {
        yield return new WaitForSeconds(3.0f);

        Managers.Instance.Game.OnPlayerDead();
    }
}