using UnityEngine;

public class HighlightOnRaycast : MonoBehaviour
{
    private Renderer lastHighlighted = null;

    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Connectable") &&
                (hit.collider.gameObject.name == "PositiveTerminal" || hit.collider.gameObject.name == "NegativeTerminal"))
            {
                // Find the child object named "highlighted" within the hit object
                Transform highlightedChild = hit.collider.transform.Find("Highlighted");
                if (highlightedChild)
                {
                    Renderer renderer = highlightedChild.GetComponent<Renderer>();
                    if (lastHighlighted != renderer)
                    {
                        ResetLastHighlighted();
                        renderer.enabled = true;
                        lastHighlighted = renderer;
                    }
                }
            }
            else
            {
                ResetLastHighlighted();
            }
        }
        else
        {
            ResetLastHighlighted();
        }
    }

    void ResetLastHighlighted()
    {
        if (lastHighlighted != null)
        {
            lastHighlighted.enabled = false;
            lastHighlighted = null;
        }
    }
}
