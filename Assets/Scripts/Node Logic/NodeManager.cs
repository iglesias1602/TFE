using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static List<Node> Nodes = new List<Node>(); // Holds all nodes in the circuit

    void Start()
    {
        // Initialize node list once at start
        RefreshNodes();
    }

    public void RefreshNodes()
    {
        Nodes = new List<Node>(FindObjectsOfType<Node>());
        Debug.Log("Nodes refreshed.");
    }

    public static Node FindNodeByGameObject(GameObject gameObject)
    {
        return gameObject?.GetComponent<Node>();
    }

    // Method to be called when nodes are connected or disconnected
    public void UpdateCircuit()
    {
        CircuitManager.Instance.UpdateCircuit();
    }
}
