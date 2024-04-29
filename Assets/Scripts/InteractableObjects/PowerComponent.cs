using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPowerable
{
    void SetPower(bool power);
    bool CanPropagatePower();
}