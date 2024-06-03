using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public string NodeName;
    public bool IsPowered { get; set; }
    public List<Node> ConnectedNodes = new List<Node>();

    void Start()
    {
        IsPowered = false; // Default to not powered
    }

    public void AddConnection(Node node)
    {
        if (!ConnectedNodes.Contains(node))
        {
            ConnectedNodes.Add(node);
            node.ConnectedNodes.Add(this); // Ensure bi-directional connection
            Debug.Log($"Connection added between {this.NodeName} and {node.NodeName}");
        }
    }

    public void RemoveConnection(Node node)
    {
        if (node != null && ConnectedNodes.Contains(node))
        {
            ConnectedNodes.Remove(node);
            node.ConnectedNodes.Remove(this); // Ensure bi-directional disconnection
            Debug.Log($"Connection removed between {this.NodeName} and {node.NodeName}");
        }
    }
}