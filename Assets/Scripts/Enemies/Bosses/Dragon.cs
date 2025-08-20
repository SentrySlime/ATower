using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dragon : Enemy_Movement
{
    [Header("//----- Wizard Logic -----\\")]

    [Header("FireSpire")]
    public float fireSpireDistance = 110;
    public GameObject fireSpireTelegraphVFX;
    public GameObject fireSpireVFX;
    public LayerMask fireSpireLayermask;
    bool canFireSpire = true;


    [Header("shield")]
    public GameObject barrier;
    public EnemyBase existingBarrier;
    bool canCreateBarrier = true;
    float barrierRate = 8;

    [Header("Summon")]
    public GameObject kobold;
    public Transform spawnPos;
    public GameObject spawnVFX;
    public GameObject spawnSFX;
    List<GameObject> spawnList = new List<GameObject>();
    bool canSummon = true;
    int minionCount = 0;

    [Header("Shard")]
    public GameObject shard;
    public Transform shardSpawnPoint;
    EnemyBase existingShard;
    bool canCreateShard = true;

    [Header("RangedAttack")]
    public GameObject shootTelegraphVFX;
    public GameObject shootExplosionSFX;
    public GameObject shootTelegraphVFXPosition;
    public GameObject shootProjectile;
    public GameObject cannonObject;
    public GameObject shootPoint;
    bool canRangedAttack = true;

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

        //if (canCreateBarrier)
        //    CreateBarrier();

        if (attackRateTimer < attackRate)
            attackRateTimer += Time.deltaTime;
        else
        {

            if(canRangedAttack)
                RangedAttack();
            else if (canCreateBarrier)
                CreateBarrier();
            else if (canFireSpire && DistanceAndLineOS())
                StartFireSpire();
            else if (canSummon && CanSummon())
                SpawnKobold();
            //else if (canCreateShard)
                //SpawnShard();
        }
    }

    private bool DistanceAndLineOS()
    {
        if (playerDistance < fireSpireDistance && HasLineOfSight())
            return true;
        else
            return false;
    }

    public override void EndAttack()
    {
        base.EndAttack();
        //animator.SetBool("Evocation", false);
    }

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

        //animator.SetBool("Evocation", true);
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

    #region FireSpire

    private void StartFireSpire()
    {
        canFireSpire = false;
        //animator.SetBool("Evocation", true);
        StartCoroutine(FireSpireTelegraph());
    }

    IEnumerator FireSpireTelegraph()
    {
        StartAttack(0);

        Vector3 fireSpirePos = player.transform.position;

        RaycastHit hit;
        if (Physics.Raycast(fireSpirePos, -transform.up, out hit, 20, fireSpireLayermask))
        {
            fireSpirePos = hit.point;

            if (fireSpireTelegraphVFX)
            {
                Instantiate(fireSpireTelegraphVFX, fireSpirePos, Quaternion.identity);
            }
        }
        else
        {
            if (fireSpireTelegraphVFX)
            {
                Instantiate(fireSpireTelegraphVFX, fireSpirePos, Quaternion.identity);
            }
        }

        yield return new WaitForSeconds(1);
        FireSpire(fireSpirePos);
    }

    private void FireSpire(Vector3 spawnPos)
    {
        //animator.SetBool("Evocation", true);
        Instantiate(fireSpireVFX, spawnPos, Quaternion.identity);
        StartCoroutine(FireSpireCooldown());
        EndAttack();
    }

    IEnumerator FireSpireCooldown()
    {
        yield return new WaitForSeconds(14);
        canFireSpire = true;
    }

    #endregion

    #region Summon

    private void SpawnKobold()
    {
        canSummon = false;
        StartAttack(0);

        for (int i = 0; i < 2; i++)
        {
            StartCoroutine(SpawnKoboldTelegraph());
        }
    }

    private IEnumerator SpawnKoboldTelegraph()
    {
        //animator.SetBool("Evocation", true);

        Vector3 randomPosition = GetRandomNavMeshPosition(transform.position, 15, 30);

        Instantiate(spawnVFX, randomPosition, transform.rotation);

        yield return new WaitForSeconds(1);

        StartCoroutine(DoSpawnKobold(randomPosition));
    }

    private IEnumerator DoSpawnKobold(Vector3 position)
    {
        GameObject minion = Instantiate(kobold, position, transform.rotation);

        print("1");

        spawnList.Add(minion);
        if (enemyBase.roomScript)
            enemyBase.roomScript.AddEnemy(minion);
        minion.GetComponent<EnemyBase>().koboldWizard = this;
        minionCount++;


        yield return new WaitForSeconds(0.1f);

        StartCoroutine(SummonCooldown());
        EndAttack();
    }

    Vector3 GetRandomNavMeshPosition(Vector3 center, float minRange, float maxRange)
    {
        for (int i = 0; i < 30; i++)
        {

            Vector2 randomCircle = Random.insideUnitCircle.normalized * Random.Range(minRange, maxRange);
            Vector3 randomDirection = new Vector3(randomCircle.x, 0, randomCircle.y);
            randomDirection += center;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, 2.0f, NavMesh.AllAreas))
            {
                Vector3 origin = visionPoint != null ? visionPoint.position : center + Vector3.up * 1.5f;

                Vector3 dir = hit.position - origin;

                float distance = dir.magnitude;

                if (!Physics.Raycast(origin, dir.normalized, distance, layerMask))
                {
                    return hit.position;
                }
            }
        }

        return center;
    }

    IEnumerator SummonCooldown()
    {
        yield return new WaitForSeconds(11);
        canSummon = true;
    }

    private bool CanSummon()
    {
        if (minionCount < 4)
            return true;
        else
            return false;
    }

    public void DecreaseMinionCount(GameObject gameObject)
    {
        spawnList.Remove(gameObject);
        minionCount--;
    }

    #endregion

    #region Shard

    private void SpawnShard()
    {
        canCreateShard = false;
        StartAttack(0);
        StartCoroutine(DoSpawnShard());
    }

    private IEnumerator DoSpawnShard()
    {
        yield return new WaitForSeconds(1);

        GameObject tempShard = Instantiate(shard, shardSpawnPoint.position, transform.rotation, shardSpawnPoint);
        existingShard = tempShard.GetComponent<EnemyBase>();
        existingShard.OnEnemyDied += ShardReset;

        EndAttack();
    }

    public void ShardReset(EnemyBase barrier)
    {
        StartCoroutine(ShardCooldown());
    }

    IEnumerator ShardCooldown()
    {
        yield return new WaitForSeconds(5);
        canCreateShard = true;
    }


    #endregion

    #region RangedAttack

    private void RangedAttack()
    {
        canRangedAttack = false;
        StartAttack(0);
        StartCoroutine(InitiateShootAttack());
    }

    IEnumerator InitiateShootAttack()
    {
        cannonObject.transform.localRotation = new Quaternion(-0.0697634518f, -0.094860889f, 0.130030558f, 0.984493077f);
        if (shootTelegraphVFX)
            Instantiate(shootTelegraphVFX, shootTelegraphVFXPosition.transform.position, shootTelegraphVFXPosition.transform.rotation, shootTelegraphVFXPosition.transform);
        else yield return null;

        yield return new WaitForSeconds(1.5f);
        StartCoroutine(ShootProjectile());
        Invoke("ShootProjectile", 1.5f);
    }

    IEnumerator ShootProjectile()
    {
        if (shootExplosionSFX)
            Instantiate(shootExplosionSFX, shootTelegraphVFXPosition.transform.position, Quaternion.identity);
        else yield return null;

        for (int i = 0; i < 6; i++)
        {
            Vector3 direction = playerTargetPoint.transform.position - shootPoint.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            float xRotation = Random.Range(-3, 3);
            float yRotation = Random.Range(-3, 3);

            Quaternion randomRotation = Quaternion.Euler(xRotation, yRotation, 0) * lookRotation;

            if (shootProjectile)
                Instantiate(shootProjectile, shootPoint.transform.position, randomRotation).GetComponent<EnemyProjectile>().Initialize(enemyBase);
            else yield return null;


            yield return new WaitForSeconds(0.1f);
        }



        //Quaternion leftRotation = Quaternion.Euler(0, -15, 0) * lookRotation;
        //if (shootProjectile)
        //    Instantiate(shootProjectile, shootPoint.transform.position, leftRotation).GetComponent<EnemyProjectile>().Initialize(enemyBase);
        //else yield return null;

        //Quaternion rightRotation = Quaternion.Euler(0, 15, 0) * lookRotation;
        //if (shootProjectile)
        //    Instantiate(shootProjectile, shootPoint.transform.position, rightRotation).GetComponent<EnemyProjectile>().Initialize(enemyBase);
        //else yield return null;

        StartCoroutine(RangedAttackCooldown());
        EndAttack();
    }

    IEnumerator RangedAttackCooldown()
    {
        yield return new WaitForSeconds(14);
        canRangedAttack = true;
    }

    #endregion

}