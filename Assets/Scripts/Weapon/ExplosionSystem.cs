using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSystem : MonoBehaviour
{
    public GameObject standardExplosion;
    private AMainSystem mainSystem;
    private PlayerStats playerStats;

    private void Awake()
    {
        mainSystem = GetComponent<AMainSystem>();
        PlayerStats playerStats;
        
    }

    private void Start()
    {
        playerStats = mainSystem.playerStats;
    }

    public void SpawnExplosion(Vector3 position, float radius, int damage, GameObject parent = null, BaseWeapon weaponParent = null, EnemyBase enemyBase = null)
    {
        GameObject explosion = Instantiate(
            standardExplosion,
            position,
            Quaternion.identity,
            parent != null ? parent.transform : null
        );

        if (parent != null)
        {
            var enemy = parent.GetComponent<EnemyBase>();
            if (enemy != null)
                enemy.SetProjetile(explosion);
        }

        radius = (radius + playerStats.addExplosiveRadius) * (1f + playerStats.increaseExplosiveRadius / 100f);
        damage = Mathf.RoundToInt((damage + playerStats.addExplosiveDamage) * (1f + playerStats.increaseExplosiveDamage / 100f));

        explosion.GetComponent<IExplosionInterface>()
            .InitiateExplosion(mainSystem, radius, damage, enemyBase, weaponParent);
    }

    //public void SpawnExplosion2(Vector3 position, float radius, int damage, GameObject parent = null, BaseWeapon weaponParent = null, EnemyBase enemyBase = null)
    //{
    //    GameObject explosion = Instantiate(
    //        standardExplosion,
    //        position,
    //        Quaternion.identity,
    //        parent != null ? parent.transform : null
    //    );

    //    if (enemyBase != null)
    //    {
    //        enemyBase.SetProjetile(explosion);
    //    }

    //    bool enemyOwned = enemyBase != null;

    //    explosion.GetComponent<IExplosionInterface>()
    //        .InitiateExplosion(mainSystem, radius, damage, enemyOwned, weaponParent);
    //}

}
