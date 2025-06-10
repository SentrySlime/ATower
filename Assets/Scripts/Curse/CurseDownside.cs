using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CurseDownside : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public Curse curse;

    public string description;

    public abstract void ApplyCurseDownSide();
    public abstract void RemoveCurseDownSide();
}
