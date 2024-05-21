using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public List<Node> connections;

    void Start()
    {
        connections = new List<Node>(GetComponentsInChildren<Node>());
        foreach (var node in connections)
        {
            node.IsPowered = true; // Set connected nodes as powered
        }
    }

    public bool IsConnected(Node target)
    {
        // Implement connection logic if needed
        return connections.Contains(target);
    }
}
