using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpIncrease : CurseReward
{
    [Header("Reward")]
    public int increasedAmountOfJumps = 1;

    public override void Reward()
    {
        playerStats.extraJumps += increasedAmountOfJumps;
        playerStats.StartLocomotion();
    }

    public override string ReturnDescription()
    {
        return "extra jumps +" + (increasedAmountOfJumps);
    }
}
