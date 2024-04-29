using UnityEngine;

public class Battery : MonoBehaviour
{
    public float Voltage = 9.0f; // Default voltage for the battery
    public Node PositiveNode; // Node representing the positive terminal
    public Node NegativeNode; // Node representing the negative terminal
    public string BatteryName = "Default Battery"; // Optional name for the battery

    void Start()
    {
        // Ensure nodes are set up correctly
        if (PositiveNode == null || NegativeNode == null)
        {
            Debug.LogError("Battery must have both Positive and Negative nodes assigned.");
        }
        else
        {
            // Logic to establish connections to the circuit
            NodeManager.Nodes.Add(PositiveNode);
            NodeManager.Nodes.Add(NegativeNode);

            Debug.Log($"Battery {BatteryName} connected to the circuit.");
            Debug.Log($"Battery {BatteryName} initialized with {Voltage}V.");
        }
    }

    // Connect the battery to the circuit
    public void ConnectToCircuit()
    {
        // This can establish connections when the battery is activated
        if (PositiveNode != null && NegativeNode != null)
        {
            CircuitManager.Instance.AddConnection(PositiveNode, NegativeNode); // Connect battery terminals
            Debug.Log($"Battery {BatteryName}: Connected terminals.");
        }
    }

    // Disconnect the battery from the circuit
    public void DisconnectFromCircuit()
    {
        if (PositiveNode != null && NegativeNode != null)
        {
            CircuitManager.Instance.RemoveConnection(PositiveNode, NegativeNode); // Disconnect battery terminals
            Debug.Log($"Battery {BatteryName}: Disconnected terminals.");
        }
    }
    public float GetVoltage()
    {
        return Voltage; // Return the battery's voltage
    }
}
