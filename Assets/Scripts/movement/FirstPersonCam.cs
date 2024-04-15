using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    float attachedDistance = 0f;

    [SerializeField] private Vector3 attachedObjectOffset = new Vector3(0, -0.5f, 1);


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    // Update is called once per frame
    private void Update()
    {
        MouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        Body.Rotate(Vector3.up, MouseX);

        MouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
        Angle -= MouseY;
        Angle = Mathf.Clamp(Angle, -30f, 80f);
        Head.localRotation = Quaternion.Euler(Angle, 0, 0);

        // Picking objects
        RaycastHit hit;
        bool cast = Physics.Raycast(Head.position, Head.forward, out hit, itemPickupDistance);

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

        // Left-click logic for connecting
        if (Input.GetMouseButtonDown(0) && attachedObject != null)
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
                        Debug.Log("Attached object is tagged as EndAnchor.");

                        Rigidbody endAnchorRB = attachedObject.GetComponent<Rigidbody>();
                        if (endAnchorRB != null)
                        {
                            Debug.Log("Rigidbody found on attached object. Attempting to connect.");
                            plugController.Connect(attachedObject, endAnchorRB);
                        }
                        else
                        {
                            Debug.Log("No Rigidbody found on the EndAnchor object.");
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

    }
}
