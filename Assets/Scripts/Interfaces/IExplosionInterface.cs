using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IExplosionInterface
{
    void InitiateExplosion(AMainSystem mainsystem, float explosionRadius, int damage, EnemyBase enemyBase, BaseWeapon weaponParent);
}
