using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new tool class", menuName="Item/Tool")]
public class ToolClass : ItemClass
{
    [Header("Tool")] // data specific to tool class
    public ToolType toolType;
    public enum ToolType
    {
        cable, led, resistance, capacitor, interruptor, potentiometer, battery
    }
    public override ItemClass GetItem() { return this; }
    public override ToolClass GetTool() { return this; }
    public override MiscClass GetMisc() { return null; }
    public override ConsumableClass GetConsumable() { return null; }
}
