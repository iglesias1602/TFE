using UnityEngine;
using UnityEngine.UI;

public class FirstPersonCam : MonoBehaviour
{
    public static FirstPersonCam Instance;
    public float MouseX { get; private set; }
    public float MouseY { get; private set; }

    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [SerializeField] private Transform body;
    [SerializeField] private Transform head;

    private float angle;

    [SerializeField] private float itemPickupDistance;
    private Transform attachedObject = null;
    [SerializeField] private Vector3 attachedObjectOffset = new(0, -0.5f, 1);

    [SerializeField] private float raycastDistance = 5.0f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private Text objectNameText;

    [SerializeField] private InventoryManager inventoryManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        objectNameText.enabled = false;

        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager is not assigned in FirstPersonCam. Please assign it in the Inspector.");
        }
    }

    private void Update()
    {
        HandleInventoryOpen();
        HandlePotentiometerMenuOpen();
        HandleMouseLook();
        HandleRaycast();

        if (Input.GetMouseButtonDown(1))
        {
            HandleRightClick();
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }
    }

    private void HandleMouseLook()
    {
        MouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        body.Rotate(Vector3.up, MouseX);

        MouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
        angle -= MouseY;
        angle = Mathf.Clamp(angle, -30f, 80f);
        head.localRotation = Quaternion.Euler(angle, 0, 0);
    }

    private void HandleRaycast()
    {
        Ray ray = new(head.position, head.forward);
        bool hitInteractable = Physics.Raycast(ray, out RaycastHit hit, raycastDistance, interactableLayer);

        if (hitInteractable)
        {
            DisplayObjectName(hit.transform);
        }
        else
        {
            objectNameText.enabled = false;
        }
    }

    private void DisplayObjectName(Transform hitTransform)
    {
        string objectName = hitTransform.name;
        string parentName = hitTransform.parent != null ? hitTransform.parent.name : null;

        objectNameText.enabled = true;
        objectNameText.text = parentName != null ? $"{objectName} ({parentName})" : objectName;
    }

    private void HandleInventoryOpen()
    {
        if (InventoryManager.IsInventoryOpen)
        {
            return;
        }
    }

    private void HandlePotentiometerMenuOpen()
    {
        if (PotentiometerMenu.isMenuOpen)
        {
            return;
        }
    }

    private void HandleRightClick()
    {
        bool cast = Physics.Raycast(head.position, head.forward, out RaycastHit hit, itemPickupDistance);

        if (attachedObject == null)
        {
            TryPickUpObject(hit, cast);
        }
        else
        {
            DropObject();
        }
    }

    private void TryPickUpObject(RaycastHit hit, bool cast)
    {
        if (cast && hit.transform.CompareTag("pickable"))
        {
            attachedObject = hit.transform;
            AttachObjectToHead();
        }
        else
        {
            TryDisconnectAndPickUp(hit);
        }
    }

    private void TryDisconnectAndPickUp(RaycastHit hit)
    {
        PlugController plugController = hit.collider?.GetComponent<PlugController>();

        if (plugController != null && plugController.isConnected)
        {
            Transform endAnchor = plugController.endAnchor;
            plugController.Disconnect();
            attachedObject = endAnchor;
            AttachObjectToHead();
        }
    }

    private void AttachObjectToHead()
    {
        attachedObject.SetParent(head);
        attachedObject.localPosition = attachedObjectOffset;

        Rigidbody attachedObjectRb = attachedObject.GetComponent<Rigidbody>();
        if (attachedObjectRb != null)
        {
            attachedObjectRb.isKinematic = true;
        }

        Collider attachedObjectCollider = attachedObject.GetComponent<Collider>();
        if (attachedObjectCollider != null)
        {
            attachedObjectCollider.enabled = false;
        }
    }

    private void DropObject()
    {
        attachedObject.SetParent(null);

        Rigidbody attachedObjectRb = attachedObject.GetComponent<Rigidbody>();
        if (attachedObjectRb != null)
        {
            attachedObjectRb.isKinematic = false;
        }

        Collider attachedObjectCollider = attachedObject.GetComponent<Collider>();
        if (attachedObjectCollider != null)
        {
            attachedObjectCollider.enabled = true;
        }

        attachedObject = null;
    }

    private void HandleLeftClick()
    {
        if (attachedObject != null)
        {
            TryConnectObject();
        }
        else
        {
            if (Physics.Raycast(head.position, head.forward, out RaycastHit hit, itemPickupDistance))
            {
                Potentiometer potentiometer = hit.collider.GetComponent<Potentiometer>();
                if (potentiometer != null)
                {
                    potentiometer.PotentiometerClick();
                    return;
                }
            }
            UseSelectedHotbarItem();
        }
    }

    private void TryConnectObject()
    {
        if (Physics.Raycast(head.position, head.forward, out RaycastHit hit, itemPickupDistance))
        {
            PlugController plugController = hit.collider?.GetComponent<PlugController>();

            if (plugController != null && attachedObject.CompareTag("pickable"))
            {
                Rigidbody endAnchorRb = attachedObject.GetComponent<Rigidbody>();
                if (endAnchorRb != null)
                {
                    plugController.Connect(attachedObject, endAnchorRb);
                }
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
            InstantiateSelectedItem(selectedItem);
        }
    }

    private void InstantiateSelectedItem(ItemClass selectedItem)
    {
        GameObject newObject = Instantiate(selectedItem.itemPrefab, GetSpawnPosition(), Quaternion.identity);
        //Debug.Log($"New object instantiated: {newObject.name}");

        Node newNode = newObject.GetComponent<Node>();
        if (newNode != null)
        {
            AddNodeToCircuit(newNode);
        }

        LED newLED = newObject.GetComponentInChildren<LED>();
        if (newLED != null)
        {
            CircuitManager.Instance.AddLEDToCircuit(newLED);
        }

        Potentiometer newPotentiometer = newObject.GetComponent<Potentiometer>();
        if (newPotentiometer != null)
        {
            CircuitManager.Instance.AddPotentiometer(newPotentiometer);
        }

        Battery newBattery = newObject.GetComponentInChildren<Battery>();
        if (newBattery != null)
        {
            CircuitManager.Instance.AddBatteryToCircuit(newBattery);
        }

        CircuitManager.Instance.UpdateCircuit();
        Debug.Log("Circuit updated.");
    }

    private void AddNodeToCircuit(Node newNode)
    {
        if (!NodeManager.Nodes.Contains(newNode))
        {
            NodeManager.AddNode(newNode);
            CircuitManager.Instance.nodes.Add(newNode);
            Debug.Log($"Node {newNode.name} added to the circuit.");
        }
    }

    /*private void AddLEDToCircuit(LED newLED)
    {
        if (!CircuitManager.Instance.leds.Contains(newLED))
        {
            CircuitManager.Instance.leds.Add(newLED);
            Debug.Log($"LED {newLED.name} added to the circuit.");

            AddLEDTerminalsToCircuit(newLED);
        }
    }

    private void AddLEDTerminalsToCircuit(LED newLED)
    {
        if (newLED.GetPositiveTerminal() != null && newLED.GetNegativeTerminal() != null)
        {
            AddNodeToCircuit(newLED.GetPositiveTerminal());
            AddNodeToCircuit(newLED.GetNegativeTerminal());
            CircuitManager.Instance.AddConnection(newLED.GetPositiveTerminal(), newLED.GetNegativeTerminal());
        }
        else
        {
            Debug.LogError($"LED {newLED.name} terminals are not properly assigned.");
        }
    }*/

    private Vector3 GetSpawnPosition()
    {
        return head.position + head.forward * 2.0f;
    }

    public bool IsHoldingItem()
    {
        return attachedObject != null;
    }
}
