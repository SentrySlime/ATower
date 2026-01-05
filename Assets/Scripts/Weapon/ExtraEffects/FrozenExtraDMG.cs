using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrozenExtraDMG : MonoBehaviour, IAdjustDamage
{
    public float AdjustDamage(float damage, GameObject enemyRoot)
    {
        if (enemyRoot.TryGetComponent<IStatusEffect>(out var statusEffect) && statusEffect.IsFrozen())
        {
            return damage *= 10f;
        }
        else
            return damage;

    }


}
