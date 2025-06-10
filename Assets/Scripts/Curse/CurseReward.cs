using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CurseReward : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public Curse curse;

    public abstract void Reward();

    public abstract string ReturnDescription();
}
