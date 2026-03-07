public class SaveData
{
    public string lastSceneName;
    public int[] quickSlotItemIds;
    public int[] inventoryItemIds;

    public SaveData()
    {
        lastSceneName = "Stage1";
        quickSlotItemIds = new int[0];
        inventoryItemIds = new int[0];
    }
}