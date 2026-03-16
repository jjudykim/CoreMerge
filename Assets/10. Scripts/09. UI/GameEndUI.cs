using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private Button lobbyButton;
    [SerializeField] private TextMeshProUGUI titleText;

    private void Awake()
    {
        if (lobbyButton != null)
            lobbyButton.onClick.AddListener(OnLobbyButtonClicked);
        
        Hide();
    }

    public void Show(bool isSuccess)
    {
        gameObject.SetActive(true);
        
        if (successPanel != null) successPanel.SetActive(isSuccess);
        if (failPanel != null) failPanel.SetActive(!isSuccess);

        if (titleText != null)
        {
            titleText.gameObject.SetActive(true);
            titleText.text = isSuccess ? "Clear!" : "Failed...";
            titleText.color = isSuccess ? Color.yellow : Color.red;
        }
        
        if (lobbyButton != null)
            lobbyButton.gameObject.SetActive(true);
        
        // 게임 정지 (필요 시)
        Time.timeScale = 0f;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    private void OnLobbyButtonClicked()
    {
        Managers.Instance.Game.ResetGame();
        Time.timeScale = 1f;
        Managers.Instance.Flow.GoToLobby();
        Hide();
    }
}
