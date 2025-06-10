using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthIncrease : CurseReward
{
    [Header("Negative")]
    public float increasedHealth = 0.25f;

    public override void Reward()
    {
        playerStats.increasedHealth += increasedHealth;
        playerStats.StartPlayerHP();
    }

    public override string ReturnDescription()
    {
        return "max health +" + (increasedHealth * 100) + "%";
    }
}
