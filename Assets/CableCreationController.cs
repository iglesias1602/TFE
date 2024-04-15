using UnityEngine;

public class CableCreationController : MonoBehaviour
{
    public LineDrawer lineDrawer3D; // Assign in the inspector
    public TubeRenderer tubeRenderer; // Assign your TubeRenderer component here

    void Start()
    {
        if (lineDrawer3D != null)
        {
            lineDrawer3D.OnLineDrawn += HandleLineDrawn;
        }
    }

    private void HandleLineDrawn(Vector3 start, Vector3 end)
    {
        Vector3[] positions = new Vector3[] { start, end };
        tubeRenderer.SetPositions(positions); // Assuming SetPositions is a method in your TubeRenderer that takes an array of Vector3
    }

    void OnDestroy()
    {
        // Clean up the event subscription
        if (lineDrawer3D != null)
        {
            lineDrawer3D.OnLineDrawn -= HandleLineDrawn;
        }
    }
}
