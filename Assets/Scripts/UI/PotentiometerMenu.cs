using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PotentiometerMenu : MonoBehaviour
{
    public GameObject mainCanvas;

    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Slider resistanceSlider;
    [SerializeField] private TMP_Text resistanceValueText;
    [SerializeField] private Button confirmButton;
    private FirstPersonCam firstPersonCam;


    private Potentiometer currentPotentiometer;
    public static bool isMenuOpen = false;

    private void Start()
    {
        if (mainCanvas == null)
        {
            mainCanvas = GameObject.Find("MainCanvas");
        }
        if (mainCanvas != null) mainCanvas.SetActive(true);

        // Dynamically assign the firstPersonCam reference if it is not set
        if (firstPersonCam == null)
        {
            firstPersonCam = Camera.main.GetComponent<FirstPersonCam>();
        }

        // Set the slider's min and max values
        resistanceSlider.minValue = 1f;
        resistanceSlider.maxValue = 10000f;

        resistanceSlider.onValueChanged.AddListener(OnSliderValueChanged);
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        menuPanel.SetActive(false);
    }

    public void OpenMenu(Potentiometer potentiometer)
    {
        if (mainCanvas != null) mainCanvas.SetActive(false); // Hide the main canvas

        currentPotentiometer = potentiometer;
        resistanceSlider.value = potentiometer.Resistance;
        resistanceValueText.text = potentiometer.Resistance.ToString("F2");
        menuPanel.SetActive(true);
        UnlockCursorAndDisableCamera();
        isMenuOpen = true;
    }

    public void CloseMenu()
    {
        if (mainCanvas != null) mainCanvas.SetActive(true); // Show the main canvas
        menuPanel.SetActive(false);
        LockCursorAndEnableCamera();
        isMenuOpen = false;
    }

    private void OnSliderValueChanged(float value)
    {
        resistanceValueText.text = value.ToString("F2");
        if (currentPotentiometer != null)
        {
            currentPotentiometer.Resistance = value;
            currentPotentiometer.OnResistanceChanged.Invoke();
        }
    }

    private void OnConfirmButtonClick()
    {
        CloseMenu();
    }

    private void UnlockCursorAndDisableCamera()
    {
        // Disable camera movement
        if (firstPersonCam != null)
        {
            firstPersonCam.enabled = false;
        }

        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LockCursorAndEnableCamera()
    {
        // Enable camera movement
        if (firstPersonCam != null)
        {
            firstPersonCam.enabled = true;
        }

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
