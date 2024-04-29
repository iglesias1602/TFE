using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class CircuitManager : MonoBehaviour
{
    public static CircuitManager Instance { get; private set; } // Singleton instance
    public UnityEvent OnNewConnection; // Event to signal a new connection
    public UnityEvent OnConnectionRemoved; // Event to signal connection removal
    public UnityEvent OnCircuitUpdated; // Event for circuit updates

    public List<NodeConnection> connections = new List<NodeConnection>(); // Manage all connections in the circuit
    public List<Node> nodes = new List<Node>(); // Manage all nodes in the circuit
    public List<Switch> switches = new List<Switch>(); // Manage all switches in the circuit
    public List<LED> leds = new List<LED>(); // Manage all LEDs in the circuit
    public string circuitName; // Optional identifier for the circuit
    public Battery battery; // Reference to the battery (if used)

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Preserve across scenes
        }
        else
        {
            Destroy(this.gameObject); // Ensure only one instance
        }

        if (OnNewConnection == null) OnNewConnection = new UnityEvent(); // Initialize the event
        if (OnConnectionRemoved == null) OnConnectionRemoved = new UnityEvent();
        if (OnCircuitUpdated == null) OnCircuitUpdated = new UnityEvent(); // Trigger circuit updates

        // Register listeners for event-driven updates
        OnNewConnection.AddListener(UpdateCircuit);
        OnConnectionRemoved.AddListener(UpdateCircuit);
    }

    private void Start()
    {
        InitializeSwitches(); // Set up switches in the circuit
        InitializeLEDs(); // Set up LEDs in the circuit

        OnNewConnection.AddListener(UpdateCircuit); // Update circuit when a new connection is made
        OnConnectionRemoved.AddListener(UpdateCircuit); // Update circuit when a connection is removed
        OnNewConnection.AddListener(OnNewConnectionHandler); // Listen for new connections
        OnConnectionRemoved.AddListener(OnConnectionRemovedHandler); // Listen for connection removals
    }


    private void OnNewConnectionHandler()
    {
        Debug.Log("New connection event triggered.");
    }

    private void OnConnectionRemovedHandler()
    {
        Debug.Log("Connection removal event triggered.");
    }

    // Initialize and set up switches in the circuit
    private void InitializeSwitches()
    {
        Switch[] foundSwitches = GetComponentsInChildren<Switch>();
        foreach (var sw in foundSwitches)
        {
            if (!switches.Contains(sw))
            {
                switches.Add(sw);
                sw.OnSwitchToggle.AddListener(UpdateCircuit); // Add event listener for circuit updates
            }
        }
    }

    // Initialize and set up LEDs in the circuit
    private void InitializeLEDs()
    {
        LED[] foundLEDs = GetComponentsInChildren<LED>();
        foreach (var led in foundLEDs)
        {
            if (!leds.Contains(led))
            {
                leds.Add(led); // Add LEDs to the list for tracking
            }
        }
    }

    // Add a new connection and trigger relevant events
    public void AddConnection(Node node1, Node node2)
    {
        if (node1 == null || node2 == null) return;

        if (connections.Exists(c =>
            (c.Node1 == node1 && c.Node2 == node2) ||
            (c.Node1 == node2 && c.Node2 == node1)
        )) return; // Avoid duplicate connections

        var newConnection = new NodeConnection(node1, node2);
        connections.Add(newConnection); // Store the connection
        OnNewConnection.Invoke(); // Trigger event for new connections
        Debug.Log($"Connection added between {node1.NodeName} and {node2.NodeName}.");


        // Update node connected lists
        if (!node1.ConnectedNodes.Contains(node2))
        {
            node1.ConnectedNodes.Add(node2);
        }

        if (!node2.ConnectedNodes.Contains(node1))
        {
            node2.ConnectedNodes.Add(node1);
        }
    }

    // Remove a connection and trigger relevant events
    public void RemoveConnection(Node node1, Node node2)
    {
        var connection = connections.Find(c =>
            (c.Node1 == node1 && c.Node2 == node2) ||
            (c.Node1 == node2 && c.Node2 == node1)
        );

        if (connection != null)
        {
            connection.Disconnect();
            connections.Remove(connection); // Remove the connection
            node1.ConnectedNodes.Remove(node2); // Update node connections
            node2.ConnectedNodes.Remove(node1);

            OnConnectionRemoved.Invoke(); // Trigger event for connection removal
            Debug.Log($"Connection removed between {node1.NodeName} and {node2.NodeName}.");

        }

        UpdateCircuit(); // Update the circuit state after removing connections
    }

    // Update the circuit based on the current connections
    public void UpdateCircuit()
    {

        // Check if the circuit is closed
        bool isCircuitClosed = IsCircuitClosed();

        // If the circuit is closed, turn on LEDs
        if (isCircuitClosed)
        {
            TurnOnLEDs();
        }
        else // If the circuit is open, turn off LEDs
        {
            TurnOffLEDs();
        }

        // Invoke the circuit update event (if any)
        OnCircuitUpdated?.Invoke();
    }


    // Check if the circuit is closed by detecting loops
    public bool IsCircuitClosed()
    {
        var visitedNodes = new HashSet<Node>();
        var nodeStack = new Stack<Node>();

        // Find a powered node as the starting point
        var startNode = nodes.Find(n => n.IsPowered);

        if (startNode == null) // No starting point, no circuit
        {
            Debug.LogWarning("No powered node found. Circuit cannot be closed.");
            return false;
        }

        nodeStack.Push(startNode);

        while (nodeStack.Count > 0) // Traverse connections
        {
            var currentNode = nodeStack.Pop();

            if (!visitedNodes.Add(currentNode)) // Detect a loop
            {
                Debug.Log("Circuit is closed.");
                return true;
            }

            // Traverse connected nodes
            foreach (var connectedNode in currentNode.ConnectedNodes)
            {
                if (!visitedNodes.Contains(connectedNode))
                {
                    nodeStack.Push(connectedNode);
                    Debug.Log($"Adding node to check: {connectedNode.NodeName}");

                }
            }
        }

        Debug.Log("Circuit is not closed.");
        return false;
    }


    // Turn on all LEDs in the circuit
    private void TurnOnLEDs()
    {
        foreach (var led in leds)
        {
            if (led != null)
            {
                led.TurnOn(); // Light up when the circuit is closed
            }
        }
    }

    // Turn off all LEDs in the circuit
    private void TurnOffLEDs()
    {
        foreach (var led in leds)
        {
            if (led != null)
            {
                led.TurnOff(); // Turn off when the circuit is open
            }
        }
    }
}
