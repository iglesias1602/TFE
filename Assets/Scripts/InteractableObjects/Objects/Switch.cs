using UnityEngine;
using UnityEngine.Events;

public class Switch : MonoBehaviour
{
    public bool isClosed = false; // State of the switch: true if closed, false if open
    public Node node1; // One end of the switch
    public Node node2; // Other end of the switch
    public UnityEvent OnSwitchToggle; // Event for when the switch is toggled

    void Start()
    {
        if (OnSwitchToggle == null)
        {
            OnSwitchToggle = new UnityEvent();
        }

        // Initialize switch state
        UpdateSwitchState();
    }

    private void OnMouseDown() // Toggle the switch when clicked
    {
        ToggleSwitch(); // Call the toggle function
    }


    public void ToggleSwitch()
    {
        isClosed = !isClosed;
        UpdateSwitchState();
        OnSwitchToggle.Invoke();
        Debug.Log($"Switch toggled. New state: {(isClosed ? "Closed" : "Open")}");
    }

    private void UpdateSwitchState()
    {
        if (isClosed)
        {
            node1.AddConnection(node2); // Connect nodes when switch is closed
        }
        else
        {
            node1.RemoveConnection(node2);
        }

        // Log the current connections of node1 and node2 for debugging
        Debug.Log($"Node {node1.NodeName} connections: {string.Join(", ", node1.ConnectedNodes)}");
        Debug.Log($"Node {node2.NodeName} connections: {string.Join(", ", node2.ConnectedNodes)}");
    }
}
