using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kobold_C : Enemy_Movement
{
    [Header("MeleeAttack")]
    float meleeDistance = 6;
    float meleeDamage = 15;
    public AudioSource meleeAudioSource;


    [Header("RangedAttack")]
    float rangedDistance = 20;
    public GameObject projectile;
    public Transform shootPoint;
    public AudioSource shootAudioSource;

    [Header("RangedTelegraph")]
    public GameObject telegraphSphere;
    public AudioSource chargeUpAudioSource;

    void Start()
    {
        base.Start();
        SetRandomStats();
    }
    
    void Update()
    {
        base.Update();
    }

    public override void AttackLogic()
    {
        if (isAttacking) return;

        if (attackRateTimer < attackRate)
            attackRateTimer += Time.deltaTime;
        else
        {
            if (playerDistance < meleeDistance)
            {
                MeleeTelegraphAttack();
            }
            else if (playerDistance < rangedDistance)
            {
                if (IsPlayerInfront() && HasLineOfSight())
                {
                    RangedTelegraphAttack();
                }
            }
        }
    }

    public override void StartAttack(float speed)
    {
        base.StartAttack(speed);
    }

    #region MeleeAttack

    private void MeleeTelegraphAttack()
    {
        StartAttack(0);
        animator.SetBool("MeleeAttack", true);
        MeleeAttack();
    }

    public void MeleeAttack()
    {
        if(meleeAudioSource)
            meleeAudioSource.Play();

        StartCoroutine(MeleeAttackCoroutine());
    }

    IEnumerator MeleeAttackCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        aMainSystem.DealDamage(player, meleeDamage, false, false, enemyBase);
        EndAttack();
    }

    #endregion

    #region RangedAttack


    private void RangedTelegraphAttack()
    {
        StartAttack(0);
        animator.SetBool("RangedAttack", true);
    }

    public void RangedAttack()
    {
        if(chargeUpAudioSource)
            chargeUpAudioSource.Play();
        
        if(telegraphSphere)
            telegraphSphere.SetActive(true);
        
        StartCoroutine(RangedAttackCoroutine());
    }

    IEnumerator RangedAttackCoroutine()
    {
        yield return new WaitForSeconds(0.25f);

        if (enemyBase.dead) { yield return null; }

        shootAudioSource.Play();

        Vector3 directionFromShootPoint = playerTargetPoint.transform.position - shootPoint.transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(directionFromShootPoint);
        EnemyProjectile enemyProjectile = Instantiate(projectile, shootPoint.position, lookRotation).GetComponent<EnemyProjectile>();
        enemyProjectile.Initialize(enemyBase);

        EndAttack();
    }

    public override void EndAttack()
    {
        telegraphSphere.SetActive(false);        
        animator.SetBool("RangedAttack", false);
        animator.SetBool("MeleeAttack", false);
        agent.speed = movementSpeed;
        base.EndAttack();
    }

    #endregion

    public override bool ReasonToStop()
    {
        Vector3 enemyforward = transform.forward;

        float dot = Vector3.Dot(enemyforward, directionToPlayer);

        

        if (dot < 0.2 && playerDistance < rangedDistance)
        {
            return false;
        }
        else if (playerDistance < rangedDistance)
            return true;
        else
            return false;
    }

    private void SetRandomStats()
    {
        float randomRange = UnityEngine.Random.Range(1, 100);
     
        if (randomRange >= 50)
        {
            rangedDistance = UnityEngine.Random.Range(30, 35);
        }
        else
        {
            rangedDistance = UnityEngine.Random.Range(20, 25);
        }

        attackRate = UnityEngine.Random.Range(0.8f, 1.5f);
    }
}