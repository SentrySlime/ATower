using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillCondition : CurseCondition
{
    [Header("Condition")]
    public int requiredKillCount = 25;
    int currentKillCount = 0;

    void Start()
    {
        curse.conditionDescription.text = description;
        curse.conditionCount.text = currentKillCount + " / " + requiredKillCount;
        enemyManager.enemyDeathReport += ConditionCheck;
    }

    public override void ConditionCheck(int value)
    {
        if (complete) return;

        currentKillCount += value;
        curse.conditionCount.text = currentKillCount + " / " + requiredKillCount;
        if (currentKillCount >= requiredKillCount)
        {
            complete = true;
            ReturnComplete();
        }
    }
}
