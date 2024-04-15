using UnityEngine;

public class InGameWireController : MonoBehaviour
{
    public Transform startAnchorPrefab;
    public Transform endAnchorPrefab;
    public TubeRenderer tubeRenderer; // Assume this has a method SetPositions(Vector3[] positions)

    private Vector3 startAnchorPosition;
    private Vector3 endAnchorPosition;
    private bool startSet = false;

    public void SetStart(Vector3 position)
    {
        startAnchorPosition = position;
        Instantiate(startAnchorPrefab, position, Quaternion.identity); // Visual feedback for start position
        startSet = true;
    }

    public void SetEnd(Vector3 position)
    {
        if (!startSet) return;

        endAnchorPosition = position;
        Instantiate(endAnchorPrefab, position, Quaternion.identity); // Visual feedback for end position
        RenderWire();
        startSet = false; // Reset for the next wire
    }

    void RenderWire()
    {
        if (tubeRenderer != null)
        {
            // Update the TubeRenderer with the new positions
            tubeRenderer.SetPositions(new Vector3[] { startAnchorPosition, endAnchorPosition });
        }
        else
        {
            Debug.LogError("TubeRenderer reference not set.");
        }
    }
}
