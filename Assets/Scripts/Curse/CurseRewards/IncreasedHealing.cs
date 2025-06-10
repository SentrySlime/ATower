using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedHealing : CurseReward
{

    [Header("Reward")]
    public float increasedHealing = 0.25f;

    public override void Reward()
    {
        playerStats.increasedHealing += increasedHealing;
        playerStats.StartPlayerHP();
    }

    public override string ReturnDescription()
    {
        return "healing efficiency +" + (increasedHealing * 100) + "%";
    }

}
