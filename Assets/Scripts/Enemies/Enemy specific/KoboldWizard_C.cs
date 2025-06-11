using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    [Header("shield")]
    public GameObject kobold;
    public Transform spawnPos;
    public GameObject spawnVFX;
    public GameObject spawnSFX;

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
                SpawnKobold();
                //StartFireSpire();
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
        StartAttack(0);
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
        StartAttack(0);
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

    #region Projectile

    private void SpawnKobold()
    {
        StartAttack(0);
        StartCoroutine(SpawnKoboldTelegraph());
    }

    private  IEnumerator SpawnKoboldTelegraph()
    {
        animator.SetBool("Evocation", true);

        
        Vector3 randomPosition = GetRandomNavMeshPosition(transform.position, 15, 30);

        Instantiate(spawnVFX, randomPosition, transform.rotation);

        yield return new WaitForSeconds(1);

        StartCoroutine(DoSpawnKobold(randomPosition));
    }

    private IEnumerator DoSpawnKobold(Vector3 position)
    {
        Instantiate(kobold, position, transform.rotation);

        yield return new WaitForSeconds(0.1f);

        EndAttack();
    }

    Vector3 GetRandomNavMeshPosition(Vector3 center, float minRange, float maxRange)
    {
        for (int i = 0; i < 30; i++) // attempt up to 30 times
        {
            // First: pick a random direction on the horizontal plane (XZ)
            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minRange, maxRange);
            Vector3 randomDirection = new Vector3(randomCircle.x, 0, randomCircle.y);
            randomDirection += center;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 2.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        // fallback: return original position
        return center;
    }

    #endregion
}