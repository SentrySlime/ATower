using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDecrease : CurseDownside
{
    [Header("Negative")]
    public float reducedHP = -0.5f;

    public override void ApplyCurseDownSide()
    {
        playerStats.increasedHealth += reducedHP;
        playerStats.StartPlayerHP();
    }

    public override void RemoveCurseDownSide()
    {
        playerStats.increasedHealth -= reducedHP;
        playerStats.StartPlayerHP();
    }
}
