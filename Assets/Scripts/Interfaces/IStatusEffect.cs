using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public interface IStatusEffect
{
    void Freeze();

    bool IsFrozen();

    void UnFreeze();

    void Burn();
}
