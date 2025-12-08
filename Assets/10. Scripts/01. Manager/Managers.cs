using System;
using Jay;
using UnityEngine;

// Develop용 임시 Order 설정
[DefaultExecutionOrder(-100)]
public class Managers : SingletonBase<Managers>
{
    public static Managers Instance => instance;

    public static InputManager Input { get; private set; }
    public static CoreDBManager CoreDB { get; private set; }

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
        
        // Manager Initialize
        CoreDB.Init();
    }

    private void Update()
    {
        Input.Update();
    }
}