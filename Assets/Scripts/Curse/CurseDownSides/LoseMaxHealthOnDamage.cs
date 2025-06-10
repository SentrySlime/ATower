using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseMaxHealthOnDamage : CurseDownside
{
    [Header("Negative")]
    public int reducedMaxHp = 1;

    public override void ApplyCurseDownSide()
    {
        curse.downSideDescription.text = description + " -" + (reducedMaxHp);
        playerStats.maxHealthToLoseOnDamage += reducedMaxHp;
        playerStats.StartPlayerHP();
    }

    public override void RemoveCurseDownSide()
    {
        playerStats.maxHealthToLoseOnDamage -= reducedMaxHp;
        playerStats.StartPlayerHP();
    }
}
