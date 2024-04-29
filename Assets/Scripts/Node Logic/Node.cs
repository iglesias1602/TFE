using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public string NodeName; // Optional identifier
    public List<Node> ConnectedNodes = new List<Node>(); // List of connected nodes
    public GameObject AttachedComponent; // Reference to attached components (e.g., LED, Battery, etc.)
    public bool IsPowered { get; set; } // Indicates if the node is connected to a power source

    void Start()
    {
        if (ConnectedNodes == null)
        {
            ConnectedNodes = new List<Node>(); // Initialize the list to avoid null reference
        }
    }

    // Check if this node is connected to another node
    public bool IsConnected(Node otherNode)
    {
        return ConnectedNodes.Contains(otherNode);
    }

    // Utility methods for attaching and detaching components
    public void AttachComponent(GameObject component)
    {
        AttachedComponent = component;
        Debug.Log($"Component {component.name} attached to node {NodeName}");
    }

    public void DetachComponent()
    {
        AttachedComponent = null;
        Debug.Log($"Component detached from node {NodeName}");
    }

    // Utility to print connected nodes for debugging
    public void PrintConnections()
    {
        Debug.Log($"Node {NodeName} is connected to:");
        foreach (var node in ConnectedNodes)
        {
            Debug.Log($"- {node.NodeName}");
        }
    }
}
