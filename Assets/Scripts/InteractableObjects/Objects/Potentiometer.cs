using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Potentiometer : MonoBehaviour
{
    [SerializeField] private Node positiveTerminal; // A
    [SerializeField] private Node variableTerminal; // B
    [SerializeField] private Node maxResistanceTerminal; // C
    [SerializeField] private float resistance = 1f; // Potentiometer resistance value
    public float FixedResistance = 10000f;

    public UnityEvent OnResistanceChanged; // Event triggered when resistance changes

    private PotentiometerMenu potentiometerMenu;

    private NodeConnection connectionAB;
    private NodeConnection connectionAC;
    public float Resistance
    {
        get => resistance;
        set
        {
            if (resistance != value)
            {
                resistance = Mathf.Clamp(value, 1f, 10000f); // Assuming 10000 is the max resistance
                OnResistanceChanged.Invoke(); // Trigger the event when resistance 
            }
        }
    }

    public Node GetPositiveTerminal() => positiveTerminal;
    public Node GetVariableTerminal() => variableTerminal;
    public Node GetMaxTerminal() => maxResistanceTerminal;

    private void Awake()
    {
        if (positiveTerminal != null || variableTerminal != null || maxResistanceTerminal != null)
        {
            SetupTerminals();
        }
        else
        {
            //Debug.LogError("Potentiometer terminals are not assigned.");

        }

        if (OnResistanceChanged == null)
        {
            OnResistanceChanged = new UnityEvent();
        }

        if (potentiometerMenu == null)
        {
            potentiometerMenu = FindObjectOfType<PotentiometerMenu>();
        }

    }

    private void OnValidate()
    {
        Resistance = resistance; // Ensure resistance change logic is executed when modifying in the Inspector
    }

    
    private void Update()
    {
        // Update LED intensity based on resistance during runtime
        if (OnResistanceChanged != null)
        {
            OnResistanceChanged.Invoke();
        }
    }

    public void PotentiometerClick()
    {
        if (!Camera.main.GetComponent<FirstPersonCam>().IsHoldingItem() && potentiometerMenu != null)
        {
            potentiometerMenu.OpenMenu(this);
        }
    }

    #region Setup Terminal
    private void SetupTerminals()
    {
        if (positiveTerminal != null && variableTerminal != null)
        {
            positiveTerminal.AddConnection(variableTerminal);
            Debug.Log($"LED {gameObject.name} terminals connected: Positive to variable");
        }
        else
        {
            Debug.LogError($"LED {gameObject.name} terminals are not assigned");
        }

        if (positiveTerminal != null && maxResistanceTerminal != null)
        {
            positiveTerminal.AddConnection(maxResistanceTerminal);
            Debug.Log($"LED {gameObject.name} terminals connected: Positive to MAX resistance");
        }
        else
        {
            Debug.LogError($"LED {gameObject.name} terminals are not assigned");
        }
    }
    #endregion Setup Terminal
}
