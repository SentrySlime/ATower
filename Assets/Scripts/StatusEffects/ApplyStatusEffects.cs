using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyStatusEffects : MonoBehaviour
{
    //---------- Burning ----------\\
    [Header("Burning Effect")]
    public bool canBurn = false;
    public float burningDamage = 1;

    [Tooltip("1 is equal to 1% chance to apply the status effect")]
    [Range(0f, 1000f)] public int burnChance = 0;

    //---------- Freeze ----------\\
    [Header("Freeze Effect")]
    public bool canFreeze = false;

    [Tooltip("1 is equal to 1% chance to apply the status effect")]
    [Range(0f, 1000f)] public int freezeChance = 0;

    public void InitiateEffect(GameObject enemyRoot)
    {
        if(canBurn)
        {
            if (enemyRoot.TryGetComponent<BurningStatusEffect>(out var burn))
                burn.StartEffect(burnChance, burningDamage);
        }

        if (canFreeze)
        {
            if (enemyRoot.TryGetComponent<FreezeStatusEffect>(out var freeze))
                freeze.StartEffect(freezeChance, 0);
        }
    }

}
