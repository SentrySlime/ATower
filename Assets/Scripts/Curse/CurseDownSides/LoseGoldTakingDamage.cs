using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseGoldTakingDamage : CurseDownside
{
    [Header("Negative")]
    public int goldLostOnDamage = 1;

    public override void ApplyCurseDownSide()
    {
        curse.downSideDescription.text = description + " -" + (goldLostOnDamage);
        playerStats.moneyToLoseOnDamage += goldLostOnDamage;
        playerStats.StartPlayerHP();
    }

    public override void RemoveCurseDownSide()
    {
        playerStats.moneyToLoseOnDamage -= goldLostOnDamage;
        playerStats.StartPlayerHP();
    }
}
