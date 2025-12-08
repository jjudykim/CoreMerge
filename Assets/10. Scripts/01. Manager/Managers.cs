using System;
using Jay;
using UnityEngine;

// Develop용 임시 Order 설정
[DefaultExecutionOrder(-100)]
public class Managers : SingletonBase<Managers>
{
    public static Managers Instance => instance;

    public InputManager Input { get; private set; }
    public CoreDBManager CoreDB { get; private set; }
    public CoreDropManager CoreDrop { get; private set; }

    [Header("Drop Tables")] [SerializeField]
    private MonsterTypeDropTable[] monsterDropTables;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        
        // Manager Instantiate
        Input = new InputManager();
        CoreDB = new CoreDBManager();
        CoreDrop = new CoreDropManager();
        
        // Manager Initialize
        CoreDB.Init();
        CoreDrop.Init(monsterDropTables);
    }

    private void Update()
    {
        Input.Update();
    }
}