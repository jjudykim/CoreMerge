using UnityEngine;

public class ChangeSceneTrigger : MonoBehaviour
{
    public enum TriggerAction
    {
        GoNext,
        GoLobby,
        StartTutorial,
        StartStage,
        EnterBoss
    }

    [Header("Target")]
    [SerializeField] private TriggerAction action;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false)
            return;

        if (Managers.Instance.Flow == null)
            return;

        switch (action)
        {
            case TriggerAction.GoNext:
                Managers.Instance.Flow.GoNext();
                break;
            case TriggerAction.GoLobby:
                Managers.Instance.Flow.GoToLobby();
                break;
            case TriggerAction.StartTutorial:
                Managers.Instance.Flow.StartTutorial();
                break;
            case TriggerAction.StartStage:
                Managers.Instance.Flow.StartStage();
                break;
            case TriggerAction.EnterBoss:
                Managers.Instance.Flow.EnterBossRoom();
                break;
        }
    }
}