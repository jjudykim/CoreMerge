using UnityEngine;

public enum GameFlowStep
{
    Lobby = 0,
    Tutorial = 1,
    Stage = 2,
    BossRoom = 3,
    ReturnToLobby = 4
}

public class GameFlowManager : MonoBehaviour
{
    public GameFlowStep CurrentStep { get; private set; } = GameFlowStep.Lobby;
    
    private const string SCENE_LOBBY = "Lobby";
    private const string SCENE_TUTORIAL = "TutorialStage";
    private const string SCENE_STAGE = "Stage";
    private const string SCENE_BOSS = "BossRoom";

    public void GoToLobby()
    {
        CurrentStep = GameFlowStep.Lobby;
        Managers.Instance.Scene.LoadScene(SCENE_LOBBY, portalId: "LobbySpawn");
    }

    public void StartTutorial()
    {
        CurrentStep = GameFlowStep.Tutorial;
        Managers.Instance.Scene.LoadScene(SCENE_TUTORIAL, portalId: "TutorialStart");
    }

    public void StartStage()
    {
        CurrentStep = GameFlowStep.Stage;
        Managers.Instance.Scene.LoadScene(SCENE_STAGE, portalId: "StageStart");
    }

    public void EnterBossRoom()
    {
        CurrentStep = GameFlowStep.BossRoom;
        Managers.Instance.Scene.LoadScene(SCENE_BOSS, portalId: "BossEnter");
    }

    public void ReturnLobby()
    {
        CurrentStep = GameFlowStep.ReturnToLobby;
        Managers.Instance.Scene.LoadScene(SCENE_LOBBY, portalId: "Lobby");
    }
    
    public void GoNext()
    {
        switch (CurrentStep)
        {
            case GameFlowStep.Lobby:
                StartTutorial();
                break;
            case GameFlowStep.Tutorial:
                StartStage();
                break;
            case GameFlowStep.Stage:
                EnterBossRoom();
                break;
            case GameFlowStep.BossRoom:
                ReturnLobby();
                break;
            default:
                GoToLobby();
                break;
        }
    }
}
