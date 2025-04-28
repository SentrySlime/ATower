using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player HP")]
    public int maxHealth;
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

    [Header("Defense")]
    public float damageReductionPercent = 1.1f;
    public int damageIgnoreChance = 0;

    [Header("Locomotion")]
    public float moveSpeed = 0;
    public int extraJumps = 0;

    [Header("Damage")]
    public float damage = 0;
    public float criticalChance = 5;
    public float criticalMultiplier = 2;
    public int canExplode = 0;
    public int moneyIsPower = 0;
    public int hpIsPower = 0;
    

    [Header("Ammo")]
    public int ammoRefills = 0;
    public int reloadAmount = 0;
    public int maxMagazineSize = 0;
    public float reloadSpeed = 0;
    public int returnAmmoOnkill = 0;
    public int hasAlternateFastReload = 0;
    public bool alternateFastReload;
    public int heartboundRounds = 0;

    [Header("WeaponSocket")]
    public int fireBallChance = 0;

    [Header("Misc")]
    public bool increasedMoneyDrop = false;
    public int moneyIsHealth = 0;

    PlayerHealth playerHealth;
    Locomotion2 locomotion;
    WeaponSocket weaponSocket;
    Inventory inventory;
    FindAndEquipWeapons findAndEquipWeapons;
    
    private void Awake()
    {
        GetAllScripts();
    }

    // Start is called before the first frame update
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

    public void StartPlayerHP()
    {
        playerHealth.damageReductionPercent = damageReductionPercent;
        playerHealth.damageIgnoreChance = damageIgnoreChance;

        
        if (oneMaxHP > 0)
        {
            playerHealth.maxHP = 1;
            playerHealth.currentHP = 1;

        }
        else if(maxHealth > playerHealth.maxHP)
        {
            int tempHP = maxHealth - playerHealth.maxHP;
            playerHealth.maxHP = maxHealth;
            playerHealth.currentHP += tempHP;

        }
        else if(playerHealth.currentHP > playerHealth.maxHP)
        {
            if (canOverheal <= 0)
            {
                playerHealth.currentHP = maxHealth;
            }
        }


        playerHealth.ItemUpdateHealth();
    }

    public void AddStatsToPickedUpWeapon(GameObject weaponObj)
    {
        BaseWeapon weapon = weaponObj.transform.GetChild(0).GetComponent<BaseWeapon>();
        weapon.maxMagazine = weapon.baseMaxMagazine + maxMagazineSize;
        weapon.currentMagazine = weapon.maxMagazine;
    }

    public void AddAllWeaponStats()
    {
        if(inventory.heldWeapons.Count == 0) { return; }

        for (int i = 0; i < inventory.heldWeapons.Count; i++)
        {
            BaseWeapon weapon = inventory.heldWeapons[i].transform.GetChild(0).GetComponent<BaseWeapon>();
            weapon.maxMagazine = weapon.baseMaxMagazine + maxMagazineSize;
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

