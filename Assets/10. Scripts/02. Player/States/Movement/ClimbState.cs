using UnityEngine;

public class ClimbState : PlayerStateBase
{
    private float originalGravityScale; // 원래의 중력값
    private float climbSpeed;

    private int playerLayer;
    private int groundLayer;

    private bool enteredFromAbove = false;
    
    public ClimbState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }
    
    public override void Enter()
    {
        base.Enter();
        //Debug.Log("Entered ClimbState");

        playerLayer = playercontroller.gameObject.layer;
        groundLayer = playercontroller.GroundPhysicsLayer;
        
        Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, true);
        
        originalGravityScale = rigidBody.gravityScale;
        rigidBody.gravityScale = 0;

        rigidBody.linearVelocity = Vector2.zero;

        climbSpeed = playercontroller.ClimbSpeed;
        animator.speed = 0f;

        enteredFromAbove = playercontroller.IsLadderBelow 
                           && playercontroller.IsGrounded == false;
        if (enteredFromAbove && playercontroller.CurrentLadder != null)
        {
            var ladderBounds = playercontroller.CurrentLadder.bounds;
            var playerBounds = playercontroller.Collider.bounds;

            Vector3 pos = playercontroller.transform.position;

            pos.x = ladderBounds.center.x;

            float playerHalfHeight = playerBounds.extents.y;
            float ladderTop = ladderBounds.max.y;

            pos.y = ladderTop + playerHalfHeight;

            playercontroller.transform.position = pos;
        }
        
        animator.Play("Ladder");
    }

    public override void UpdateLogic()
    {
        base.UpdateLogic();

        float x = playercontroller.InputX;
        float y = playercontroller.InputY;

        if (!playercontroller.IsOnLadder)
        {
            animator.speed = 1f;

            if (playercontroller.IsGrounded)
                stateMachine.ChangeState(playercontroller.IdleState);

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
        
        Physics2D.IgnoreLayerCollision(playerLayer, groundLayer, false);
        
        rigidBody.gravityScale = originalGravityScale;
        animator.speed = 1f;
    }
}