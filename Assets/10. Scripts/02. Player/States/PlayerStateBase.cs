using System;
using UnityEngine;

public abstract class PlayerStateBase : IPlayerState
{
    protected PlayerController playercontroller;
    protected PlayerStateMachine stateMachine;
    protected Collider2D collider;
    protected Animator animator;
    protected Rigidbody2D rigidBody;

    protected PlayerStateBase(PlayerController player, PlayerStateMachine stateMachine)
    {
        this.playercontroller = player;
        this.stateMachine = stateMachine;
        this.collider = player.Collider;
        this.animator = player.Animator;
        this.rigidBody = player.Rigidbody;
    }

    public virtual void Enter()
    {
        // 상태 진입에 대한 공통 처리
    }

    public virtual void Exit()
    {
        // 상태 퇴장에 대한 공통 처리
    }
    
    public virtual void UpdateLogic()
    {
        // 공통 논리 업데이트
    }

    public virtual void UpdatePhysics()
    {
        // 공통 물리 업데이트        
    }
}