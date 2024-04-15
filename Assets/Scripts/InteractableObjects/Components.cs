using UnityEngine;
/*
public class SwitchConnector : MonoBehaviour, IPowerSource
{
    public bool IsPoweredOn { get; private set; } = false;

    public void Interact(IInteractable other)
    {
        if (other is IPowerReceiver receiver)
        {
            TogglePower();
            receiver.ReceivePower(IsPoweredOn);
        }
    }

    public void TogglePower()
    {
        IsPoweredOn = !IsPoweredOn;
        Debug.Log($"Switch power is now {(IsPoweredOn ? "ON" : "OFF")}");
    }

    private void OnMouseDown()
    {
        // Assuming only cables directly connected to this switch
        var cables = FindObjectsOfType<Cable>(); // This could be optimized
        foreach (var cable in cables)
        {
            if (cable.IsConnectedTo(this))
            {
                cable.TransmitInteraction(this);
            }
        }
    }
}
*/