using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Supabase;
using Supabase.Gotrue;
using Postgrest.Models;
using Postgrest.Attributes;
using com.example;  // Add this line to reference the SupabaseManager namespace

[Serializable]
public class InventorySaveData
{
    public List<SlotSaveData> slots = new List<SlotSaveData>();
}

[Serializable]
public class SlotSaveData
{
    public string itemName;
    public int quantity;
}

[Table("games")]
public class InventoryDataModel : BaseModel
{
    [PrimaryKey("id", false)]
    public int Id { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("filename")]
    public string Filename { get; set; }

    [Column("file")]
    public string File { get; set; }
}


public class SaveLoadManager : MonoBehaviour
{
    private string savePath;
    private List<ItemClass> allItems;

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        LoadAllItems();
    }

    private void LoadAllItems()
    {
        allItems = new List<ItemClass>();
        ItemClass[] loadedItems = Resources.LoadAll<ItemClass>("Inventory");
        foreach (var item in loadedItems)
        {
            allItems.Add(item);
        }
        Debug.Log($"Loaded {allItems.Count} items from Resources/Inventory");
    }

    public async void SaveGame(InventoryManager inventoryManager)
    {
        InventorySaveData inventorySaveData = new InventorySaveData();

        foreach (var slot in inventoryManager.GetItems())
        {
            if (slot.GetItem() != null)
            {
                SlotSaveData slotData = new SlotSaveData
                {
                    itemName = slot.GetItem().ItemName,
                    quantity = slot.GetQuantity()
                };
                inventorySaveData.slots.Add(slotData);
            }
        }

        string json = JsonUtility.ToJson(inventorySaveData, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game Saved to {savePath}");

        // Upload the save data to Supabase
        await UploadSaveDataToSupabase(json);
    }

    public void LoadGame(InventoryManager inventoryManager)
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            InventorySaveData inventorySaveData = JsonUtility.FromJson<InventorySaveData>(json);

            inventoryManager.ClearInventory();

            foreach (var slotData in inventorySaveData.slots)
            {
                ItemClass item = FindItemByName(slotData.itemName);
                if (item != null)
                {
                    inventoryManager.Add(item, slotData.quantity);
                }
                else
                {
                    Debug.LogWarning($"Item '{slotData.itemName}' not found");
                }
            }

            inventoryManager.RefreshUI();
            Debug.Log("Game Loaded");
        }
        else
        {
            Debug.LogWarning($"No save file found at {savePath}");
        }
    }

    private ItemClass FindItemByName(string itemName)
    {
        foreach (ItemClass item in allItems)
        {
            if (item.ItemName == itemName)
                return item;
        }
        return null;
    }

    private async Task UploadSaveDataToSupabase(string jsonData)
    {
        var model = new InventoryDataModel
        {
            CreatedAt = DateTime.UtcNow,
            Filename = "savegame.json",
            File = jsonData
        };

        try
        {
            var response = await SupabaseManager.Instance.Supabase().From<InventoryDataModel>().Insert(model);
            if (response.Models != null && response.Models.Count > 0)
            {
                Debug.Log("Upload complete!");
            }
            else
            {
                Debug.LogError("Failed to upload data to Supabase.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error uploading data to Supabase: {e.Message}");
        }
    }
}
