using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDecrease : CurseDownside
{
    [Header("Negative")]
    public float reducedDamage = -0.25f;

    public override void ApplyCurseDownSide()
    {
        curse.downSideDescription.text = description + " " + (reducedDamage * 100) + "%";
        playerStats.increasedDamage += reducedDamage;
        playerStats.StartPlayerHP();
    }

    public override void RemoveCurseDownSide()
    {
        playerStats.increasedDamage -= reducedDamage;
        playerStats.StartPlayerHP();
    }
}
