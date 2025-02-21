using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Player HP")]
    public PlayerHealth playerHealth;
    public int maxHealth;
    public float damageReductionPercent = 1.1f;
    public int damageIgnoreChance = 0;

    [Header("Healing")]
    public float hpRegen = 1;
    public int healPerKill = 5;


    [Header("Damage")]
    public float criticalChance = 5;
    public float criticalMultiplier = 2;

    [Header("Locomotion")]
    Locomotion2 locomotion;
    public float moveSpeed = 0;
    public int extraJumps = 0;

    [Header("WeaponSocket")]
    WeaponSocket weaponSocket;
    public int reloadAmount = 0;
    public int rocketChance = 0;

    [Header("Others")]
    public bool canExplode = false;
    public int ammoRefills = 0;

    private void Awake()
    {
        GetAllScripts();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartLocomotion();
        StartPlayerHP();
        StartWeaponSocket();
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
        playerHealth.maxHP = maxHealth;
        playerHealth.UpdateHPRegen();
        playerHealth.damageReductionPercent = damageReductionPercent;
        playerHealth.damageIgnoreChance = damageIgnoreChance;
        playerHealth.UpdateHP();
    }

    public void StartWeaponSocket()
    {
        weaponSocket.rocketChance = rocketChance;
    }

    // Update is called once per frame
    void Update()
    {


    }


    private void GetAllScripts()
    {
        locomotion = GetComponent<Locomotion2>();
        playerHealth = GetComponent<PlayerHealth>();
        weaponSocket = GetComponent<WeaponSocket>();
    }


}

