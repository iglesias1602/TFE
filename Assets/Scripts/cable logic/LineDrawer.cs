using UnityEngine;

public class LineDrawer : MonoBehaviour
{
    public InventoryManager inventoryManager; // Assign in the inspector

    public GameObject lineRendererPrefab; // Assign your LineRenderer prefab here in the inspector

    public Material blackCableMaterial; // Assign in the inspector
    public Material redCableMaterial; // Assign in the inspector

    private Camera mainCamera;
    private bool isDrawing = false;
    private bool isSettingStartPoint = true;
    private GameObject currentLineObject;
    private LineRenderer currentLineRenderer;

    private ToolClass currentCableTool = null; // Track the currently selected cable tool

    // Delegate and event definition for line drawing completion
    public delegate void LineDrawnHandler(Vector3 start, Vector3 end);
    public event LineDrawnHandler OnLineDrawn;


    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        ToolClass tool = inventoryManager.selectedItem as ToolClass;

        // First, check for null to prevent unnecessary processing
        if (tool == null)
        {
            if (isDrawing)
            {
                CancelDrawing();
            }
            return; // Exit early if no tool is selected
        }

        if (tool == null && Input.GetMouseButtonDown(1)) // Right mouse button pressed
        {
            TryDeleteCable();
        }

        // Check if we've switched away from a cable tool after starting a drawing,
        // or switched between different cable types (e.g., from red to black).
        if (isDrawing && (!tool.toolType.Equals(ToolClass.ToolType.cable) ||
            (currentCableTool != null && !tool.ItemName.Equals(currentCableTool.ItemName, System.StringComparison.OrdinalIgnoreCase))))
        {
            CancelDrawing(); // Cancel the current drawing if the tool type is not cable or if the cable type has changed
                             // Note: No return here to allow for re-assignment of currentCableTool below if the user switches to another cable
        }

        // Proceed only if we have a cable tool selected
        if (tool.toolType == ToolClass.ToolType.cable)
        {
            // Update the current tool being used for drawing if it's a different tool
            if (currentCableTool == null || !tool.ItemName.Equals(currentCableTool.ItemName, System.StringComparison.OrdinalIgnoreCase))
            {
                currentCableTool = tool;
            }

            if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
            {
                SetLinePoint(tool.ItemName.ToLower());
            }
            else if (!isSettingStartPoint && currentLineRenderer != null)
            {
                UpdateLineEndPoint();
            }
            // Check for right-click to cancel drawing
            else if (Input.GetMouseButtonDown(1))
            {
                CancelDrawing();
            }
        }
        else
        {
            isDrawing = false;
        }
    }

    void SetLinePoint(string toolName)
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (isSettingStartPoint)
            {
                // Instantiate a new LineRenderer object
                currentLineObject = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
                currentLineRenderer = currentLineObject.GetComponent<LineRenderer>();

                // Set material based on toolName
                SetMaterialBasedOnToolName(currentLineRenderer, toolName);

                // First click sets the start point
                Vector3 startPoint = hit.point;
                isSettingStartPoint = false;
                isDrawing = true;
                currentLineRenderer.positionCount = 2;
                currentLineRenderer.SetPosition(0, startPoint);
            }
            else
            {
                // Second click sets the end point and finalizes the line
                currentLineRenderer.SetPosition(1, hit.point);
                OnLineDrawn?.Invoke(currentLineRenderer.GetPosition(0), hit.point); // Trigger the event
                isSettingStartPoint = true; // Ready for the next line
                isDrawing = false; // Stop drawing
                currentLineRenderer = null; // Clear current LineRenderer
            }
        }
    }

    void UpdateLineEndPoint()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            currentLineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            currentLineRenderer.SetPosition(1, ray.origin + ray.direction * 100);
        }
    }

    void SetMaterialBasedOnToolName(LineRenderer lineRenderer, string toolName)
    {
        // Assuming the tool names for the black and red cables are exactly "black-cable" and "red-cable"
        if (toolName.Equals("black cable", System.StringComparison.OrdinalIgnoreCase))
        {
            lineRenderer.material = blackCableMaterial;
        }
        else if (toolName.Equals("red cable", System.StringComparison.OrdinalIgnoreCase))
        {
            lineRenderer.material = redCableMaterial;
        }
        else
        {
            Debug.LogWarning($"No specific material found for toolName: {toolName}, using default.");
            // Here you can decide to set a default material or just leave the LineRenderer with its current material.
        }
    }

    void CancelDrawing()
    {
        if (currentLineObject != null)
        {
            Destroy(currentLineObject); // Remove the incomplete line
        }
        isDrawing = false;
        isSettingStartPoint = true;
        currentLineRenderer = null;
        currentCableTool = null; // Reset the current tool reference
    }

    void TryDeleteCable()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100)) // Adjust the max distance as needed
        {
            if (hit.collider.CompareTag("Cable")) // Check if the hit object is tagged as a "Cable"
            {
                Destroy(hit.collider.gameObject); // Remove the cable object
            }
        }

    }
}
