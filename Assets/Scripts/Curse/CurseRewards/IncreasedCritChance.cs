using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedCritChance : CurseReward
{
    [Header("Reward")]
    public float critChance = 15f;

    public override void Reward()
    {
        playerStats.criticalChance += critChance;
    }

    public override string ReturnDescription()
    {
        return "crit chance +" + critChance + "%";
    }

}
