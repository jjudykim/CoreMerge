using System;
using Jay;
using UnityEngine;

// Develop용 임시 Order 설정
[DefaultExecutionOrder(-100)]
public class Managers : SingletonBase<Managers>
{
    public static Managers Instance => instance;

    public InputManager Input { get; private set; }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        
        Input = new InputManager();
    }

    private void Update()
    {
        Input.Update();
    }
}