using UnityEngine;

public class DeadState : PlayerStateBase
{
    public DeadState(PlayerController player, PlayerStateMachine stateMachine) : base(player, stateMachine)
    {
    }
}