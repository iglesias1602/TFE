using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ObjectType { resistor, led, potentiometer }

public class SaveableObject : MonoBehaviour
{
    protected string save;
    private  ObjectType objectType;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    public void Save(int id)
    {

    }

    public void Load(string[] values)
    {
        
    }

    public void DestroySaveable()
    {

    }

}
