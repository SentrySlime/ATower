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

    public enum ItemType
    {
        Health,
        Defense,
        Movement,
        Damage,
        Ammo,
        WeaponSocket
    }

    public ItemType type;

    #region Health
    [Header("Player health Stats ---------------------------------------------------")]

    //Health ---
    [Tooltip("One point is equal to one max HP")]
    [Range(-200, 200f)] public int maxHp;


    [Tooltip("1 point is equal to 1 extra health gained when hitting an enemy")]
    [Range(-90f, 90f)] public int hpOnHit;

    [Tooltip("1 point is equal to 1 extra health gained when hitting killing an enemy")]
    [Range(-90f, 90f)] public int hpOnKill;

    [Tooltip("If it's at 1 it gives the player helping hand")]
    [Range(-1f, 1f)] public int helpingHand = 0;

    [Tooltip("1 point is equal to 1 extra health gained over 1 sec")]
    [Range(-90f, 90f)] public float hpRegen;

    [Tooltip("1 point is equal to activate this")]
    [HideInInspector] [Range(-1f, 1f)] public int hpRegenOnEnemyHit;

    #endregion

    #region Defense
    [Header("Defense ---------------------------------------------------")]

    [Tooltip(" 0.1 point is equal to 10% less damage taken from each attack")]
    [Range(-0.9f, 0.9f)] public float damageReductionPercent;

    [Tooltip("1 point is equal to 1% chance to take no damage")]
    [Range(-90f, 90f)] public int damageIgnoreChance;
    #endregion

    #region Movement
    [Header("Player movement Stats ---------------------------------------------------")]

    //Movement --- 
    [Tooltip("One point is equal to one extra movement speed")]
    [Range(-200, 200f)] public int moveSpeed;

    [Tooltip("1 point is equal to one extra jump")]
    [Range(-5f, 5f)] public int extraJumps;
    #endregion

    #region Damage
    [Header("Damage ---------------------------------------------------")]

    //Damage ---- 
    [Range(-100, 100f)] public int damage = 0;
    
    //Critical Hit ---- 
    [Tooltip("1 point is equal to 1% percent")]
    [Range(-25, 25f)] public int critChance = 0;
    
    //Explosion ---- 
    [Tooltip("If this is true, all your triggered kills explode enemies")]
    [Range(-1f, 1f)] public int canExplodeEnemies = 0;

    [Tooltip("If this is true, you get 1% damage for every 100 money")]
    [Range(-1f, 1f)] public int moneyIsPower = 0;

    #endregion

    #region Ammo
    [Header("Ammo ---------------------------------------------------")]

    //Ammunition ---- 
    [Tooltip("One point is equal to one max ammo refill")]
    [Range(-4, 4f)] public int ammoRefills = 0;

    [Tooltip("One point is equal to one extra shot reloaded at a time")]
    [Range(-4, 4f)] public int reloadAmount = 0;

    [Tooltip("Increases the magazine size of each weapon")]
    [Range(-10, 10f)] public int maxMagazineSize = 0;

    [Tooltip("0.1 is equal to 10% increase in reload speed")]
    [Range(-50, 50f)] public float reloadSpeed = 0;

    [Tooltip("1 is equal to returning one ammo on kill")]
    [Range(-5, 5f)] public int returnAmmoOnkill = 0;

    [Tooltip("Every other reload is 50% faster")]
    [Range(-1f, 1f)] public int hasAlternateFastReload = 0;

    #endregion

    #region WeaponSocket
    [Header("WeaponSocket ---------------------------------------------------")]
    [Tooltip("1 point is equal to 1% chance to fire a rocket")]
    [Range(-100, 100f)] public int fireBallChance = 0;

    #endregion
    
    [Header("Misc ---------------------------------------------------")]
    [Tooltip("Makes enemies drop increased amount of money")]
    [Range(-1f, 1f)] public int increasedMoneyDrops = 0;

    #region Misc



    #endregion

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
        playerStats.hpOnHit += hpOnHit;
        playerStats.hpOnKill += hpOnKill;
        playerStats.helpingHand += helpingHand;
        playerStats.hpRegenOnEnemyHit += hpRegenOnEnemyHit;
        playerStats.damageIgnoreChance += damageIgnoreChance;
        playerStats.damageReductionPercent += damageReductionPercent;


        //Movement ---
        playerStats.moveSpeed += moveSpeed;
        playerStats.extraJumps += extraJumps;
        
        //Damage ----
        playerStats.canExplode += canExplodeEnemies;

        playerStats.moneyIsPower += moneyIsPower;

        playerStats.damage += damage;
        playerStats.criticalChance += critChance;


        //Ammo ---
        playerStats.ammoRefills += ammoRefills;
        playerStats.reloadSpeed += reloadSpeed;
        playerStats.reloadAmount += reloadAmount;
        playerStats.maxMagazineSize += maxMagazineSize;
        playerStats.returnAmmoOnkill += returnAmmoOnkill;
        playerStats.hasAlternateFastReload += hasAlternateFastReload;



        //WeaponSocket
        playerStats.fireBallChance += fireBallChance;

        //Misc

        //Restart each one
        playerStats.StartPlayerHP();
        playerStats.StartLocomotion();
        playerStats.AddAllWeaponStats();
    }

    public virtual void UnEquipItem()
    {
        //Health ---
        playerStats.maxHealth -= maxHp;
        playerStats.hpRegen -= hpRegen;
        playerStats.hpOnHit -= hpOnHit;
        playerStats.hpOnKill -= hpOnKill;
        playerStats.helpingHand -= helpingHand;
        playerStats.hpRegenOnEnemyHit -= hpRegenOnEnemyHit;
        playerStats.damageIgnoreChance -= damageIgnoreChance;
        playerStats.damageReductionPercent -= damageReductionPercent;

        //movement ---
        playerStats.moveSpeed -= moveSpeed;
        playerStats.extraJumps -= extraJumps;

        //Damage ----
        playerStats.canExplode -= canExplodeEnemies;

        playerStats.moneyIsPower -= moneyIsPower;

        playerStats.damage -= damage;
        playerStats.criticalChance -= critChance;


        //Ammo
        playerStats.ammoRefills -= ammoRefills;
        playerStats.reloadSpeed -= reloadSpeed;
        playerStats.reloadAmount -= reloadAmount;
        playerStats.maxMagazineSize -= maxMagazineSize;
        playerStats.returnAmmoOnkill -= returnAmmoOnkill;
        playerStats.hasAlternateFastReload -= hasAlternateFastReload;


        //WeaponSocket ---
        playerStats.fireBallChance -= fireBallChance;
        
        //Restart each one
        playerStats.StartPlayerHP();
        playerStats.StartLocomotion();
        playerStats.AddAllWeaponStats();
    }

}