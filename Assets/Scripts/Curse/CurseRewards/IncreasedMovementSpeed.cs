using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedMovementSpeed : CurseReward
{
    [Header("Reward")]
    public int increasedMovementSpeed = 5;

    public override void Reward()
    {
        playerStats.moveSpeed += increasedMovementSpeed;
        playerStats.StartLocomotion();
    }

    public override string ReturnDescription()
    {
        return "added movement speed +" + (increasedMovementSpeed);
    }
}
