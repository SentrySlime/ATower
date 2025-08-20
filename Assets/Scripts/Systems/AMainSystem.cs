using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AMainSystem : MonoBehaviour
{
    [Header("SFX")]
    public GameObject hitSFX;
    public GameObject critHitSFX;

    [Header("VFX")]
    public GameObject hitVFX;
    public GameObject hitEnemyVFX;
    public GameObject pickUpVFX;

    public GameObject player;

    PlayerStats playerStats;
    PlayerHealth playerHealth;
    HealthRegen healthRegen;
    Inventory inventory;
    ShootSystem shootSystem;
    HitmarkerLogic hitMarkerLogic;
    ExplosionSystem explosionSystem;
    ChainLightning chainLightning;

    [HideInInspector] public event System.Action<int> criHitReport;
    [HideInInspector] public event System.Action<int> weakSpotHitCondition;

    private void Awake()
    {
        shootSystem = GetComponent<ShootSystem>();
        explosionSystem = GetComponent<ExplosionSystem>();
        chainLightning = GetComponent<ChainLightning>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        playerHealth = player.GetComponent<PlayerHealth>();
        healthRegen = player.GetComponent<HealthRegen>();
        inventory = player.GetComponent<Inventory>();
        hitMarkerLogic = GameObject.FindGameObjectWithTag("HitMarker").GetComponent<HitmarkerLogic>();
    }

    public void SpawnPickUpEffects(Vector3 position)
    {
        if(pickUpVFX)
            Instantiate(pickUpVFX, position, Quaternion.identity);
    }

    public void DealDamage(GameObject incomingObj, float incomingDamage, bool friendly, bool weakSpotShot = false, EnemyBase enemyBase = null, IDamageInterface iDamageInterface = null, bool canTriggerChainLightning = false)
    {
        print("Dealing Damage 1");

        if (friendly)
        {
            print("Dealing Damage 2");

            if (canTriggerChainLightning == false)
                chainLightning.DoLightning(incomingObj);

            if(iDamageInterface == null)
            {
                iDamageInterface = incomingObj.GetComponent<IDamageInterface>();
            }

            DamageEnemy(iDamageInterface, incomingDamage, weakSpotShot);
            UILogic(weakSpotShot);
            if (weakSpotShot)
                weakSpotHitCondition?.Invoke(1);
        }
        else
        {
            print("Dealing Damage 2 : side");
            DamagePlayer(incomingObj, incomingDamage, enemyBase);
        }
    }

    #region DealingDamageToenemies
    private void DamageEnemy(IDamageInterface damageInterface, float incomingDamage, bool incomingWeakSpotShot)
    {
        print("Dealing Damage 3");
        CalculateDamage(damageInterface, incomingDamage, incomingWeakSpotShot);
    }

    //For enemies
    private void CalculateDamage(IDamageInterface damageInterface, float incomingDamage, bool incomingWeakSpotShot)
    {
        incomingDamage += playerStats.addedDamage;

        if(incomingWeakSpotShot)
        {
            incomingDamage *= 1.5f;
        }

        float totalMultiplier = 1f;

        if (playerStats.moneyIsPower > 0)
        {
            totalMultiplier += ((float)inventory.money * 0.03f / 100);
        }

        if (playerStats.hpIsPower > 0)
        {
            totalMultiplier += (playerHealth.maxHP / 4f) * 0.01f;

        }

        if(playerStats.increasedDamage > 0)
        {
            totalMultiplier += playerStats.increasedDamage;
        }

        incomingDamage *= totalMultiplier;

        int critChance = Random.Range(0, 100);


        if (playerStats.criticalChance > critChance)
        {
            PlaySFX(true);
            criHitReport?.Invoke(1);
            incomingDamage *= playerStats.criticalMultiplier;
            damageInterface.Damage(incomingDamage, true);
            return;
        }
        
        PlaySFX(false);
        print("Dealing Damage 4");
        damageInterface.Damage(incomingDamage, false);
    }

    private void PlaySFX(bool isCrit)
    {
        if (isCrit)
        {
            if (critHitSFX != null)
                Instantiate(critHitSFX.GetComponent<AudioSource>());

            if(playerStats.hpOnCritHit > 0)
                playerHealth.Heal(playerStats.hpOnCritHit, false);
        }
        else
        {
            if (hitSFX != null)
                Instantiate(hitSFX.GetComponent<AudioSource>());
        }

    }

    private void UILogic(bool weakSpotShot)
    {
        if (weakSpotShot)
            hitMarkerLogic.EnableCritHitMarker();
        else
            hitMarkerLogic.EnableHitMarker();
    }

    #endregion

    #region DealingDamageToPlayer

    private void DamagePlayer(GameObject incomingObj, float incomingDamage, EnemyBase enemyBase = null)
    {
        incomingObj.GetComponent<IDamageInterface>().Damage(incomingDamage, false, enemyBase);
    }

    #endregion

    public void HitEffect()
    {
        playerHealth.Heal(playerStats.hpOnHit, false);
        //healthRegen.StartHPRegen();
    }

}
