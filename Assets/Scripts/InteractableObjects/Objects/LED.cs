using UnityEngine;


public class LED : MonoBehaviour
{
    public Color onColor = Color.green;
    public Color offColor = Color.red;
    private bool isOn = false;

    public void ToggleColor()
    {
        isOn = !isOn;
        GetComponent<MeshRenderer>().material.color = isOn ? onColor : offColor;
    }
}
