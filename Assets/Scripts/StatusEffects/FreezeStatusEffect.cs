using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeStatusEffect : StatusEffectBase
{

    public ParticleSystem flashFrrezePS;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            StartEffect(100, 1f); // frequency = 1s, damage per stack = 1unity 
        }
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

            
            SetParticleStrength();

            yield return null;
        }

        KillStatusEffect();
    }

    public override void DoEffect(float totalDamage)
    {
        //mainSystem.DealDamage(gameObject, totalDamage, true, false, iDamageInterface: damageInterface);
    }

    public override void ImmediateEffect()
    {
        if (statusEffectInterface == null) return;

        flashFrrezePS.Play();
        statusEffectInterface.Freeze();

        //Getting animator speed then setting it
        //if (animator)
        //{
        //    animatorSpeed = animator.speed;
        //    animator.speed = 0;
        //}

        ////Getting move speed then setting it
        //moveSpeed = enemyMovement.agent.speed;
        //enemyMovement.agent.speed = 0;
        //enemyMovement.agent.isStopped = true;

        //enemyMovement.frozen = true;
    }

    public override void EndEffect()
    {

        if (statusEffectInterface == null) return;

        statusEffectInterface.UnFreeze();

        //// End effects here
        //if (animator)
        //    animator.speed = animatorSpeed;
        //enemyMovement.agent.isStopped = false;
        //enemyMovement.agent.speed = moveSpeed;
        //enemyMovement.frozen = false;
    }

    public override bool ImmediateEffectCondition()
    {
        if (activeStacks.Count >= 10)
            return true;
        else
            return false;

    }

    public override bool ReturnCondition()
    {
        if (statusEffectInterface == null) return false;

        return statusEffectInterface.IsFrozen();
    }
}
