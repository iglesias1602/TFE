using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonCam : MonoBehaviour
{
    public float MouseX;
    public float MouseY;

    [SerializeField] float sensX;
    [SerializeField] float sensY;

    public Transform Body;
    public Transform Head;

    public float Angle;

    public float itemPickupDistance;
    Transform attachedObject = null;
    //float attachedDistance = 0f;

    [SerializeField] private Vector3 attachedObjectOffset = new Vector3(0, -0.5f, 1);

    [SerializeField] private float raycastDistance = 5.0f;
    [SerializeField] private LayerMask interactableLayer; // Set in Unity Editor to define which objects are interactable
    [SerializeField] private Text objectNameText; // Assign the Text component for displaying object names

    [SerializeField] private InventoryManager inventoryManager; // Reference to the InventoryManager

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        objectNameText.enabled = false;

        // Check if inventoryManager is assigned
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is not assigned in FirstPersonCam. Please assign it in the Inspector.");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        MouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        Body.Rotate(Vector3.up, MouseX);

        MouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
        Angle -= MouseY;
        Angle = Mathf.Clamp(Angle, -30f, 80f);
        Head.localRotation = Quaternion.Euler(Angle, 0, 0);

        Ray ray = new Ray(Head.position, Head.forward);
        // Picking objects
        RaycastHit hit;
        bool cast = Physics.Raycast(Head.position, Head.forward, out hit, itemPickupDistance);

        if (Physics.Raycast(ray, out hit, raycastDistance, interactableLayer))
        {
            // Get the transform of the object hit by the raycast
            Transform hitTransform = hit.transform;

            // Get the object's name
            string objectName = hitTransform.name;

            // Get the parent object's name if it exists
            string parentName = hitTransform.parent != null ? hitTransform.parent.name : null;

            objectNameText.enabled = true;
            objectNameText.text = parentName != null ? $"{objectName} ({parentName})" : objectName;
        }
        else
        {
            // Hide the text if there's no raycast hit
            objectNameText.enabled = false;
        }

        if (InventoryManager.IsInventoryOpen)
        {
            // Optionally, you might still want to show the cursor or update UI elements here
            return; // Skip the rest of the update if the inventory is open
        }

        if (Input.GetMouseButtonDown(1)) // Right-click to pick up or drop objects
        {
            if (attachedObject == null)
            {
                // Attempt to pick up or disconnect and pick up.
                if (Physics.Raycast(Head.position, Head.forward, out hit, itemPickupDistance))
                {
                    PlugController plugController = hit.collider.GetComponent<PlugController>();

                    // Check if hitting a connected plug to disconnect and pick up.
                    if (plugController != null && plugController.isConnected)
                    {
                        Debug.Log("Disconnecting and picking up the connected object.");
                        Transform endAnchor = plugController.endAnchor;

                        plugController.Disconnect();

                        attachedObject = endAnchor;
                        attachedObject.SetParent(Head);
                        attachedObject.localPosition = new Vector3(0, -0.5f, 1);
                        if (attachedObject.GetComponent<Rigidbody>() != null)
                            attachedObject.GetComponent<Rigidbody>().isKinematic = true;
                        if (attachedObject.GetComponent<Collider>() != null)
                            attachedObject.GetComponent<Collider>().enabled = false;
                    }
                }
            }
            else
            {
                // Check to ensure we don't immediately drop what we just picked up.
                // If there's an attached object, drop it.
                Debug.Log("Dropping object.");
                attachedObject.SetParent(null);
                if (attachedObject.GetComponent<Rigidbody>() != null)
                    attachedObject.GetComponent<Rigidbody>().isKinematic = false;
                if (attachedObject.GetComponent<Collider>() != null)
                    attachedObject.GetComponent<Collider>().enabled = true;

                attachedObject = null;
            }

            if (attachedObject != null)
            {
                // Drop logic...
                attachedObject.SetParent(null);
                if (attachedObject.GetComponent<Rigidbody>() != null)
                    attachedObject.GetComponent<Rigidbody>().isKinematic = false;
                if (attachedObject.GetComponent<Collider>() != null)
                    attachedObject.GetComponent<Collider>().enabled = true;

                attachedObject = null;
            }
            else
            {
                // Pick up logic...
                if (cast && hit.transform.CompareTag("pickable"))
                {
                    attachedObject = hit.transform;
                    attachedObject.SetParent(Head); // Attach to Head instead of Body for better alignment

                    // Set a specific local position that doesn't block the view/center.
                    attachedObject.localPosition = attachedObjectOffset;

                    // Pick up logic...
                    if (attachedObject.GetComponent<Rigidbody>() != null)
                        attachedObject.GetComponent<Rigidbody>().isKinematic = true;
                    if (attachedObject.GetComponent<Collider>() != null)
                        attachedObject.GetComponent<Collider>().enabled = false;
                }
            }
        }

        // Left-click logic for connecting or using hotbar items
        if (Input.GetMouseButtonDown(0))
        {
            // Check if there's an attached object for connecting logic
            if (attachedObject != null)
            {
                Debug.Log("Left-click detected with an attached object.");

                if (Physics.Raycast(Head.position, Head.forward, out hit, itemPickupDistance))
                {
                    Debug.Log("Raycast hit: " + hit.collider.name);

                    PlugController plugController = hit.collider.GetComponent<PlugController>();
                    if (plugController != null)
                    {
                        Debug.Log("PlugController found on " + hit.collider.name);

                        if (attachedObject.CompareTag("pickable"))
                        {
                            Debug.Log("Attached object is tagged as pickable.");

                            Rigidbody endAnchorRB = attachedObject.GetComponent<Rigidbody>();
                            if (endAnchorRB != null)
                            {
                                Debug.Log("Rigidbody found on attached object. Attempting to connect.");
                                plugController.Connect(attachedObject, endAnchorRB);
                            }
                            else
                            {
                                Debug.Log("No Rigidbody found on the pickable object.");
                            }
                        }
                        else
                        {
                            Debug.Log("Attached object is not tagged as pickable.");
                        }
                    }
                    else
                    {
                        Debug.Log("No PlugController found on hit object.");
                    }
                }
                else
                {
                    Debug.Log("Raycast did not hit any object.");
                }
            }
            else
            {
                // Use the selected hotbar item if no object is attached
                UseSelectedHotbarItem();
            }
        }
    }

    private void UseSelectedHotbarItem()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is not assigned.");
            return;
        }

        ItemClass selectedItem = inventoryManager.selectedItem;
        if (selectedItem != null && selectedItem.itemPrefab != null)
        {
            // Instantiate the selected item's prefab
            GameObject newObject = Instantiate(selectedItem.itemPrefab, GetSpawnPosition(), Quaternion.identity);
            Debug.Log($"New object instantiated: {newObject.name}");

            // Add the new object to NodeManager if it's a node or LED
            Node newNode = newObject.GetComponent<Node>();
            if (newNode != null)
            {
                if (!NodeManager.Nodes.Contains(newNode))
                {
                    NodeManager.AddNode(newNode);
                    CircuitManager.Instance.nodes.Add(newNode);
                    Debug.Log($"Node {newNode.name} added to the circuit.");
                }
            }

            LED newLED = newObject.GetComponent<LED>();
            if (newLED != null)
            {
                if (!CircuitManager.Instance.leds.Contains(newLED))
                {
                    CircuitManager.Instance.leds.Add(newLED);
                    Debug.Log($"LED {newLED.name} added to the circuit.");

                    if (newLED.GetPositiveTerminal() != null && newLED.GetNegativeTerminal() != null)
                    {
                        if (!NodeManager.Nodes.Contains(newLED.GetPositiveTerminal()))
                        {
                            NodeManager.AddNode(newLED.GetPositiveTerminal());
                            CircuitManager.Instance.nodes.Add(newLED.GetPositiveTerminal());
                            Debug.Log($"Positive terminal of {newLED.name} added to the circuit.");
                        }
                        if (!NodeManager.Nodes.Contains(newLED.GetNegativeTerminal()))
                        {
                            NodeManager.AddNode(newLED.GetNegativeTerminal());
                            CircuitManager.Instance.nodes.Add(newLED.GetNegativeTerminal());
                            Debug.Log($"Negative terminal of {newLED.name} added to the circuit.");
                        }
                        CircuitManager.Instance.AddConnection(newLED.GetPositiveTerminal(), newLED.GetNegativeTerminal());
                    }
                    else
                    {
                        Debug.LogError($"LED {newLED.name} terminals are not properly assigned.");
                    }
                }
            }

            // Update the circuit after adding new nodes and LEDs
            CircuitManager.Instance.UpdateCircuit();
            Debug.Log("Circuit updated.");
        }
        else
        {
            //Debug.LogError("Selected item or item prefab is null.");
        }
    }

    private Vector3 GetSpawnPosition()
    {
        // This example just spawns the item in front of the player
        return Head.position + Head.forward * 2.0f;
    }
}
