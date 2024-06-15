using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        ItemClass[] loadedItems = Resources.LoadAll<ItemClass>("Inventory v2/Items");
        foreach (var item in loadedItems)
        {
            allItems.Add(item);
        }
        Debug.Log($"Loaded {allItems.Count} items from Inventory v2/Items");
    }

    public void SaveGame(InventoryManager inventoryManager)
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
}
