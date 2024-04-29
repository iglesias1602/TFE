using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static List<Node> Nodes = new List<Node>(); // Holds all nodes in the circuit
    public static Node GroundNode = null; // Reference to the ground node
    public static Node PositiveNode = null; // Reference to the positive node

    void Start()
    {
        if (CircuitManager.Instance != null)
        {
            CircuitManager.Instance.OnNewConnection.AddListener(CheckNodes); // Subscribe to the event
        }
    }

    private void CheckNodes()
    {
        Nodes = new List<Node>(FindObjectsOfType<Node>()); // Refresh the list of nodes

        GroundNode = Nodes.Find(n => n.NodeName == "Ground");
        PositiveNode = Nodes.Find(n => n.NodeName == "Positive");

        if (GroundNode != null && PositiveNode != null)
        {
            Debug.Log("Circuit nodes found: Ground and Positive initialized.");
        }
        else
        {
            Debug.LogError("Circuit initialization failed: Missing Ground or Positive node.");
        }
    }

    // NodeManager focuses on managing nodes
    public static Node FindNodeByGameObject(GameObject gameObject)
    {
        return gameObject?.GetComponent<Node>(); // Ensure safe retrieval of Node component
    }
}
