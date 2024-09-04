using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AMainSystem : MonoBehaviour
{

    public ExplosionSystem explosionSystem;
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

    public ShootSystem shootSystem;

    
    

}
