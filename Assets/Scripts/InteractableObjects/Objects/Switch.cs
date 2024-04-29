using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
    public UnityEvent OnSwitchToggle; // Event to trigger when the switch is toggled
    private bool isClosed = false; // Determines if the switch is closed (circuit is complete)
    public Node StartNode; // Node where the switch starts
    public Node EndNode; // Node where the switch ends
    public string SwitchName = "Default Switch"; // Optional name for the switch

    private void OnMouseDown() // Toggle the switch when clicked
    {
        ToggleSwitch(); // Call the toggle function
    }

    public void ToggleSwitch()
    {
        isClosed = !isClosed; // Toggle the switch state
        Debug.Log($"Switch {SwitchName} is now {(isClosed ? "closed" : "open")}.");

        // Trigger the event when the switch is toggled
        OnSwitchToggle?.Invoke();

        if (isClosed)
        {
            ConnectNodes(); // Connect nodes when the switch is closed
        }
        else
        {
            DisconnectNodes(); // Disconnect nodes when the switch is open
        }
    }

    public bool IsClosed()
    {
        return isClosed; // Returns whether the switch is closed
    }

    // Connect the nodes to complete the circuit
    private void ConnectNodes()
    {
        if (StartNode != null && EndNode != null)
        {
            // Call CircuitManager to manage the connection
            CircuitManager.Instance.AddConnection(StartNode, EndNode);
            Debug.Log($"Switch {SwitchName}: Connection established between {StartNode.NodeName} and {EndNode.NodeName}.");
        }
        else
        {
            Debug.LogError("Switch connection failed: StartNode or EndNode is missing.");
        }
    }

    // Disconnect the nodes to break the circuit
    private void DisconnectNodes()
    {
        if (StartNode != null && EndNode != null)
        {
            // Call CircuitManager to remove the connection
            CircuitManager.Instance.RemoveConnection(StartNode, EndNode);
            Debug.Log($"Switch {SwitchName}: Connection removed between {StartNode.NodeName} and {EndNode.NodeName}.");
        }
        else
        {
            Debug.LogError("Switch disconnection failed: StartNode or EndNode is missing.");
        }
    }
}
