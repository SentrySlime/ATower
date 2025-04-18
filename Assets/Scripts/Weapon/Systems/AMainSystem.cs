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
    Inventory inventory;
    ShootSystem shootSystem;
    HitmarkerLogic hitMarkerLogic;
    ExplosionSystem explosionSystem;

    private void Awake()
    {
        shootSystem = GetComponent<ShootSystem>();
        explosionSystem = GetComponent<ExplosionSystem>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        playerHealth = player.GetComponent<PlayerHealth>();
        inventory = player.GetComponent<Inventory>();
        hitMarkerLogic = GameObject.FindGameObjectWithTag("HitMarker").GetComponent<HitmarkerLogic>();
    }

    public void SpawnExplosion(Vector3 position, float radius, int damage)
    {
        explosionSystem.SpawnExplosion(position, radius, damage);
    }

    public void SpawnExplosion(Vector3 position, float radius, int damage, GameObject parent)
    {
        explosionSystem.SpawnExplosion(position, radius, damage, parent);
    }

    public void SpawnExplosion(Vector3 position, float radius, int damage, bool enemyOwned)
    {
        explosionSystem.SpawnExplosion(position, radius, damage, enemyOwned);
    }

    public void SpawnExplosion(Vector3 position, float radius, int damage, GameObject parent, bool enemyOwned)
    {
        explosionSystem.SpawnExplosion(position, radius, damage, parent, enemyOwned);
    }



    public void SpawnPickUpEffects(Vector3 position)
    {
        if(pickUpVFX)
            Instantiate(pickUpVFX, position, Quaternion.identity);
    }

    //Damage calculations
    public void DealDamage(GameObject incomingObj, float incomingDamage, bool friendly)
    {
        if(friendly)
        {
            DamageEnemy(incomingObj, incomingDamage, false);
            UILogic(false);
        }
        else
        {
            DamagePlayer(incomingObj, incomingDamage);
        }
    }

    public void DealDamage(GameObject incomingObj, float incomingDamage, bool friendly, bool weakSpotShot)
    {
        if (friendly)
        {
            DamageEnemy(incomingObj, incomingDamage, weakSpotShot);
            UILogic(weakSpotShot);
        }
        else
        {
            DamagePlayer(incomingObj, incomingDamage);
        }
    }

    #region DealingDamageToenemies
    private void DamageEnemy(GameObject incomingObj, float incomingDamage, bool incomingWeakSpotShot)
    {
        CalculateDamage(incomingObj, incomingDamage, incomingWeakSpotShot);
        playerHealth.Heal(playerStats.hpOnHit);
        
    }

    private void CalculateDamage(GameObject incomingObj, float incomingDamage, bool incomingWeakSpotShot)
    {
        incomingDamage += playerStats.damage;

        if(incomingWeakSpotShot)
        {
            incomingDamage *= 1.5f;
        }

        if (playerStats.moneyIsPower > 0)
        {
            float percentage = 1 + ((float) inventory.Money / 10000);
            incomingDamage *= percentage;   
        }
        int critChance = Random.Range(0, 100);

        if (playerStats.criticalChance > critChance)
        {
            PlaySFX(true);
            incomingDamage *= playerStats.criticalMultiplier;
            incomingObj.GetComponent<IDamageInterface>().Damage(incomingDamage, true);
            return;
        }
        
        PlaySFX(false);

        incomingObj.GetComponent<IDamageInterface>().Damage(incomingDamage);
    }

    private void PlaySFX(bool isCrit)
    {
        if (isCrit)
        {
            if (critHitSFX != null)
                Instantiate(critHitSFX.GetComponent<AudioSource>());
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

    private void DamagePlayer(GameObject incomingObj, float incomingDamage)
    {
        incomingObj.GetComponent<IDamageInterface>().Damage(incomingDamage);
    }

    #endregion
}
