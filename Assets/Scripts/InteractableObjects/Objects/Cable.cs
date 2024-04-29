using UnityEngine;
using UnityEngine.Events;

public class Cable : MonoBehaviour
{
    public Node startTerminal; // Reference to the start node
    public Node endTerminal; // Reference to the end node
    public bool isConnected = false; // Track if the cable is connected
    public string cableType = "standard"; // Type of cable (e.g., power, data)

    public UnityEvent OnConnected; // Event to signal when the cable is connected
    public UnityEvent OnDisconnected; // Event for disconnection

    private void Start()
    {
        if (OnConnected == null) OnConnected = new UnityEvent(); // Initialize events
        if (OnDisconnected == null) OnDisconnected = new UnityEvent();
    }

    public void Connect(Node start, Node end)
    {
        if (start == null || end == null)
        {
            Debug.LogError("Cable connection failed: Start or End node is null.");
            return;
        }

        startTerminal = start;
        endTerminal = end;

        if (!startTerminal.ConnectedNodes.Contains(endTerminal))
        {
            startTerminal.ConnectedNodes.Add(endTerminal); // Update start node connections
        }

        if (!endTerminal.ConnectedNodes.Contains(startTerminal))
        {
            endTerminal.ConnectedNodes.Add(startTerminal); // Update end node connections
        }

        isConnected = true; // Cable is now connected
        OnConnected.Invoke(); // Trigger the connection event
        Debug.Log($"Cable connected from {startTerminal.NodeName} to {endTerminal.NodeName}.");
    }

    public void Disconnect()
    {
        if (startTerminal == null || endTerminal == null)
        {
            Debug.LogError("Cable disconnection failed: Start or End node is null.");
            return;
        }

        if (startTerminal.ConnectedNodes.Contains(endTerminal))
        {
            startTerminal.ConnectedNodes.Remove(endTerminal); // Update start node connections
        }

        if (endTerminal.ConnectedNodes.Contains(startTerminal))
        {
            endTerminal.ConnectedNodes.Remove(startTerminal); // Update end node connections
        }

        isConnected = false; // Cable is now disconnected
        OnDisconnected.Invoke(); // Trigger the disconnection event
        Debug.Log($"Cable disconnected between {startTerminal.NodeName} and {endTerminal.NodeName}.");
    }

    public bool IsConnected()
    {
        return isConnected; // Return the connection state
    }
}
