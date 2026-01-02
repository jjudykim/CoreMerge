using System;
using UnityEngine;

public abstract class BossStateBase : IBossState
{
    protected BossController bosscontroller;
    protected BossStateMachine stateMachine;
    protected Collider2D collider;
    protected Animator animator;

    protected BossStateBase(BossController bossController, BossStateMachine stateMachine)
    {
        this.bosscontroller = bossController;
        this.stateMachine = stateMachine;
        this.collider = bossController.Collider;
        this.animator = bossController.Animator;
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