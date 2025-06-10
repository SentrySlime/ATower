using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedReloadSpeed : CurseReward
{
    [Header("Reward")]
    public float increasedReloadSpeed = 0.5f;

    public override void Reward()
    {
        playerStats.reloadSpeed += increasedReloadSpeed;
    }

    public override string ReturnDescription()
    {
        return "reload speed +" + (increasedReloadSpeed * 100) + "%";
    }
}
