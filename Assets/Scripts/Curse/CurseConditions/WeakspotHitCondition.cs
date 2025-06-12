using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class WeakspotHitCondition : CurseCondition
{
    [Header("Condition")]
    public int requiredWeakSpotHitCount = 25;
    int currentWeakSpotCount = 0;

    void Start()
    {
        curse.conditionDescription.text = description;
        curse.conditionCount.text = currentWeakSpotCount + " / " + requiredWeakSpotHitCount;
        aMainSystem.weakSpotHitCondition += ConditionCheck;
    }

    public override void ConditionCheck(int value)
    {
        if (complete) return;

        currentWeakSpotCount += value;
        Mathf.Clamp(currentWeakSpotCount, 0, requiredWeakSpotHitCount);
        curse.conditionCount.text = currentWeakSpotCount + " / " + requiredWeakSpotHitCount;
        if (currentWeakSpotCount >= requiredWeakSpotHitCount)
        {
            complete = true;
            ReturnComplete();
        }
    }
}
