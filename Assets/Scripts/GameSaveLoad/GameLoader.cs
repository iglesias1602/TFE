using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SaveLoad
{
    public class GameLoader : MonoBehaviour
    {
        private List<ItemClass> allItems;  // This will store all loaded items
        private string savePath;

        private void Start()
        {
            savePath = SaveFileManager.Instance.SaveFilePath;
            savePath = Path.GetFullPath(savePath).Replace("\\", "/");  // Ensure path consistency
            LoadAllItems();
            Debug.Log($"Save path is set to: {savePath}");
            StartCoroutine(Initialize());
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

        public System.Collections.IEnumerator Initialize()
        {
            yield return null;  // Wait for one frame to ensure the scene is fully loaded
            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager != null)
            {
                LoadGame(inventoryManager, savePath);
            }
            else
            {
                Debug.LogWarning("InventoryManager not found.");
            }
        }

        public void LoadGame(InventoryManager inventoryManager, string savePath)
        {
            Debug.Log("LoadGame method called.");
            if (File.Exists(savePath))
            {
                Debug.Log($"Loading game from: {savePath}");
                string json = File.ReadAllText(savePath);
                InventorySaveData inventorySaveData = JsonUtility.FromJson<InventorySaveData>(json);

                Debug.Log("InventorySaveData loaded from JSON:");
                foreach (var slotData in inventorySaveData.slots)
                {
                    Debug.Log($"Item: {slotData.itemName}, Quantity: {slotData.quantity}");
                }

                Debug.Log("Calling ClearInventory.");
                inventoryManager.ClearInventory();

                Debug.Log("Adding items to the inventory.");
                foreach (var slotData in inventorySaveData.slots)
                {
                    ItemClass item = FindItemByName(slotData.itemName);
                    if (item != null)
                    {
                        inventoryManager.Add(item, slotData.quantity);
                        Debug.Log($"Added item: {item.ItemName}, Quantity: {slotData.quantity}");
                    }
                    else
                    {
                        Debug.LogWarning($"Item '{slotData.itemName}' not found");
                    }
                }

                Debug.Log("Refreshing inventory UI.");
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
}
