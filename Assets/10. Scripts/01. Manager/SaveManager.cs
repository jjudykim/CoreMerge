using System.IO;
using UnityEngine;

public class SaveManager
{
    private string savePath;
    private const string SAVE_FILE_NAME = "SaveData.json";

    public void Init()
    {
        savePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
    }
    
    public void SaveGame(string currentSceneName)
    {
        SaveData data = new SaveData();

        // 1. 스테이지 정보 저장
        data.lastSceneName = currentSceneName;

        // 2. 퀵슬롯 데이터 추출 (기존의 coreIds 배열 복사)
        var quickSlots = Managers.Instance.QuickSlots;
        data.quickSlotItemIds = new int[quickSlots.Capacity];
        for (int i = 0; i < quickSlots.Capacity; i++)
        {
            data.quickSlotItemIds[i] = quickSlots.GetItemIdAt(i);
        }

        // 3. 인벤토리 데이터 추출
        var inventory = Managers.Instance.CoreInventory;
        data.inventoryItemIds = new int[inventory.Capacity];
        for (int i = 0; i < inventory.Capacity; i++)
        {
            data.inventoryItemIds[i] = inventory.GetItemIdAt(i);
        }

        // JSON 직렬화 및 저장
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);

        Debug.Log($"[Save] Game Saved to {savePath}");
    }
    
    public SaveData LoadGame()
    {
        if (File.Exists(savePath) == false)
        {
            Debug.LogWarning("[Save] No save file found.");
            return null;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // --- 불러온 데이터를 각 매니저에 반영 ---

        // 플레이어 기본 데이터부터 초기화
        Managers.Instance.PlayerData.ResetData();
        
        // 인벤토리 복구
        var inventory = Managers.Instance.CoreInventory;
        inventory.ResetInventory();
        for (int i = 0; i < data.inventoryItemIds.Length; i++)
        {
            inventory.TryAssignAt(i, data.inventoryItemIds[i]);
        }

        // 퀵슬롯 복구
        var quickSlots = Managers.Instance.QuickSlots;
        quickSlots.ClearAllSlots(); // 일단 비우고
        for (int i = 0; i < data.quickSlotItemIds.Length; i++)
        {
            quickSlots.TryAssignAt(i, data.quickSlotItemIds[i]);
        }

        Debug.Log("[Save] Game Data Loaded.");
        return data;
    }

    public void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("[Save] Save file deleted (Game Cleared).");
        }
    }

    /// <summary>
    /// 저장 데이터가 존재하는지 확인
    /// </summary>
    public bool HasSaveFile() => File.Exists(savePath);
}
