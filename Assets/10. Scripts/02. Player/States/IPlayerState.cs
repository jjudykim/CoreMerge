using UnityEngine;

public interface IPlayerState
{
    void Enter();
    void Exit();
    void UpdateLogic();
    void UpdatePhysics();
}
