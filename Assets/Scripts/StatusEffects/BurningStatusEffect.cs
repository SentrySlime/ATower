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
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    StartEffect(100, 1f); // frequency = 1s, damage per stack = 1
        //}
    }

    public override IEnumerator EffectTickLoop()
    {
        float nextTickTime = Time.time;

        while (activeStacks.Count > 0)
        {
            float now = Time.time;

            activeStacks.RemoveAll(stack => stack.expiryTime <= now);

            if (activeStacks.Count == 0)
                break;

            float effectiveFrequency = Mathf.Max(baseFrequency / activeStacks.Count, 0.05f);

            if (now >= nextTickTime)
            {
                float totalDamage = 0f;
                foreach (var stack in activeStacks)
                {
                    totalDamage += stack.damage;
                }


                DoEffect(totalDamage);
                nextTickTime = now + effectiveFrequency;
            }

            SetParticleStrength();

            yield return null;
        }

        KillStatusEffect();
    }

    public override void DoEffect(float totalDamage)
    {
        // Apply total damage scaled by active stacks
        mainSystem.DealDamage(gameObject, totalDamage, true, false, iDamageInterface: damageInterface);
    }
}
