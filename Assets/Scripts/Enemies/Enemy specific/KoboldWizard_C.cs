using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KoboldWizard_C : Enemy_Movement
{
    [Header("//----- Wizard Logic -----\\")]

    [Header("FireSpire")]
    public float fireSpireDistance = 110;
    public GameObject fireSpireTelegraphVFX;
    public GameObject fireSpireVFX;

    [Header("shield")]
    public GameObject barrier;
    public EnemyBase existingBarrier;
    public bool canCreateBarrier = true;
    float barrierRate = 6;

    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    public override void AttackLogic()
    {

        if (isAttacking) return;
        
        if (canCreateBarrier)
            CreateBarrier();

        if (attackRateTimer < attackRate)
            attackRateTimer += Time.deltaTime;
        else
        {

            if(playerDistance < fireSpireDistance && HasLineOfSight())
            {
                StartFireSpire();
            }

            //if (playerDistance < meleeDistance)
            //{
            //    MeleeAttack();
            //}
            //else if (playerDistance < rangedDistance)
            //{
            //    if (IsPlayerInfront() && HasLineOfSight())
            //    {
            //        RangedAttack();
            //    }
            //}
        }
    }

    public override void EndAttack()
    {
        base.EndAttack();
        animator.SetBool("Evocation", false);
    }

    #region FireSpire

    private void StartFireSpire()
    {
        animator.SetBool("Evocation", true);
        StartCoroutine(FireSpireTelegraph());
    }

    IEnumerator FireSpireTelegraph()
    {
        StartAttack();
        Vector3 fireSpirePos = player.transform.position;
        Instantiate(fireSpireTelegraphVFX, fireSpirePos, Quaternion.identity);
        yield return new WaitForSeconds(1);
        FireSpire(fireSpirePos);
    }

    private void FireSpire(Vector3 spawnPos)
    {
        animator.SetBool("Evocation", true);
        Instantiate(fireSpireVFX, spawnPos, Quaternion.identity);
        EndAttack();
    }

    #endregion

    #region Barrier

    private void CreateBarrier()
    {
        StartAttack();
        StartCoroutine(CreateBarrierCoroutine());
    }

    IEnumerator CreateBarrierCoroutine()
    {
        canCreateBarrier = false;
        GameObject tempBarrier = Instantiate(barrier, transform.position, Quaternion.identity, transform);
        existingBarrier = tempBarrier.GetComponent<EnemyBase>();

        existingBarrier.OnEnemyDied += BarrierReset;
        
        animator.SetBool("Evocation", true);
        yield return new WaitForSeconds(1);
        EndAttack();
    }

    IEnumerator ResetBarrier()
    {
        yield return new WaitForSeconds(barrierRate);
        canCreateBarrier = true;
    }


    public void BarrierReset(EnemyBase barrier)
    {
        StartCoroutine(ResetBarrier());
    }

    #endregion
}