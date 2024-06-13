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

    public List<NodeConnection> connections = new(); // Manage all connections in the circuit
    public List<Node> nodes = new(); // Manage all nodes in the circuit
    public List<Switch> switches = new(); // Manage all switches in the circuit
    public List<LED> leds = new(); // Manage all LEDs in the circuit
    public List<Potentiometer> potentiometers = new(); // Manage all potentiometers in the circuit
    public List<Battery> batteries = new(); // Manage all batteries in the circuit
    public string circuitName; // Optional identifier for the circuit

    #endregion Variables

    private void Awake()
    {
        InitializeInstance();
        InitializeEvents();
        InitializeLEDs();
        InitializePotentiometers();
        InitializeBatteries();
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
        foreach (var led in leds)
        {
            led.OnIntensityChanged.AddListener(UpdateCircuit); // Listen for intensity changes
        }

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

    private void InitializeBatteries()
    {
        batteries = new List<Battery>(FindObjectsOfType<Battery>());
        // Debug.Log(batteries.Count == 0 ? "No batteries found in the scene." : $"{batteries.Count} batteries found and added to the list.");

        foreach (var battery in batteries)
        {
            AddBatteryToCircuit(battery);
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
        AddPotentiometerConnections();
    }

    #region Adding connections
    private void AddBatteryConnection()
    {
        foreach (var battery in batteries)
        {
            if (battery.positiveTerminal != null && battery.negativeTerminal != null)
            {
                AddConnection(battery.positiveTerminal, battery.negativeTerminal);
            }
        }
    }

    private void AddLEDConnections()
    {
        // Create a copy of the list to avoid modifying the collection while iterating
        var tempLEDs = new List<LED>(leds);

        foreach (LED led in tempLEDs)
        {
            if (!leds.Contains(led)) // Ensure the LED is not already added
            {
                AddLEDToCircuit(led);
            }
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

    private void AddPotentiometerConnections()
    {
        var tempPotentiometers = new List<Potentiometer>(potentiometers);

        foreach (Potentiometer potentiometer in tempPotentiometers)
        {
            if (!potentiometers.Contains(potentiometer)) // Ensure the potentiometer is not already added
            {
                AddPotentiometerToCircuit(potentiometer);
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
        if (!leds.Contains(led))
        {
            leds.Add(led);
            led.OnIntensityChanged.AddListener(UpdateCircuit);
        }

        if (led.GetPositiveTerminal() != null && led.GetNegativeTerminal() != null)
        {
            AddConnection(led.GetPositiveTerminal(), led.GetNegativeTerminal());
            if (!nodes.Contains(led.GetPositiveTerminal())) nodes.Add(led.GetPositiveTerminal());
            if (!nodes.Contains(led.GetNegativeTerminal())) nodes.Add(led.GetNegativeTerminal());
            OnCircuitUpdated.Invoke();
        }
    }

    public void RemoveLEDFromCircuit(LED led)
    {
        if (leds.Contains(led))
        {
            leds.Remove(led);
            led.OnIntensityChanged.RemoveListener(UpdateCircuit);
        }
    }

    // Method to add a new potentiometer
    public void AddPotentiometer(Potentiometer potentiometer)
    {
        if (!potentiometers.Contains(potentiometer))
        {
            potentiometers.Add(potentiometer);
            potentiometer.OnResistanceChanged.AddListener(UpdateCircuit); // Listen for resistance changes
            AddPotentiometerToCircuit(potentiometer);
            OnCircuitUpdated.Invoke();
        }
    }

    private void AddPotentiometerToCircuit(Potentiometer potentiometer)
    {
        if (potentiometer.GetPositiveTerminal() != null && potentiometer.GetVariableTerminal() != null)
        {
            AddConnection(potentiometer.GetPositiveTerminal(), potentiometer.GetVariableTerminal());
            if (!nodes.Contains(potentiometer.GetPositiveTerminal())) nodes.Add(potentiometer.GetPositiveTerminal());
            if (!nodes.Contains(potentiometer.GetVariableTerminal())) nodes.Add(potentiometer.GetVariableTerminal());
        }
        if (potentiometer.GetPositiveTerminal() != null && potentiometer.GetMaxTerminal() != null)
        {
            AddConnection(potentiometer.GetPositiveTerminal(), potentiometer.GetMaxTerminal());
            if (!nodes.Contains(potentiometer.GetPositiveTerminal())) nodes.Add(potentiometer.GetPositiveTerminal());
            if (!nodes.Contains(potentiometer.GetMaxTerminal())) nodes.Add(potentiometer.GetMaxTerminal());
        }
    }

    // Method to add a new battery
    public void AddBatteryToCircuit(Battery battery)
    {
        if (!batteries.Contains(battery))
        {
            batteries.Add(battery);
            AddConnection(battery.positiveTerminal, battery.negativeTerminal);
            foreach (var node in battery.connections)
            {
                if (!nodes.Contains(node))
                {
                    nodes.Add(node);
                }
                node.IsPowered = true;
            }
        }
        OnCircuitUpdated.Invoke();
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

        float batteryVoltage = 0f;
        foreach (var battery in batteries)
        {
            batteryVoltage += battery.GetVoltage();
        }
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
    private bool lastCircuitState = false; // Store the last known state of the circuit

    public bool IsCircuitClosed()
    {
        var visitedNodes = new HashSet<Node>();
        var parentNodes = new Dictionary<Node, Node>();
        var nodeQueue = new Queue<Node>();

        // Find all powered nodes
        var poweredNodes = nodes.FindAll(n => n.IsPowered);
        if (poweredNodes.Count == 0)
        {
            if (lastCircuitState != false)
            {
                Debug.LogWarning("No powered node found. Circuit cannot be closed.");
                lastCircuitState = false;
            }
            return false;
        }

        foreach (var startNode in poweredNodes)
        {
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
                    else if (parentNodes.ContainsKey(currentNode) && parentNodes[currentNode] != connectedNode)
                    {
                        if (lastCircuitState != true)
                        {
                            Debug.Log("Circuit is closed.");
                            lastCircuitState = true;
                        }
                        return true; // A loop is detected
                    }
                }
            }

            // Clear visited nodes and queue for the next start node
            visitedNodes.Clear();
            parentNodes.Clear();
            nodeQueue.Clear();
        }

        if (lastCircuitState != false)
        {
            Debug.Log("Circuit is not closed.");
            lastCircuitState = false;
        }
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
