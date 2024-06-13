using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemClass : ScriptableObject
{
    [Header("Item")] // data shared across every item
    public string ItemName;
    public Sprite itemIcon;
    public bool isStackable = true;
    public GameObject itemPrefab;  // Add this line

    public abstract ItemClass GetItem();
    public abstract ToolClass GetTool();
    public abstract MiscClass GetMisc();
    public abstract ConsumableClass GetConsumable();

}
