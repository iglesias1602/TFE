using UnityEngine;

public class TubeDrawer3D : MonoBehaviour
{
    public InventoryManager inventoryManager; // Assign in the inspector

    public GameObject tubeRendererPrefab; // Prefab with TubeRenderer attached

    public Material blackCableMaterial; // Assign in the inspector
    public Material redCableMaterial; // Assign in the inspector

    public bool meshColliderEnabled = true; // This will create a checkbox in the inspector

    private Camera mainCamera;
    private bool isDrawing = false;
    private bool isSettingStartPoint = true;
    private GameObject currentTubeObject;
    private TubeRenderer currentTubeRenderer;
    private Vector3 startPoint;
    private ToolClass currentCableTool = null; // Track the currently selected cable tool


    private Vector3 lastEndPoint;


    void Start()
    {
        SetMeshColliderState(meshColliderEnabled);
        mainCamera = Camera.main;
    }
    void Update()
    {
        ToolClass tool = inventoryManager.selectedItem as ToolClass;

        // Check if the tool is a cable and we're not currently drawing
        if (tool != null && tool.toolType == ToolClass.ToolType.cable && !isDrawing)
        {
            if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
            {
                StartDrawing(tool.ItemName.ToLower());
            }
        }
        // If we are currently drawing, set the end point
        else if (isDrawing && Input.GetMouseButtonDown(0))
        {
            SetEndPoint();
        }

        if (!isSettingStartPoint && isDrawing)
        {
            UpdateCablePreview();
        }

        // Cancel drawing on right-click or destroy cable if no item is selected
        if (Input.GetMouseButtonDown(1))
        {
            if (isDrawing || !isSettingStartPoint) // If in the middle of drawing, cancel it
            {
                CancelDrawing();
            }
            else if (inventoryManager.selectedItem == null) // If no item is selected, attempt to destroy a cable
            {
                TryDestroyCable();
            }
        }
    }

    void TryDestroyCable()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Define a layer mask for the "Cable" layer. Adjust the layer number to match what you've set.
        int layerMask = LayerMask.GetMask("Cable");

        // Use the layerMask in the Raycast
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            // With the layer mask, this check might be redundant, but it's left here as an example.
            if (hit.collider.gameObject.CompareTag("Cable"))
            {
                Destroy(hit.collider.gameObject); // Destroy the cable object.
            }
        }
    }



    void StartDrawing(string toolName)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Create a layer mask for the "Interactable" layer
        int layerMask = 1 << LayerMask.NameToLayer("Interactable");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) // Check if the ray hits an object on the "Interactable" layer
        {
            startPoint = hit.point;
            GameObject hitObject = hit.collider.gameObject; // Capture the hit object
            // Proceed to create the cable object if the hit object is interactable
            CreateTubeObject(toolName, startPoint, hitObject);
            isSettingStartPoint = false;
        }
    }

    void SetEndPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // Use the same layer mask as in StartDrawing
        int layerMask = 1 << LayerMask.NameToLayer("Interactable");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask)) // Check again for the "Interactable" layer
        {
            Vector3 endPoint = hit.point;
            GameObject hitObject = hit.collider.gameObject; // Capture the hit object


            if (currentTubeRenderer != null && !isSettingStartPoint) // Valid end position on an interactable object
            {
                Cable cableComponent = currentTubeObject.GetComponent<Cable>();
                if (cableComponent != null)
                {
                    cableComponent.endObject = hitObject; // Now set the end object reference
                }

                currentTubeRenderer.SetPositions(new Vector3[] { startPoint, endPoint });
                FinalizeDrawing();
            }
        }
    }


    void CreateTubeObject(string toolName, Vector3 position, GameObject startObject)
    {
        currentTubeObject = Instantiate(tubeRendererPrefab, Vector3.zero, Quaternion.identity);
        currentTubeRenderer = currentTubeObject.GetComponent<TubeRenderer>();

        Cable cableComponent = currentTubeObject.GetComponent<Cable>();
        if (cableComponent != null)
        {
            cableComponent.startObject = startObject; // Set the start object reference here
        }

        SetMaterialBasedOnToolName(toolName);
        // Initially set both start and "end" point to the same to visualize the starting point
        currentTubeRenderer.SetPositions(new Vector3[] { position, position });
        isDrawing = true;
    }

    void UpdateCablePreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.yellow, 1f);

        int layerMask = LayerMask.GetMask("Ground", "Interactable");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {

                // Update the preview endpoint to the hit point
                Vector3 previewEndPoint = hit.point;
                if (currentTubeRenderer != null)
                {
                    currentTubeRenderer.SetPositions(new Vector3[] { startPoint, previewEndPoint });
                }
        }
        else
        {
            Debug.Log("Raycast did not hit any object on the specified layers.");

        }

        // Debug visual
        Debug.DrawLine(ray.origin, hit.point, Color.red); // Visualize the raycast hit point
    }

    void FinalizeDrawing()
    {
        isDrawing = false;
        isSettingStartPoint = true; // Ready for a new drawing
        currentTubeRenderer = null; // Clear current TubeRenderer for the next operation
    }

    void CancelDrawing()
    {
        if (currentTubeObject != null)
        {
            Destroy(currentTubeObject); // Remove the incomplete tube
        }
        ResetDrawingState();
    }

    void ResetDrawingState()
    {
        isDrawing = false;
        isSettingStartPoint = true;
        currentTubeRenderer = null;
        currentTubeObject = null;
        currentCableTool = null;
    }

    void SetMaterialBasedOnToolName(string toolName)
    {
        if (currentTubeRenderer == null) return;

        Material selectedMaterial = toolName.Contains("black") ? blackCableMaterial : redCableMaterial;
        currentTubeRenderer.material = selectedMaterial;
    }

    void TryDeleteCable()
    {
        // Implement deletion logic, if needed
    }

    public void SetMeshColliderState(bool enabled)
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.enabled = enabled;
        }
    }
}