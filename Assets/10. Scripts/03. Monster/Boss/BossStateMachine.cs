using UnityEngine;

public class BossStateMachine
{
    public BossStateBase CurrentState { get; private set; }

    public void Init(BossStateBase startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
    }

    public void ChangeState(BossStateBase newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}