using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningStatusEffect : StatusEffectBase
{
    void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartEffect(100, 1f); // frequency = 1s, damage per stack = 1
        }
    }

    public override void DoEffect(float totalDamage)
    {
        // Apply total damage scaled by active stacks
        mainSystem.DealDamage(gameObject, totalDamage, true, false, iDamageInterface: damageInterface);
    }
}
