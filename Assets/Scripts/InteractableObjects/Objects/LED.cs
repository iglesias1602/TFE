using UnityEngine;

public class LED : MonoBehaviour
{
    public Light pointLight; // Reference to the Light component
    public float defaultIntensity = 4f; // Default light intensity
    public bool isOn = false; // Track whether the LED is on

    void Start()
    {
        pointLight = GetComponentInChildren<Light>(); // Get the light component
        if (pointLight == null)
        {
            Debug.LogError("Light component is missing.");
            return;
        }

        //TurnOff(); // Start with the light off
    }

    void Update()
    {
        // Example: Dynamic check to turn the light on or off based on `isOn`
        if (isOn)
        {
            if (!pointLight.enabled) // Avoid unnecessary re-enabling
            {
                pointLight.enabled = true;
                pointLight.intensity = defaultIntensity;
                Debug.Log("LED turned on in Update().");
            }
        }
        else
        {
            if (pointLight.enabled) // Avoid unnecessary re-disabling
            {
                pointLight.enabled = false;
                Debug.Log("LED turned off in Update().");
            }
        }
    }

    public void TurnOn()
    {
        isOn = true; // Set the variable to indicate the light is on
    }

    public void TurnOff()
    {
        isOn = false; // Set the variable to indicate the light is off
    }

    public void AdjustIntensity(float intensity)
    {
        if (pointLight == null) return;

        pointLight.intensity = Mathf.Clamp(intensity, 0f, defaultIntensity); // Adjust intensity
    }
}
