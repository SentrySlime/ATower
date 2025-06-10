using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CritHitCondition : CurseCondition
{
    [Header("Condition")]
    public int requiredCritCount = 25;
    int currentCritCount = 0;

    void Start()
    {
        aMainSystem.criHitReport += ConditionCheck;
    }

    public override void ConditionCheck(int value)
    {
        if (complete) return;

        currentCritCount += value;
        if (currentCritCount >= requiredCritCount)
        {
            complete = true;
            ReturnComplete();
        }
    }
}
