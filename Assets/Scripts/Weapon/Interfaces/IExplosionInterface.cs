using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExplosionInterface
{
    //void InitiateExplosion(float explosionRadius, int damage);
    void InitiateExplosion(float explosionRadius, int damage, bool enemyOwned);
}
