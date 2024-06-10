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
        leds = new List<LED>(FindObjectsOfType<LED>());
        if (leds.Count == 0)
        {
            Debug.LogError("No LEDs found in the scene.");
        }
        else
        {
            Debug.Log(leds.Count + " LEDs found and added to the list.");
        }

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
        InitializeConnections(); // Set up connections in the circuit
        InitializeNodes(); // Set up nodes in the circuit
        OnNewConnection.AddListener(UpdateCircuit); // Update circuit when a new connection is made
        OnConnectionRemoved.AddListener(UpdateCircuit); // Update circuit when a connection is removed
        OnNewConnection.AddListener(OnNewConnectionHandler); // Listen for new connections
        OnConnectionRemoved.AddListener(OnConnectionRemovedHandler); // Listen for connection removals
    }


    private void InitializeConnections()
    {
        // Manually set up connections for the battery
        if (battery != null && battery.positiveTerminal != null && battery.negativeTerminal != null)
        {
            AddConnection(battery.positiveTerminal, battery.negativeTerminal);
        }

        // Manually set up connections for LEDs
        foreach (LED led in leds)
        {
            if (led.GetPositiveTerminal() != null && led.GetNegativeTerminal() != null)
            {
                AddConnection(led.GetPositiveTerminal(), led.GetNegativeTerminal());
            }
        }

        // Manually set up connections for switches
        foreach (Switch sw in switches)
        {
            if (sw.node1 != null && sw.node2 != null && sw.isClosed)
            {
                AddConnection(sw.node1, sw.node2);
            }
        }
    }

    private void InitializeNodes()
{
    // Clear existing nodes
    nodes.Clear();

    // Find all nodes in the scene and add them to the list
    Node[] allNodes = FindObjectsOfType<Node>();
    foreach (Node node in allNodes)
    {
        nodes.Add(node);
        Debug.Log($"Node added: {node.NodeName}, IsPowered: {node.IsPowered}");
    }
}

    private void OnNewConnectionHandler()
    {
        Debug.Log("New connection event triggered.");
    }

    private void OnConnectionRemovedHandler()
    {
        Debug.Log("Connection removal event triggered.");
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

        Debug.Log($"Node {node1.NodeName} now connected to: {string.Join(", ", node1.ConnectedNodes)}");
        Debug.Log($"Node {node2.NodeName} now connected to: {string.Join(", ", node2.ConnectedNodes)}");
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
        var nodeQueue = new Queue<Node>();  // Utilisez une file d'attente au lieu d'une pile

        // Trouver un noeud alimenté comme point de départ
        var startNode = nodes.Find(n => n.IsPowered);

        if (startNode == null)  // Pas de point de départ, pas de circuit
        {
            Debug.LogWarning("No powered node found. Circuit cannot be closed.");
            return false;
        }

        nodeQueue.Enqueue(startNode);
        Debug.Log($"Starting BFS from powered node: {startNode.NodeName}");

        while (nodeQueue.Count > 0)  // Parcourir les connexions
        {
            var currentNode = nodeQueue.Dequeue();
            Debug.Log($"Visiting node: {currentNode.NodeName}");

            if (!visitedNodes.Add(currentNode))  // Détecter une boucle
            {
                Debug.Log("Circuit is closed.");
                return true;
            }

            // Parcourir les noeuds connectés
            foreach (var connectedNode in currentNode.ConnectedNodes)
            {
                if (!visitedNodes.Contains(connectedNode))
                {
                    nodeQueue.Enqueue(connectedNode);
                    Debug.Log($"Adding node to check: {connectedNode.NodeName}");
                }
            }
        }

        Debug.Log("Circuit is not closed.");
        return false;
    }

    // Check if an LED is connected in the circuit
    public bool IsLEDConnected(LED led)
    {
        Node positive = led.GetPositiveTerminal();
        Node negative = led.GetNegativeTerminal();

        // Check if both terminals are not null and there exists a connection between them
        if (positive != null && negative != null)
        {
            return connections.Exists(c =>
                (c.Node1 == positive && c.Node2 == negative) ||
                (c.Node1 == negative && c.Node2 == positive));
        }
        return false;
    }

    // Turn on all LEDs that are part of a closed circuit
    private void TurnOnLEDs()
    {
        if (IsCircuitClosed())  // Only proceed if the circuit is closed
        {
            foreach (var led in leds)
            {
                if (led != null && IsLEDConnected(led))
                {
                    led.TurnOn(); // Only turn on the LED if it's connected in a closed circuit
                }
            }
        }
    }

    // Turn off all LEDs
    private void TurnOffLEDs()
    {
        foreach (var led in leds)
        {
            if (led != null)
            {
                led.TurnOff(); // Always turn off LEDs if the circuit is open
            }
        }
    }
}