using UnityEngine;

public class Switch : MonoBehaviour
{
    public GameObject lightSource; // Reference to the GameObject that contains the Light component

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject) // Check if clicking on this switch
            {
                CheckAndToggleLight();
            }
        }
    }

    private void CheckAndToggleLight()
    {
        Cable[] cables = FindObjectsOfType<Cable>();

        foreach (var cable in cables)
        {
            bool isLedPositiveConnected = IsTerminalConnected(cable.startObject, "PositiveTerminal") || IsTerminalConnected(cable.endObject, "PositiveTerminal");
            bool isLedNegativeConnected = IsTerminalConnected(cable.startObject, "NegativeTerminal") || IsTerminalConnected(cable.endObject, "NegativeTerminal");
            bool isSwitchPositiveConnected = IsTerminalConnected(cable.startObject, "PositiveTerminal", true) || IsTerminalConnected(cable.endObject, "PositiveTerminal", true);

            if ((isLedPositiveConnected || isLedNegativeConnected) && isSwitchPositiveConnected)
            {
                // Get the Light component and toggle it
                Light light = lightSource.GetComponent<Light>();
                if (light != null)
                {
                    light.enabled = !light.enabled;
                }
                break;
            }
        }
    }

    private bool IsTerminalConnected(GameObject obj, string terminalName, bool isSwitch = false)
    {
        if (obj == null) return false;

        // If we're checking for the switch's terminal, compare directly to the gameObject
        if (isSwitch && obj == gameObject) return true;

        // Otherwise, check the name of the parent GameObject
        return obj.name == terminalName || (obj.transform.parent != null && obj.transform.parent.name == terminalName);
    }
}
