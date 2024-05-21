using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public string NodeName;
    public bool IsPowered { get; set; }
    public List<Node> ConnectedNodes;

    void Start()
    {
        ConnectedNodes = new List<Node>();
        IsPowered = false; // Default to not powered
    }

    public void AddConnection(Node node)
    {
        if (!ConnectedNodes.Contains(node))
        {
            ConnectedNodes.Add(node);
            node.ConnectedNodes.Add(this); // Ensure bi-directional connection
        }
    }

    public void RemoveConnection(Node node)
    {
        if (node != null && ConnectedNodes.Contains(node))
        {
            ConnectedNodes.Remove(node);
            node.ConnectedNodes.Remove(this); // Ensure bi-directional disconnection
        }
    }
}