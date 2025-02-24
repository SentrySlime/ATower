using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemBase : MonoBehaviour
{
    [Header("Base")]
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public Mesh itemMesh;
    public Material itemMaterial;

    #region Health
    //Health ---
    [Header("Player health Stats")]
    [Tooltip("One point is equal to one max HP")]
    [Range(-200, 200f)] public int maxHp;

    [Tooltip("1 point is equal to 1 extra health gained over 1 sec")]
    [Range(-90f, 90f)] public int hpRegen;

    [Tooltip(" 0.1 point is equal to 10% less damage taken from each attack")]
    [Range(-0.9f, 0.9f)] public float damageReductionPercent;

    [Tooltip("1 point is equal to 1% chance to take no damage")]
    [Range(-90f, 90f)] public int damageIgnoreChance;
    #endregion

    #region Movement
    //Movement --- 
    [Header("Player movement Stats")]
    [Tooltip("One point is equal to one extra movement speed")]
    [Range(-200, 200f)] public int moveSpeed;

    [Tooltip("1 point is equal to one extra jump")]
    [Range(-5f, 5f)] public int extraJumps;
    #endregion

    #region Damage
    //Explosion ---- 
    [Header("Explosisions")]
    [Tooltip("If this is true, all your triggered kills explode enemies")]
    public bool includeExplosion = false;
    public bool canExplodeEnemies;

    //Damage ---- 
    [Header("Damage")]
    [Range(-100, 100f)] public int damage = 0;

    //Critical Hit ---- 
    [Header("Crit")]
    [Range(-25, 25f)] public int critChance = 0;
    
    //Weapon affected
    [Tooltip("1 point is equal to 1% chance to fire a rocket")]
    [Range(-90f, 90f)] public int rocketChance;

    #endregion

    #region Ammo

    //Ammunition ---- 
    [Header("Ammo")]
    [Tooltip("One point is equal to one max ammo refill")]
    [Range(-4, 4f)] public int ammoRefills = 0;

    [Tooltip("One point is equal to one extra shot reloaded at a time")]
    [Range(-4, 4f)] public int reloadAmount = 0;

    #endregion

    [Header("Ammo")]
    [Tooltip("Increases the magazine size of each weapon")]
    [Range(-10, 10f)] public int maxMagazineSize = 0;

    public PlayerStats playerStats;

    void Start()
    {

    }

    public virtual void EquipItem()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        //Health ---
        playerStats.maxHealth += maxHp;
        playerStats.hpRegen += hpRegen;
        playerStats.damageReductionPercent += damageReductionPercent;
        playerStats.damageIgnoreChance += damageIgnoreChance;

        //Movement ---
        playerStats.moveSpeed += moveSpeed;
        playerStats.extraJumps += extraJumps;
        
        //Damage & Ammo ----
        if (includeExplosion)
        {
            playerStats.canExplode = canExplodeEnemies;
        }

        playerStats.damage += damage;
        playerStats.criticalChance += critChance;
        playerStats.ammoRefills += ammoRefills;
        playerStats.reloadAmount += reloadAmount;

        playerStats.rocketChance += rocketChance;

        //Weapon Stats ---
        playerStats.maxMagazineSize = maxMagazineSize;

        //Restart each one
        playerStats.StartPlayerHP();
        playerStats.StartLocomotion();
        playerStats.StartWeaponSocket();
        playerStats.AddAllWeaponStats();
    }

    public virtual void UnEquipItem()
    {
        //Health ---
        playerStats.maxHealth -= maxHp;
        playerStats.hpRegen -= hpRegen;
        playerStats.damageReductionPercent -= damageReductionPercent;
        playerStats.damageIgnoreChance -= damageIgnoreChance;

        //movement ---
        playerStats.moveSpeed -= moveSpeed;
        playerStats.extraJumps -= extraJumps;

        //Damage & Ammo ----
        if (includeExplosion)
        {
            playerStats.canExplode = canExplodeEnemies;
        }

        playerStats.damage -= damage;
        playerStats.criticalChance -= critChance;
        playerStats.ammoRefills -= ammoRefills;
        playerStats.reloadAmount -= reloadAmount;

        playerStats.rocketChance -= rocketChance;
        


        //Restart each one
        playerStats.StartPlayerHP();
        playerStats.StartLocomotion();
        playerStats.StartWeaponSocket();
        playerStats.AddAllWeaponStats();
    }

}