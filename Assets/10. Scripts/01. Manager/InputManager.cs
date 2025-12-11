using UnityEngine;

public class InputManager
{
    // Move
    public float MoveX { get; private set; }
    public float MoveY { get; private set; }
    
    // Button Down
    public bool JumpDown { get; private set; }
    public bool AttackDown { get; private set; }
    
    // Slide Down
    public bool SlideAttackDown { get; private set; }
    
    // Dash
    public bool DashDown { get; private set; }
    private int lastMoveDir = 1;
    
    // UI / QuickSlot
    public bool InventoryToggleDown { get; private set; }

    public bool QuickSlot1Down { get; private set; }
    public bool QuickSlot2Down { get; private set; }
    public bool QuickSlot3Down { get; private set; }

    public bool GamePlayInputEnabled { get; set; } = true;  // 이동 / 공격 허용
    public bool UIInputEnabled { get; set; } = true;        // UI 허용

    public void Update()
    {
        ClearFrameInputs();

        if (GamePlayInputEnabled)
        {
            MoveX = Input.GetAxisRaw("Horizontal");
            MoveY = Input.GetAxisRaw("Vertical");

            if (MoveX > 0.01f)
                lastMoveDir = 1;
            else if (MoveX < -0.01f)
                lastMoveDir = -1;

            JumpDown = Input.GetKeyDown(KeyCode.Space);
            AttackDown = Input.GetKeyDown(KeyCode.Z);
            SlideAttackDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);

            DashDown = Input.GetKeyDown(KeyCode.C);
        }
        else
        {
            MoveX = 0f;
            MoveY = 0f;
        }

        if (UIInputEnabled)
        {
            if (Input.GetKeyDown(KeyCode.I))
                InventoryToggleDown = true;

            if (Input.GetKeyDown(KeyCode.Alpha1))
                QuickSlot1Down = true;
            if (Input.GetKeyDown(KeyCode.Alpha2))
                QuickSlot2Down = true;
            if (Input.GetKeyDown(KeyCode.Alpha3))
                QuickSlot3Down = true;
        }
    }

    private void ClearFrameInputs()
    {
        JumpDown = false;
        AttackDown = false;
        SlideAttackDown = false;

        DashDown = false;
        InventoryToggleDown = false;
        
        QuickSlot1Down = false;
        QuickSlot2Down = false;
        QuickSlot3Down = false;
    }

    public void SetEnable(bool enable)
    {
        GamePlayInputEnabled = enable ? true : false;
    }
}