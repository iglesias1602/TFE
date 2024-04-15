using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    Transform cameraPosition;
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
