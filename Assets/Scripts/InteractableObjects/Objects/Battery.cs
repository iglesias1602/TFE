using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public Node positiveTerminal;
    public Node negativeTerminal;
    public List<Node> connections;
    public float voltage = 9f; // Default voltage of the battery

    private void Start()
    {
        connections = new List<Node>(GetComponentsInChildren<Node>());
        foreach (var node in connections)
        {
            node.IsPowered = true; // Set connected nodes as powered
        }

        if (positiveTerminal != null && negativeTerminal != null)
        {
            positiveTerminal.AddConnection(negativeTerminal);
            // Debug.Log("Battery terminals connected: Positive to Negative");
        }
        else
        {
            // Debug.LogError("Battery terminals are not assigned.");
        }
    }

    public bool IsConnected(Node target)
    {
        // Implement connection logic if needed
        return connections.Contains(target);
    }

    public float GetVoltage()
    {
        return voltage;
    }
}
