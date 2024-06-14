using UnityEngine;
using UnityEngine.Events;

public class LED : MonoBehaviour
{
    [SerializeField] private Light pointLight; // Reference to the Light component
    [SerializeField] private float defaultIntensity = 1f; // Default light intensity
    [SerializeField] private bool isOn = false; // Track whether the LED is on
    [SerializeField] private Node positiveTerminal; // Positive terminal of the LED
    [SerializeField] private Node negativeTerminal; // Negative terminal of the LED

    public UnityEvent OnIntensityChanged; // Event triggered when intensity changes

    public Node GetPositiveTerminal() => positiveTerminal;
    public Node GetNegativeTerminal() => negativeTerminal;

    private void Awake()
    {
        SetupLightComponent();
        SetupTerminals();
        UpdateLightState();

        if (OnIntensityChanged == null)
        {
            OnIntensityChanged = new UnityEvent();
        }
    }

    private void Update()
    {
        UpdateLightState();
    }

    public void TurnOn()
    {
        isOn = true;
        // Debug.Log($"LED {gameObject.name} turned on.");
    }

    public void TurnOff()
    {
        isOn = false;
        // Debug.Log($"LED {gameObject.name} turned off.");
    }

    public void AdjustIntensity(float intensity)
    {
        if (pointLight != null)
        {
            pointLight.intensity = Mathf.Clamp(intensity, 0f, defaultIntensity);
        }
    }

    public void SetDefaultIntensity(float intensity)
    {
        defaultIntensity = intensity;
        if (isOn)
        {
            AdjustIntensity(intensity);
        }
    }

    private void UpdateLightState()
    {
        if (pointLight == null) return;

        pointLight.enabled = isOn;
        if (isOn)
        {
            pointLight.intensity = defaultIntensity;
        }
    }

    #region Setting up

    private void SetupLightComponent()
    {
        pointLight = GetComponentInChildren<Light>();
        if (pointLight == null)
        {
            Debug.LogError($"Light component is missing in {gameObject.name}");
        }
    }

    private void SetupTerminals()
    {
        if (positiveTerminal != null && negativeTerminal != null)
        {
            positiveTerminal.AddConnection(negativeTerminal);
            //Debug.Log($"LED {gameObject.name} terminals connected: Positive to Negative");
        }
        else
        {
            Debug.LogError($"LED {gameObject.name} terminals are not assigned");
        }
    }
    #endregion Setting up
}
