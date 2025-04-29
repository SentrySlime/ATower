using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSystem : MonoBehaviour
{

    public GameObject standardExplosion;
    private GameObject tempExplosions;   
    private AMainSystem mainSystem;

    private void Awake()
    {
        mainSystem = GetComponent<AMainSystem>();
    }

    public void SpawnExplosion(Vector3 explosionPos, float explosionRadius, int damage)
    {
        tempExplosions = Instantiate(standardExplosion, explosionPos, Quaternion.identity);
        tempExplosions.GetComponent<IExplosionInterface>().InitiateExplosion(mainSystem, explosionRadius, damage, false, null);
    }

    public void SpawnExplosion(Vector3 explosionPos, float explosionRadius, int damage, GameObject parent, BaseWeapon weaponParent)
    {
        tempExplosions = Instantiate(standardExplosion, explosionPos, Quaternion.identity, parent.transform);
        
        if(parent.GetComponent<EnemyBase>())
            parent.GetComponent<EnemyBase>().SetProjetile(tempExplosions);
        
        tempExplosions.GetComponent<IExplosionInterface>().InitiateExplosion(mainSystem, explosionRadius, damage, false, weaponParent);
    }

    public void SpawnExplosion(Vector3 explosionPos, float explosionRadius, int damage, BaseWeapon weaponParent)
    {
        tempExplosions = Instantiate(standardExplosion, explosionPos, Quaternion.identity);
        tempExplosions.GetComponent<IExplosionInterface>().InitiateExplosion(mainSystem, explosionRadius, damage, false, weaponParent);
    }

    public void SpawnExplosion(Vector3 explosionPos, float explosionRadius, int damage, GameObject parent, bool enemyOwned)
    {
        tempExplosions = Instantiate(standardExplosion, explosionPos, Quaternion.identity, parent.transform);
        
        if (parent.GetComponent<EnemyBase>())
            parent.GetComponent<EnemyBase>().SetProjetile(tempExplosions);

        tempExplosions.GetComponent<IExplosionInterface>().InitiateExplosion(mainSystem, explosionRadius, damage, true, null);
    }

    public void SpawnExplosion(Vector3 explosionPos, float explosionRadius, int damage, bool enemyOwned)
    {
        tempExplosions = Instantiate(standardExplosion, explosionPos, Quaternion.identity);

        tempExplosions.GetComponent<IExplosionInterface>().InitiateExplosion(mainSystem, explosionRadius, damage, true, null);
    }

}
