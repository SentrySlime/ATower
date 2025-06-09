using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIncrease : CurseReward
{
    public override void Reward()
    {
        playerStats.increasedDamage += 0.5f;
    }
}
