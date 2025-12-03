using UnityEngine;
public class SlideAttackState : PlayerStateBase
{
    private float slideTimer;
    private int slideDir = 1;
    public SlideAttackState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entered SlideAttackState");
        
        slideDir = playercontroller.FacingDir;
        slideTimer = playercontroller.SlideDuration;
        
        animator.SetTrigger(playercontroller.AnimKeySlide);
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();
        
        slideTimer -= Time.deltaTime;

        if (slideTimer <= 0f)
        {
            float x = playercontroller.InputX;
            
            if(Mathf.Abs(x) >= 0.1f)
                stateMachine.ChangeState(playercontroller.RunState);
            else
                stateMachine.ChangeState(playercontroller.IdleState);

            return;
        }
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();

        float SlideSpeed = playercontroller.MoveSpeed * playercontroller.SlideSpeedMultiplier;

        float targetSpeed = slideDir * SlideSpeed;
        
        float t = slideTimer / playercontroller.SlideDuration;
        float smoothed = Mathf.Lerp(0.0f, 1.0f, t); 
        
        float slideSpeed = targetSpeed * smoothed;
        rigidBody.linearVelocity = new Vector2(slideSpeed, 0);
    }

    public override void Exit()
    {
        base.Exit();
        
        animator.ResetTrigger(playercontroller.AnimKeySlide);
    }
}