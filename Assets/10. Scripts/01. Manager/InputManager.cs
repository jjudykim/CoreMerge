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
    private const float doubleTapThreshold = 0.25f;
    public bool DashRightDown { get; private set; }
    public bool DashLeftDown { get; private set; }
    private float lastTapTime = 0f;
    private int lastTapDir = 0;
    
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

            JumpDown = Input.GetKeyDown(KeyCode.Space);
            AttackDown = Input.GetKeyDown(KeyCode.F);
            SlideAttackDown = Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow);
            
            DetectDoubleTapDash();
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

    private void DetectDoubleTapDash()
    {
        float now = Time.time;
        int tapDir = 0;
        
        bool rightKeyDown = Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);
        bool leftKeyDown = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow);

        if (rightKeyDown) tapDir = 1;
        else if (leftKeyDown) tapDir = -1;

        if (tapDir == 0)
            return;

        if (lastTapDir == tapDir && now - lastTapTime <= doubleTapThreshold)
        {
            if (tapDir > 0)
                DashRightDown = true;
            else
                DashLeftDown = true;
        }

        lastTapDir = tapDir;
        lastTapTime = now;
    }

    private void ClearFrameInputs()
    {
        JumpDown = false;
        AttackDown = false;
        SlideAttackDown = false;

        InventoryToggleDown = false;
        
        QuickSlot1Down = false;
        QuickSlot2Down = false;
        QuickSlot3Down = false;
        
        DashRightDown = false;
        DashLeftDown  = false;
    }
}