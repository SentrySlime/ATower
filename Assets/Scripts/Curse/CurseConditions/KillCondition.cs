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
        enemyManager.enemyDeathReport += ConditionCheck;
    }

    
    void Update()
    {
        
    }

    public override void ConditionCheck()
    {
        if (complete) return;

        currentKillCount++;
        if (currentKillCount >= requiredKillCount)
        {
            complete = true;
            ReturnComplete();
        }
    }
}
