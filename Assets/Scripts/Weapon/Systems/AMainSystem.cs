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
    
    HitmarkerLogic hitMarkerLogic;
    PlayerStats playerStats;
    ShootSystem shootSystem;
    ExplosionSystem explosionSystem;

    private void Awake()
    {
        shootSystem = GetComponent<ShootSystem>();
        explosionSystem = GetComponent<ExplosionSystem>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
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

    public void SpawnExplosion(Vector3 position, float radius, int damage, GameObject parent, bool enemyOwned)
    {
        explosionSystem.SpawnExplosion(position, radius, damage, parent, enemyOwned);
    }


    //Damage calculations
    public void DealDamage(GameObject incomingObj, float incomingDamage, bool friendly)
    {
        if(friendly)
        {
            DamageEnemy(incomingObj, incomingDamage);
        }
        else
        {
            DamagePlayer(incomingObj, incomingDamage);
        }
    }

    #region DealingDamageToenemies
    private void DamageEnemy(GameObject incomingObj, float incomingDamage)
    {
        float finalDamage = CalculateDamage(incomingDamage);

        incomingObj.GetComponent<IDamageInterface>().Damage(incomingDamage);
    }

    private float CalculateDamage(float incomingDamage)
    {
        incomingDamage += playerStats.damage;

        int critChance = Random.Range(0, 100);

        if (playerStats.criticalChance > critChance)
        {
            PlaySFX(true);
            return incomingDamage *= playerStats.criticalMultiplier;
        }
        
        PlaySFX(false);
        return incomingDamage;
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

    private void UILogic()
    {
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
