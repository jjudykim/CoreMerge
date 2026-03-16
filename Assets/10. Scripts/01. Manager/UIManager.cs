using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Prefabs")] 
    [SerializeField] private GameObject hudPrefab;

    [Header("Transitions")]
    [SerializeField] private SceneFader sceneFader;
    
    private static GameObject hudInstance;

    private GameObject hudContent;
    
    private PlayerStatUI playerStatUI;
    private LifeHUD lifeHUD;
    private GameEndUI gameEndUI;

    private const string SCENE_LOBBY = "Lobby";
    
    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ChangeScene(string sceneName)
    {
        Managers.Instance.Scene.LoadScene(sceneName);
    }

    public void FadeOut(Action onComplete = null)
    {
        if (sceneFader != null)
            sceneFader.FadeOut(onComplete);
        else
            onComplete?.Invoke();
    }

    public void FadeIn(Action onComplete = null)
    {
        if (sceneFader != null)
            sceneFader.FadeIn(onComplete);
        else
            onComplete?.Invoke();
    }
    

    public void EnsureHUD()
    {
        if (hudInstance != null)
        {
            hudInstance.SetActive(true);
            return;    
        }

        hudInstance = Instantiate(hudPrefab);
        DontDestroyOnLoad(hudInstance);

        Transform contentTransform = hudInstance.transform.Find("Content");
        if (contentTransform != null)
            hudContent = contentTransform.gameObject;
        
        playerStatUI = hudInstance.GetComponentInChildren<PlayerStatUI>(true);
        lifeHUD = hudInstance.GetComponentInChildren<LifeHUD>(true);
        gameEndUI = hudInstance.GetComponentInChildren<GameEndUI>(true);
        
        if (sceneFader == null)
            sceneFader = hudInstance.GetComponentInChildren<SceneFader>(true);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureHUD();
        
        bool isLobby = scene.name == SCENE_LOBBY;
        
        if (hudContent != null)
            hudContent.SetActive(!isLobby);

        RebindToCurrentPlayer();

        if (gameEndUI != null)
            gameEndUI.Hide();
    }

    public void RebindToCurrentPlayer()
    {
        Player p = Player.LocalPlayer;
        
        if (p == null)
            p = FindFirstObjectByType<Player>();

        if (playerStatUI != null)
            playerStatUI.Bind(p);
        
        if (lifeHUD != null)
            lifeHUD.Bind(p);
    }

    public void ShowGameEndUI(bool isSuccess)
    {
        if (gameEndUI != null)
            gameEndUI.Show(isSuccess);
    }
}


