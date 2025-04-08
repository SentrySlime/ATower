using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player HP")]
    public int maxHealth;
    public float damageReductionPercent = 1.1f;
    public int damageIgnoreChance = 0;

    [Header("Healing")]
    public float hpRegen = 1;
    public int hpOnKill = 5;
    public int hpOnHit = 1;
    public int helpingHand = 0;
    public int hpRegenOnEnemyHit = 0;

    [Header("Damage")]
    public float damage = 0;
    public float criticalChance = 5;
    public float criticalMultiplier = 2;

    [Header("Locomotion")]
    public float moveSpeed = 0;
    public int extraJumps = 0;

    [Header("Weapon Stats")]
    public int reloadAmount = 0;
    public int maxMagazineSize = 0;
    [Tooltip("0.1 is equal to 10% increase in reload speed")]
    public float reloadSpeed = 0;

    [Header("Return Ammo On Kill")]
    public int returnAmmoOnkill = 0;

    [Header("Alternate reload is 50% faster")]
    public int hasAlternateFastReload = 0;
    public bool alternateFastReload = false;

    [Header("Others")]
    public bool canExplode = false;
    public int ammoRefills = 0;
    public int moneyIsPower = 0;

    public bool increasedMoneyDrop = false;

    [Header("WeaponSocket")]
    public int fireBallChance = 0;

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

        //locomotion.maxJumps = extraJumps;
    }

    public void StartPlayerHP()
    {
        playerHealth.damageReductionPercent = damageReductionPercent;
        playerHealth.damageIgnoreChance = damageIgnoreChance;
        playerHealth.maxHP = maxHealth;
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

