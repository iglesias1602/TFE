using UnityEngine;

public class LED : MonoBehaviour
{
    [SerializeField] private Light pointLight; // Reference to the Light component, can be assigned in the editor
    [SerializeField] private float defaultIntensity = 4f; // Default light intensity, adjustable in editor
    [SerializeField] private bool isOn = false; // Track whether the LED is on
    [SerializeField] private Node positiveTerminal; // Positive terminal of the LED
    [SerializeField] private Node negativeTerminal; // Negative terminal of the LED
    public Node GetPositiveTerminal()
    {
        return positiveTerminal;
    }

    public Node GetNegativeTerminal()
    {
        return negativeTerminal;
    }

    private void Awake()
    {
        // Validate and configure the LED setup
        SetupLightComponent();
        SetupTerminals();
        UpdateLightState();
    }

    private void Update()
    {
        // Update the light state based on `isOn`
        UpdateLightState();
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
        if (pointLight != null)
        {
            pointLight.intensity = Mathf.Clamp(intensity, 0f, defaultIntensity); // Adjust intensity safely
        }
    }

    private void SetupLightComponent()
    {
        // Ensure there is a Light component
        pointLight = GetComponentInChildren<Light>();
        if (pointLight == null)
        {
            Debug.LogError("Light component is missing in " + gameObject.name);
        }
    }

    private void SetupTerminals()
    {
        // Ensure terminals are properly connected
        if (positiveTerminal != null && negativeTerminal != null)
        {
            positiveTerminal.AddConnection(negativeTerminal);
            Debug.Log("LED terminals connected: Positive to Negative");
        }
        else
        {
            Debug.LogError("LED terminals are not assigned in " + gameObject.name);
        }
    }

    private void UpdateLightState()
    {
        if (pointLight == null) return;

        if (isOn && !pointLight.enabled)
        {
            pointLight.enabled = true;
            pointLight.intensity = defaultIntensity;
            Debug.Log("LED turned on.");
        }
        else if (!isOn && pointLight.enabled)
        {
            pointLight.enabled = false;
            Debug.Log("LED turned off.");
        }
    }
}
