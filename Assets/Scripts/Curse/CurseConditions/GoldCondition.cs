using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldCondition : CurseCondition
{
    [Header("Condition")]
    public int requiredGoldCount = 500;
    int currentGoldCount = 0;

    void Start()
    {
        curse.conditionDescription.text = description;
        curse.conditionCount.text = currentGoldCount + " / " + requiredGoldCount;
        inventory.goldReport += ConditionCheck;
    }

    public override void ConditionCheck(int value)
    {
        if (complete) return;

        currentGoldCount += value;
        Mathf.Clamp(currentGoldCount, 0, requiredGoldCount);
        curse.conditionCount.text = currentGoldCount + " / " + requiredGoldCount;
        if (currentGoldCount >= requiredGoldCount)
        {
            complete = true;
            ReturnComplete();
        }
    }
}
