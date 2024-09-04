using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSocketItems : ItemBase
{

    public PlayerStats playerStats;

    [Header("Player weaponStats Stats")]

    [Tooltip("1 point is equal to 1% chance to fire a rocket")]
    [Range(-90f, 90f)] public int rocketChance;

    private void Awake()
    {

        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    private void Start()
    {
    }

    public override void EquipItem()
    {
        playerStats.rocketChance += rocketChance;
        playerStats.StartWeaponSocket();
    }

    public override void UnEquipItem()
    {
        playerStats.rocketChance -= rocketChance;
        playerStats.StartWeaponSocket();
    }
}
