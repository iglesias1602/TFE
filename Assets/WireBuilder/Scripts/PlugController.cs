using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlugController : MonoBehaviour
{
    public bool isConnected = false;
    public UnityEvent OnWirePlugged;
    public Transform plugPosition;

    [HideInInspector]
    public Transform endAnchor;
    [HideInInspector]
    public Rigidbody endAnchorRB;
    [HideInInspector]
    public WireController wireController;
    public void OnPlugged()
    {
        if (OnWirePlugged != null)
        {
            OnWirePlugged.Invoke();
        }
    }

    public void Connect(Transform endAnchorTransform, Rigidbody endAnchorRigidBody)
    {
        if (!isConnected && endAnchorTransform != null && endAnchorRigidBody != null)
        {
            isConnected = true;
            endAnchorRigidBody.isKinematic = true;
            endAnchorTransform.position = plugPosition.position;
            endAnchorTransform.rotation = transform.rotation;

            OnPlugged();
        }
    }

    public void Disconnect()
    {
        if (isConnected)
        {
            Debug.Log("Disconnecting: " + endAnchor.name);
            isConnected = false;

            // Here you would reset any specific state related to the connection.
            // Note: Don't modify position or kinematic state here since it's being picked up immediately after.

            // Invoke any necessary events for disconnection.
            OnWirePlugged?.Invoke();

            // No clearing of endAnchor; it's being picked up immediately.
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.gameObject == endAnchor.gameObject)
        {
            isConnected = true;
            endAnchorRB.isKinematic = true;
            endAnchor.transform.position = plugPosition.position;
            endAnchor.transform.rotation = transform.rotation;


            OnPlugged();
        }
    }

    private void Update()
    {

        if (isConnected)
        {
            endAnchorRB.isKinematic = true;
            endAnchor.transform.position = plugPosition.position;
            Vector3 eulerRotation = new Vector3(this.transform.eulerAngles.x + 90, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
            endAnchor.transform.rotation = Quaternion.Euler(eulerRotation);
        }
    }
}
