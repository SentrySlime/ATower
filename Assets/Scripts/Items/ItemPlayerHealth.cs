using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlayerHealth : ItemBase
{
    public PlayerStats playerStats;


    [Header("Player health Stats")]

    [Tooltip("One point is equal to one max HP")]
    [Range(-200, 200f)] public int maxHp;

    [Tooltip(" 0.1 point is equal to 10% less damage taken from each attack")]
    [Range(-0.9f, 0.9f)] public float damageReductionPercent;

    [Tooltip("1 point is equal to 1% chance to take no damage")]
    [Range(-90f, 90f)] public int damageIgnoreChance;

    private void Awake()
    {

        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    private void Start()
    {
    }

    public override void EquipItem()
    {
        playerStats.maxHealth += maxHp;
        playerStats.damageReductionPercent += damageReductionPercent;
        playerStats.damageIgnoreChance += damageIgnoreChance;
        playerStats.StartPlayerHP();
    }

    public override void UnEquipItem()
    {
        playerStats.maxHealth -= maxHp;
        playerStats.damageReductionPercent -= damageReductionPercent;
        playerStats.damageIgnoreChance -= damageIgnoreChance;
        playerStats.StartPlayerHP();
    }

}
