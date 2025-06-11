using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CurseReward : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public Curse curse;

    public enum RewardCategory
    {
        Damage,
        Utility,
        Health
    }

    public RewardCategory rewardCategory;

    public abstract void Reward();

    public abstract string ReturnDescription();
}
