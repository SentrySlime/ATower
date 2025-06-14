using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIncrease : CurseReward
{
    [Header("Reward")]
    public float increasedDamage = 0.25f;

    public override void Reward()
    {
        playerStats.increasedDamage += increasedDamage;
    }

    public override string ReturnDescription()
    {
        return "damage +" + (increasedDamage * 100) + "%";
    }
}
