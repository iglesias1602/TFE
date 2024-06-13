using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static List<Node> Nodes = new List<Node>(); // Holds all nodes in the circuit
    public static List<LED> LEDs = new List<LED>(); // Holds all LEDs in the circuit

    void Start()
    {
        // Initialize node and LED lists once at start
        RefreshNodes();
        RefreshLEDs();
    }

    public void RefreshNodes()
    {
        Nodes = new List<Node>(FindObjectsOfType<Node>());
        Debug.Log("Nodes refreshed.");
    }

    public void RefreshLEDs()
    {
        LEDs = new List<LED>(FindObjectsOfType<LED>());
        Debug.Log("LEDs refreshed.");
    }

    public static void AddNode(Node node)
    {
        if (!Nodes.Contains(node))
        {
            Nodes.Add(node);
            Debug.Log($"Node {node.name} added. Total nodes: {Nodes.Count}");
            CircuitManager.Instance.nodes.Add(node);
            CircuitManager.Instance.OnCircuitUpdated.Invoke();
        }
    }

    public static void RemoveNode(Node node)
    {
        if (Nodes.Contains(node))
        {
            Nodes.Remove(node);
            Debug.Log($"Node {node.name} removed. Total nodes: {Nodes.Count}");
            CircuitManager.Instance.nodes.Remove(node);
            CircuitManager.Instance.OnCircuitUpdated.Invoke();
        }
    }

    public static void AddLED(LED led)
    {
        if (!LEDs.Contains(led))
        {
            LEDs.Add(led);
            Debug.Log($"LED {led.name} added. Total LEDs: {LEDs.Count}");
            CircuitManager.Instance.leds.Add(led);
            CircuitManager.Instance.OnCircuitUpdated.Invoke();
        }
    }

    public static void RemoveLED(LED led)
    {
        if (LEDs.Contains(led))
        {
            LEDs.Remove(led);
            Debug.Log($"LED {led.name} removed. Total LEDs: {LEDs.Count}");
            CircuitManager.Instance.leds.Remove(led);
            CircuitManager.Instance.OnCircuitUpdated.Invoke();
        }
    }

    public static Node FindNodeByGameObject(GameObject gameObject)
    {
        return gameObject?.GetComponent<Node>();
    }

    public void UpdateCircuit()
    {
        CircuitManager.Instance.UpdateCircuit();
    }
}
