using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player HP")]
    int baseHealth = 100;
    public int addedHealth = 0;
    public float increasedHealth = 1;
    public int finalHealth = 0;
    public int oneMaxHP = 0;
    public int hpOnHit = 0;
    public int hpOnCritHit = 0;
    public int hpOnKill = 0;
    public int hpOnEliteKill = 0;
    public int helpingHand = 0;
    public float hpRegen = 0;
    public int hpRegenOnEnemyHit = 0;
    public int healCap = 0;
    public int canOverheal = 0;
    public int onlyEliteKillHeal = 0;
    public int healOnReload = 0;
    public float overkillDamageHeal = 0;
    public float increasedHealing = 0;

    [Header("Defense")]
    public float damageReductionPercent = 1.1f;
    public int damageIgnoreChance = 0;

    [Header("Locomotion")]
    public float moveSpeed = 0;
    public int extraJumps = 0;

    [Header("Damage")]
    public float addedDamage = 0;
    public float increasedDamage = 0;
    public float criticalChance = 5;
    public float criticalMultiplier = 2;
    public int canExplode = 0;
    public int moneyIsPower = 0;
    public int hpIsPower = 0;
    public int chainLightningTargets = 0;
    public int chainLightningDamage = 0;
    public int crimsonDagger = 0;
    public int pestilentSwarm = 0;

    [Header("Ammo")]
    public int ammoRefills = 0;
    public int reloadAmount = 0;
    public int maxMagazineSize = 0;
    public float reloadSpeed = 0;
    public int returnAmmoOnkill = 0;
    public int hasAlternateFastReload = 0;
    public bool alternateFastReload;
    public int heartboundRounds = 0;
    public float maxAmmo = 0;
    public int bandolierEffect = 0;

    [Header("WeaponSocket")]
    public int fireBallChance = 0;
    public int accuracy = 0;

    [Header("Misc")]
    public bool increasedMoneyDrop = false;
    public int moneyIsHealth = 0;
    public int thorns = 0;

    [Header("CurseStats")]
    public int moneyToLoseOnDamage = 0;
    public int maxHealthToLoseOnDamage = 0;


    PlayerHealth playerHealth;
    Locomotion2 locomotion;
    WeaponSocket weaponSocket;
    Inventory inventory;
    FindAndEquipWeapons findAndEquipWeapons;
    
    private void Awake()
    {
        GetAllScripts();
        GetFinalHealth();
    }

    void Start()
    {
        StartLocomotion();
    }

    //
    public void StartLocomotion()
    {
        locomotion.moveSpeed = moveSpeed;
        locomotion.UpdateJumps(extraJumps);
    }

    public void GetFinalHealth()
    {
        finalHealth = Mathf.Clamp((int)((baseHealth + addedHealth) * increasedHealth), 1, 9999);
    }

    public void StartPlayerHP()
    {
        playerHealth.damageReductionPercent = damageReductionPercent;
        playerHealth.damageIgnoreChance = damageIgnoreChance;

        GetFinalHealth();
        
        if (oneMaxHP > 0)
        {
            finalHealth = 1;
            playerHealth.maxHP = finalHealth;
            playerHealth.currentHP = finalHealth;

        }
        else if(finalHealth > playerHealth.maxHP)
        {
            int tempHP = finalHealth - playerHealth.maxHP;
            playerHealth.maxHP = finalHealth;
            playerHealth.currentHP += tempHP;
        }
        else if(finalHealth < playerHealth.maxHP)
        {
            playerHealth.maxHP = finalHealth;

            if(playerHealth.currentHP > finalHealth && canOverheal <= 0)
            {
                playerHealth.currentHP = finalHealth;
            }
            
        }
        else if(playerHealth.currentHP > playerHealth.maxHP)
        {
            if (canOverheal <= 0)
            {
                playerHealth.currentHP = finalHealth;
            }
        }


        playerHealth.ItemUpdateHealth();
    }

    

    public void AddStatsToPickedUpWeapon(GameObject weaponObj)
    {

        BaseWeapon weapon = weaponObj.transform.GetChild(0).GetComponent<BaseWeapon>();

        weapon.maxMagazine = weapon.baseMaxMagazine + maxMagazineSize;

        weapon.maxMagazine = Mathf.Clamp(weapon.maxMagazine, 1, 999); 

        if (bandolierEffect != 0)
        {
            int additionalBullets = Mathf.FloorToInt((weapon.baseMaxMagazine / 15f) * bandolierEffect);
            weapon.maxMagazine += additionalBullets;
            weapon.maxMagazine = Mathf.Clamp(weapon.maxMagazine, 1, 999);
        }

        weapon.currentMagazine = weapon.maxMagazine;

        weapon.maxAmmo = Mathf.FloorToInt(weapon.baseMaxAmmo + (weapon.baseMaxAmmo * maxAmmo));
        weapon.currentAmmo = weapon.maxAmmo;

        
    }


    public void AddAllWeaponStats()
    {
        print("2");

        if (inventory.heldWeapons.Count == 0) { return; }

        for (int i = 0; i < inventory.heldWeapons.Count; i++)
        {
            BaseWeapon weapon = inventory.heldWeapons[i].transform.GetChild(0).GetComponent<BaseWeapon>();

            // Apply base and player bonus
            weapon.maxMagazine = weapon.baseMaxMagazine + maxMagazineSize;
            weapon.maxMagazine = Mathf.Clamp(weapon.maxMagazine, 1, 999);

            // Apply Bandolier effect
            if (bandolierEffect != 0)
            {
                int additionalBullets = Mathf.FloorToInt((weapon.baseMaxMagazine / 15f) * bandolierEffect);
                weapon.maxMagazine += additionalBullets;
                weapon.maxMagazine = Mathf.Clamp(weapon.maxMagazine, 1, 999);
            }

            weapon.maxAmmo = Mathf.FloorToInt(weapon.baseMaxAmmo + (weapon.baseMaxAmmo * maxAmmo));

            weapon.currentMagazine = weapon.maxMagazine;

            weapon.SetAmmoInfo();

            findAndEquipWeapons.SetWeapon(weaponSocket.equippedWeapon.transform.parent.gameObject);
        }
    }



    private void GetAllScripts()
    {
        playerHealth = GetComponent<PlayerHealth>();
        locomotion = GetComponent<Locomotion2>();
        weaponSocket = GetComponent<WeaponSocket>();
        inventory = GetComponent<Inventory>();
        findAndEquipWeapons = GetComponent<FindAndEquipWeapons>();
    }
}

