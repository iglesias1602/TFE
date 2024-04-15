using UnityEngine;

public class InGameWireBuilder : MonoBehaviour
{
    public InGameWireController wireController;
    public InventoryManager inventoryManager; // Reference to your inventory manager

    void Update()
    {
        HandleWireBuilding();
    }

    void HandleWireBuilding()
    {
        // Check if the black cable is currently selected
        if (inventoryManager.selectedItem != null && inventoryManager.selectedItem.name == "Black Cable")
        {
            if (Input.GetMouseButtonDown(1)) // Right-click to set the starting point
            {
                SetWirePoint(true);
            }
            else if (Input.GetMouseButtonDown(0)) // Left-click to set the ending point
            {
                SetWirePoint(false);
            }
        }
    }

    void SetWirePoint(bool isStartPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (isStartPoint)
            {
                wireController.SetStart(hit.point);
            }
            else
            {
                wireController.SetEnd(hit.point);
            }
        }
    }
}
