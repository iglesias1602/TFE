using UnityEngine;
using SpiceSharp;
using SpiceSharp.Components;

public class SpiceSharpTest : MonoBehaviour
{
    void Start()
    {
        // Attempt to create a simple circuit
        var ckt = new Circuit();
        var resistor = new Resistor("R1", "0", "1", 1000);

        ckt.Add(resistor);

        Debug.Log("SpiceSharp integration is successful!");
    }
}
