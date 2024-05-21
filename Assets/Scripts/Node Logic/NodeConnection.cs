using UnityEngine;

public class NodeConnection
{
    public Node Node1 { get; private set; }
    public Node Node2 { get; private set; }
    public Cable ConnectionCable { get; private set; }

    public NodeConnection(Node node1, Node node2, Cable cable = null)
    {
        Node1 = node1;
        Node2 = node2;
        ConnectionCable = cable;

        if (ConnectionCable != null)
        {
            ConnectionCable.Connect(Node1, Node2); // Establish the cable's connection
        }
    }

    public void Disconnect()
    {
        if (Node1 == null || Node2 == null) return;

        if (ConnectionCable != null)
        {
            ConnectionCable.Disconnect(); // Disconnect the cable
        }

        Debug.Log($"NodeConnection: Disconnected {Node1.NodeName} and {Node2.NodeName}.");
    }
}
