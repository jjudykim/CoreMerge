using System;
using Jay;
using UnityEngine;

// Develop용 임시 Order 설정
[DefaultExecutionOrder(-100)]
public class Managers : SingletonBase<Managers>
{
    [SerializeField] private GameObject playerPrefab;
    public GameObject PlayerPrefab => playerPrefab;
    
    public PlayerRuntimeData PlayerData { get; private set; }

    public static Managers Instance => instance;

    public GameManager Game { get; private set; }
    public InputManager Input { get; private set; }
    public SaveManager Save { get; private set; }

    public QuickSlotData QuickSlots { get; private set; }
    public CoreInventoryData CoreInventory { get; private set; }
    public CoreDBManager CoreDB { get; private set; }
    public CoreDropManager CoreDrop { get; private set; }
    public SceneLoadManager Scene { get; private set; }
    public GameFlowManager Flow { get; private set; }
    public UIManager UI { get; private set; }
    
    private void InitPlayerData()
    {
        if (PlayerData == null)
            PlayerData = new PlayerRuntimeData();
    }
    
    [Header("Drop Tables")] 
    [SerializeField] private MonsterTypeDropTable[] monsterDropTables;
    
    [Header("Scene Manager")]
    [SerializeField] private SceneLoadManager sceneLoadManager;
    [SerializeField] private GameFlowManager flowManager;
    
    [Header("UIManager")]
    [SerializeField] private UIManager uiManager;
    
    protected override void Awake()
    {
        base.Awake();

        //if (instance != null)
        //    return;
        
        InitPlayerData();

        if (sceneLoadManager == null)
            sceneLoadManager = gameObject.AddComponent<SceneLoadManager>();

        if (flowManager == null)
            flowManager = gameObject.AddComponent<GameFlowManager>();
        
        if (uiManager == null)
            uiManager = gameObject.AddComponent<UIManager>();
        
        Scene = sceneLoadManager;
        Flow = flowManager;
        UI = uiManager;
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        
        // Manager Instantiate
        Game = new GameManager();
        Input = new InputManager();
        Save = new SaveManager();
        CoreInventory = new CoreInventoryData(32);
        QuickSlots = new QuickSlotData(3);
        CoreDB = new CoreDBManager();
        CoreDrop = new CoreDropManager();
        
        // Manager Initialize
        Game.Init();
        Save.Init();
        CoreDB.Init();
        CoreDrop.Init(monsterDropTables);
    }

    private void Update()
    {
        Input.Update();
    }
}