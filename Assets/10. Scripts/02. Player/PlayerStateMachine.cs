using UnityEngine;

public class PlayerStateMachine
{
    public IPlayerState CurrentState { get; private set; }

    public void Init(IPlayerState startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
    }

    public void ChangeState(IPlayerState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}