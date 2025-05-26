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
    public int goldCost = 500;


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
    public bool isDevilItem;

    [Header("//-----Items-----\\")]

    #region Health
    [Header("Player health Stats ---------------------------------------------------")]

    //Health ---
    [Tooltip("One point is equal to one max HP")]
    [Range(-200, 200f)] public int maxHp;

    [Tooltip("Sets HP to 1")]
    [Range(-1, 1f)] public int oneMaxHP;

    [Tooltip("1 point is equal to 1 extra health gained when hitting an enemy")]
    [Range(-90f, 90f)] public int hpOnHit;

    [Tooltip("1 point is equal to 1 extra health gained when hitting a crit on an enemy")]
    [Range(-90f, 90f)] public int hpOnCritHit;
    
    [Tooltip("1 point is equal to 1 extra health gained when hitting killing an enemy")]
    [Range(-90f, 90f)] public int hpOnKill;

    [Tooltip("1 point is equal to 1 extra health gained when hitting killing an elite enemy")]
    [Range(-90f, 90f)] public int hpOnEliteKill;

    [Tooltip("If it's at 1 it gives the player helping hand")]
    [Range(-1f, 1f)] public int helpingHand = 0;

    [Tooltip("1 point is equal to 1 extra health gained over 1 sec")]
    [Range(-90f, 90f)] public float hpRegen;

    [Tooltip("1 point is equal to activate this")]
    [HideInInspector] [Range(-1f, 1f)] public int hpRegenOnEnemyHit;

    [Tooltip("1 point says we are healCapped")]
    [HideInInspector][Range(-100f, 100f)] public int healCap;

    [Tooltip("1 point says we can overheal")]
    [HideInInspector][Range(-1f, 1f)] public int canOverheal;

    [Tooltip("1 point says we can only heal from killing elites")]
    [HideInInspector][Range(-1f, 1f)] public int onlyEliteKillHeal;

    [Tooltip("1 point says we can heal one extra from reloading an empty mag")]
    [HideInInspector][Range(-100f, 100f)] public int healOnReload;

    [Tooltip("1 point is equal to 1% of overkill damage gained as healing")]
    [Range(-100f, 100f)] public float overkillDamageHeal;

    

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

    [Tooltip("0.1 point is equal to 10% percent")]
    [Range(-100, 100f)] public float increasedDamage = 0;
    
    //Critical Hit ---- 
    [Tooltip("1 point is equal to 1% percent")]
    [Range(-25, 25f)] public int critChance = 0;
    
    //Explosion ---- 
    [Tooltip("If this is true, all your triggered kills explode enemies")]
    [Range(-1f, 1f)] public int canExplodeEnemies = 0;

    [Tooltip("If this is true, you get 1% damage for every 100 money")]
    [Range(-1f, 1f)] public int moneyIsPower = 0;

    [Tooltip("If this is true, you get 1% damage for every 100 money")]
    [Range(-1f, 1f)] public int hpIsPower = 0;
    

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

    [Tooltip("Shooting while on an empty magazine deals damage equal to 5% of your health")]
    [Range(-1f, 1f)] public int heartboundRounds = 0;

    [Tooltip("Increased percentage of max ammo for all weapons, 0.1 point is 10% increased ammo")]
    [Range(-4f, 4f)] public float maxAmmo = 0;

    #endregion

    #region WeaponSocket
    [Header("WeaponSocket ---------------------------------------------------")]
    [Tooltip("1 point is equal to 1% chance to fire a rocket")]
    [Range(-100, 100f)] public int fireBallChance = 0;

    [Tooltip("1 point is equal to 1 point lower accuracy")]
    [Range(-30, 30f)] public int accuracy = 0;

    #endregion

    #region Misc

    [Header("Misc ---------------------------------------------------")]
    [Tooltip("Makes enemies drop increased amount of money")]
    [Range(-1f, 1f)] public int increasedMoneyDrops = 0;

    [Tooltip("You take first from money before you take damage from hp")]
    [Range(-1f, 1f)] public int moneyIsHealth = 0;


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
        playerStats.oneMaxHP += oneMaxHP;
        playerStats.hpRegen += hpRegen;
        playerStats.hpOnHit += hpOnHit;
        playerStats.hpOnCritHit += hpOnCritHit;
        playerStats.hpOnKill += hpOnKill;
        playerStats.hpOnEliteKill += hpOnEliteKill;
        playerStats.helpingHand += helpingHand;
        playerStats.hpRegenOnEnemyHit += hpRegenOnEnemyHit;
        playerStats.healCap += healCap;
        playerStats.canOverheal += canOverheal;
        playerStats.onlyEliteKillHeal += onlyEliteKillHeal;
        playerStats.healOnReload += healOnReload;
        playerStats.overkillDamageHeal += overkillDamageHeal;

        //Defense
        playerStats.damageIgnoreChance += damageIgnoreChance;
        playerStats.damageReductionPercent += damageReductionPercent;

        //Movement ---
        playerStats.moveSpeed += moveSpeed;
        playerStats.extraJumps += extraJumps;
        
        //Damage ----
        playerStats.addedDamage += damage;
        playerStats.increasedDamage += increasedDamage;
        playerStats.moneyIsPower += moneyIsPower;
        playerStats.criticalChance += critChance;
        playerStats.hpIsPower += hpIsPower;
        playerStats.canExplode += canExplodeEnemies;


        //Ammo ---
        playerStats.ammoRefills += ammoRefills;
        playerStats.reloadAmount += reloadAmount;
        playerStats.maxMagazineSize += maxMagazineSize;
        playerStats.reloadSpeed += reloadSpeed;
        playerStats.returnAmmoOnkill += returnAmmoOnkill;
        playerStats.hasAlternateFastReload += hasAlternateFastReload;
        playerStats.heartboundRounds += heartboundRounds;
        playerStats.maxAmmo += maxAmmo;


        //WeaponSocket
        playerStats.fireBallChance += fireBallChance;
        playerStats.accuracy += accuracy;

        //Misc
        playerStats.moneyIsHealth += moneyIsHealth;

        //Restart each one
        playerStats.StartPlayerHP();
        playerStats.StartLocomotion();
        playerStats.AddAllWeaponStats();
    }

    public virtual void UnEquipItem()
    {
        //Health ---
        playerStats.maxHealth -= maxHp;
        playerStats.oneMaxHP -= oneMaxHP;
        playerStats.hpRegen -= hpRegen;
        playerStats.hpOnHit -= hpOnHit;
        playerStats.hpOnCritHit -= hpOnCritHit;
        playerStats.hpOnKill -= hpOnKill;
        playerStats.hpOnEliteKill -= hpOnEliteKill;
        playerStats.helpingHand -= helpingHand;
        playerStats.hpRegenOnEnemyHit -= hpRegenOnEnemyHit;
        playerStats.healCap -= healCap;
        playerStats.canOverheal -= canOverheal;
        playerStats.onlyEliteKillHeal -= onlyEliteKillHeal;
        playerStats.healOnReload -= healOnReload;
        playerStats.overkillDamageHeal -= overkillDamageHeal;

        //Defense
        playerStats.damageIgnoreChance -= damageIgnoreChance;
        playerStats.damageReductionPercent -= damageReductionPercent;

        //movement ---
        playerStats.moveSpeed -= moveSpeed;
        playerStats.extraJumps -= extraJumps;

        //Damage ----
        playerStats.addedDamage -= damage;
        playerStats.increasedDamage -= increasedDamage;
        playerStats.criticalChance -= critChance;
        playerStats.moneyIsPower -= moneyIsPower;
        playerStats.hpIsPower -= hpIsPower;
        playerStats.canExplode -= canExplodeEnemies;


        //Ammo
        playerStats.ammoRefills -= ammoRefills;
        playerStats.reloadSpeed -= reloadSpeed;
        playerStats.reloadAmount -= reloadAmount;
        playerStats.maxMagazineSize -= maxMagazineSize;
        playerStats.returnAmmoOnkill -= returnAmmoOnkill;
        playerStats.hasAlternateFastReload -= hasAlternateFastReload;
        playerStats.heartboundRounds -= heartboundRounds;
        playerStats.maxAmmo -= maxAmmo;

        //WeaponSocket ---
        playerStats.fireBallChance -= fireBallChance;
        playerStats.accuracy -= accuracy;

        //Misc
        playerStats.moneyIsHealth -= moneyIsHealth;

        //Restart each one
        playerStats.StartPlayerHP();
        playerStats.StartLocomotion();
        playerStats.AddAllWeaponStats();
    }

}