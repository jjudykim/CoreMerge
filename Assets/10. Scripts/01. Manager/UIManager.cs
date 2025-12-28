using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Prefabs")] 
    [SerializeField] private GameObject hudPrefab;

    private GameObject hudInstance;

    private PlayerStatUI playerStatUI;
    private LifeHUD lifeHUD;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    public void EnsureHUD()
    {
        if (hudInstance != null)
            return;

        hudInstance = Instantiate(hudPrefab);
        DontDestroyOnLoad(hudInstance);
        
        playerStatUI = hudInstance.GetComponentInChildren<PlayerStatUI>(true);
        lifeHUD = hudInstance.GetComponentInChildren<LifeHUD>(true);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureHUD();
        RebindToCurrentPlayer();
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
}