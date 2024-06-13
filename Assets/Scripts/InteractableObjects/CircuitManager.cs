using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class CircuitManager : MonoBehaviour
{
    #region Variables
    public static CircuitManager Instance { get; private set; } // Singleton instance
    public UnityEvent OnNewConnection; // Event to signal a new connection
    public UnityEvent OnConnectionRemoved; // Event to signal connection removal
    public UnityEvent OnCircuitUpdated; // Event for circuit updates

    public List<NodeConnection> connections = new List<NodeConnection>(); // Manage all connections in the circuit
    public List<Node> nodes = new List<Node>(); // Manage all nodes in the circuit
    public List<Switch> switches = new List<Switch>(); // Manage all switches in the circuit
    public List<LED> leds = new List<LED>(); // Manage all LEDs in the circuit
    public List<Potentiometer> potentiometers = new List<Potentiometer>(); // Manage all potentiometers in the circuit
    public string circuitName; // Optional identifier for the circuit
    public Battery battery; // Reference to the battery (if used)

    #endregion Variables

    private void Awake()
    {
        InitializeInstance();
        InitializeEvents();
        InitializeLEDs();
        InitializePotentiometers();
        InitializeNodes();
    }

    private void Start()
    {
        InitializeCircuitComponents();
    }

    #region Initializing components

    private void InitializeInstance()
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
    }

    private void InitializeEvents()
    {
        OnNewConnection ??= new UnityEvent(); // Initialize the event
        OnConnectionRemoved ??= new UnityEvent();
        OnCircuitUpdated ??= new UnityEvent(); // Trigger circuit updates

        // Register listeners for event-driven updates
        OnNewConnection.AddListener(UpdateCircuit);
        OnConnectionRemoved.AddListener(UpdateCircuit);
    }

    private void InitializeLEDs()
    {
        leds = new List<LED>(FindObjectsOfType<LED>());
        // Debug.Log(leds.Count == 0 ? "No LEDs found in the scene." : $"{leds.Count} LEDs found and added to the list.");
    }

    private void InitializePotentiometers()
    {
        potentiometers = new List<Potentiometer>(FindObjectsOfType<Potentiometer>());
        // Debug.Log(potentiometers.Count == 0 ? "No potentiometers found in the scene." : $"{potentiometers.Count} potentiometers found and added to the list.");

        foreach (var potentiometer in potentiometers)
        {
            potentiometer.OnResistanceChanged.AddListener(UpdateCircuit); // Listen for resistance changes
        }
    }

    private void InitializeNodes()
    {
        nodes.Clear();
        foreach (Node node in FindObjectsOfType<Node>())
        {
            nodes.Add(node);
            // Debug.Log($"Node added: {node.NodeName}, IsPowered: {node.IsPowered}");
        }
    }
    #endregion Initializing components

    private void InitializeCircuitComponents()
    {
        InitializeConnections(); // Set up connections in the circuit
        RegisterEventHandlers();
    }



    private void RegisterEventHandlers()
    {
        OnNewConnection.AddListener(OnNewConnectionHandler); // Listen for new connections
        OnConnectionRemoved.AddListener(OnConnectionRemovedHandler); // Listen for connection removals
    }

    private void InitializeConnections()
    {
        AddBatteryConnection();
        AddLEDConnections();
        AddSwitchConnections();
    }

    #region Adding connections
    private void AddBatteryConnection()
    {
        if (battery != null && battery.positiveTerminal != null && battery.negativeTerminal != null)
        {
            AddConnection(battery.positiveTerminal, battery.negativeTerminal);
        }
    }

    private void AddLEDConnections()
    {
        // Create a copy of the list to avoid modifying the collection while iterating
        var ledsCopy = new List<LED>(leds);
        foreach (LED led in ledsCopy)
        {
            AddLEDToCircuit(led);
        }
    }

    private void AddSwitchConnections()
    {
        foreach (Switch sw in switches)
        {
            if (sw.node1 != null && sw.node2 != null && sw.isClosed)
            {
                AddConnection(sw.node1, sw.node2);
            }
        }
    }
    #endregion Adding connections


    private void OnNewConnectionHandler()
    {
       // Debug.Log("New connection event triggered.");
    }

    private void OnConnectionRemovedHandler()
    {
       // Debug.Log("Connection removal event triggered.");
    }

    public void AddLEDToCircuit(LED led)
    {
        if (led.GetPositiveTerminal() != null && led.GetNegativeTerminal() != null)
        {
            AddConnection(led.GetPositiveTerminal(), led.GetNegativeTerminal());
            nodes.Add(led.GetPositiveTerminal());
            nodes.Add(led.GetNegativeTerminal());
            leds.Add(led);
            OnCircuitUpdated.Invoke();
        }
    }

    // Add a new connection and trigger relevant events
    public void AddConnection(Node node1, Node node2)
    {
        if (node1 == null || node2 == null || ConnectionExists(node1, node2)) return;

        var newConnection = new NodeConnection(node1, node2);
        connections.Add(newConnection); // Store the connection
        OnNewConnection.Invoke(); // Trigger event for new connections
        // Debug.Log($"Connection added between {node1.NodeName} and {node2.NodeName}.");

        UpdateConnectedNodes(node1, node2);
    }

    private bool ConnectionExists(Node node1, Node node2)
    {
        return connections.Exists(c =>
            (c.Node1 == node1 && c.Node2 == node2) ||
            (c.Node1 == node2 && c.Node2 == node1));
    }

    private void UpdateConnectedNodes(Node node1, Node node2)
    {
        if (!node1.ConnectedNodes.Contains(node2))
        {
            node1.ConnectedNodes.Add(node2);
        }

        if (!node2.ConnectedNodes.Contains(node1))
        {
            node2.ConnectedNodes.Add(node1);
        }

        // Debug.Log($"Node {node1.NodeName} now connected to: {string.Join(", ", node1.ConnectedNodes)}");
        // Debug.Log($"Node {node2.NodeName} now connected to: {string.Join(", ", node2.ConnectedNodes)}");
    }

    // Remove a connection and trigger relevant events
    public void RemoveConnection(Node node1, Node node2)
    {
        var connection = connections.Find(c =>
            (c.Node1 == node1 && c.Node2 == node2) ||
            (c.Node1 == node2 && c.Node2 == node1));

        if (connection != null)
        {
            connection.Disconnect();
            connections.Remove(connection); // Remove the connection
            UpdateDisconnectedNodes(node1, node2);

            OnConnectionRemoved.Invoke(); // Trigger event for connection removal
            // Debug.Log($"Connection removed between {node1.NodeName} and {node2.NodeName}.");

            UpdateCircuit(); // Update the circuit state after removing connections
        }
    }

    private void UpdateDisconnectedNodes(Node node1, Node node2)
    {
        node1.ConnectedNodes.Remove(node2); // Update node connections
        node2.ConnectedNodes.Remove(node1);
    }

    // Update the circuit based on the current connections
    public void UpdateCircuit()
    {
        if (IsCircuitClosed())
        {
            foreach (var led in leds)
            {
                if (led != null && IsPartOfClosedLoop(led.GetPositiveTerminal(), led.GetNegativeTerminal()))
                {

                    // Find the corresponding potentiometer and adjust LED intensity
                    foreach (var potentiometer in potentiometers)
                    {
                        if (IsPartOfClosedLoop(potentiometer.GetPositiveTerminal(), potentiometer.GetVariableTerminal()))
                        {
                            float intensity = CalculateLEDIntensity(potentiometer.Resistance);
                            led.SetDefaultIntensity(intensity); // Adjust LED default intensity based on potentiometer resistance
                        }
                        else if (IsPartOfClosedLoop(potentiometer.GetPositiveTerminal(), potentiometer.GetMaxTerminal()))
                        {
                            float intensity = CalculateLEDIntensity(potentiometer.FixedResistance);
                            led.SetDefaultIntensity(intensity); // Adjust LED default intensity based on potentiometer resistance
                        }
                    }

                    led.TurnOn();

                }
                else
                {
                    led.TurnOff(); // Turn off LEDs that are not in a closed loop
                }
            }
        }
        else // If the circuit is open, turn off LEDs
        {
            TurnOffLEDs();
        }

        OnCircuitUpdated?.Invoke(); // Invoke the circuit update event (if any)
    }

    private float CalculateLEDIntensity(float resistance)
    {

        float batteryVoltage = battery != null ? battery.GetVoltage() : 9f;

        // Calculate the current using Ohm's Law: I = V / R
        float current = batteryVoltage / resistance;

        // The power delivered to the LED: P = V * I
        float power = batteryVoltage * current;

        // Map the power to LED intensity
        float maxIntensity = 59f;
        float intensity = Mathf.Lerp(maxIntensity, 0f, Mathf.InverseLerp(1f, 10000f, resistance));
        return intensity;
    }

    // Check if the circuit is closed by detecting loops
    public bool IsCircuitClosed()
    {
        var visitedNodes = new HashSet<Node>();
        var parentNodes = new Dictionary<Node, Node>();
        var nodeQueue = new Queue<Node>();

        var startNode = nodes.Find(n => n.IsPowered);
        if (startNode == null)
        {
            // Debug.LogWarning("No powered node found. Circuit cannot be closed.");
            return false;
        }

        nodeQueue.Enqueue(startNode);
        visitedNodes.Add(startNode);

        while (nodeQueue.Count > 0)
        {
            var currentNode = nodeQueue.Dequeue();
            foreach (var connectedNode in currentNode.ConnectedNodes)
            {
                if (!visitedNodes.Contains(connectedNode))
                {
                    visitedNodes.Add(connectedNode);
                    parentNodes[connectedNode] = currentNode;
                    nodeQueue.Enqueue(connectedNode);
                }
                else if (parentNodes[currentNode] != connectedNode)
                {
                    // Debug.Log("Circuit is closed.");
                    return true; // A loop is detected
                }
            }
        }

        // Debug.Log("Circuit is not closed.");
        return false;
    }

    // Check if an LED is connected in the circuit
    public bool IsLEDConnected(LED led)
    {
        Node positive = led.GetPositiveTerminal();
        Node negative = led.GetNegativeTerminal();

        if (positive != null && negative != null)
        {
            return ConnectionExists(positive, negative);
        }
        return false;
    }

    public bool IsPartOfClosedLoop(Node startNode, Node endNode)
    {
        var visitedNodes = new HashSet<Node>();
        var nodeQueue = new Queue<Node>();
        nodeQueue.Enqueue(startNode);
        visitedNodes.Add(startNode);

        while (nodeQueue.Count > 0)
        {
            var currentNode = nodeQueue.Dequeue();
            foreach (var connectedNode in currentNode.ConnectedNodes)
            {
                if (!visitedNodes.Contains(connectedNode))
                {
                    visitedNodes.Add(connectedNode);
                    nodeQueue.Enqueue(connectedNode);
                }
                else if (connectedNode == endNode)
                {
                    return true; // If we reach the end node, it's a closed loop
                }
            }
        }
        return false;
    }

    // Turn off all LEDs
    private void TurnOffLEDs()
    {
        foreach (var led in leds)
        {
            led?.TurnOff(); // Always turn off LEDs if the circuit is open
        }
    }
}
