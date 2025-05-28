using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troll_C : Enemy_Movement
{
    [Header("//----- Troll stuff -----\\")]

    public float rangedDistance = 20;


    [Header("MeleeAttack")]
    public float meleeDistance = 25;
    public GameObject donutExplosionVFX;
    public GameObject donutExplosionSFX;
    public GameObject donutExplosion;
    public Transform donutExplosionSpawnTransform;
    public LayerMask donutLayerMask;
    float donutExplosionTimer = 0.4f;

    [Header("RangedAttack")]
    public GameObject shootTelegraphVFX;
    public GameObject shootExplosionSFX;
    public GameObject shootTelegraphVFXPosition;
    public GameObject shootProjectile;
    public GameObject cannonObject;
    public GameObject shootPoint;

    [Header("Animation")]
    public Animator cannonAnimator;
    float animationTimer = 0.6f;
    float speedTimer = 1.75f;


    void Start()
    {
        base.Start();
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
                MeleeAttack();
            }
            else if (playerDistance < rangedDistance)
            {
                if (IsPlayerInfront() && HasLineOfSight())
                {
                    RangedAttack();
                }
            }
        }
    }

    public override void StartAttack(float speed)
    {
        base.StartAttack(speed);
    }

    public override void EndAttack()
    {
        base.EndAttack();
        ResetMeleeAnimation();
        cannonObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
    }

    #region RangedAttack

    private void RangedAttack()
    {
        StartAttack(0);
        InitiateShootAttack();
    }

    private void InitiateShootAttack()
    {

        

        cannonObject.transform.localRotation = new Quaternion(-0.0697634518f, -0.094860889f, 0.130030558f, 0.984493077f);
        if (shootTelegraphVFX)
            Instantiate(shootTelegraphVFX, shootTelegraphVFXPosition.transform.position, shootTelegraphVFXPosition.transform.rotation, shootTelegraphVFXPosition.transform);
        Invoke("ShootProjectile", 1.5f);
    }


    private void ShootProjectile()
    {
        if (shootExplosionSFX)
            Instantiate(shootExplosionSFX, shootTelegraphVFXPosition.transform.position, Quaternion.identity);

        Vector3 direction = playerTargetPoint.transform.position - shootPoint.transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        if (shootProjectile)
            Instantiate(shootProjectile, shootPoint.transform.position, lookRotation).GetComponent<EnemyProjectile>().Initialize(enemyBase);

        Quaternion leftRotation = Quaternion.Euler(0, -15, 0) * lookRotation;
        if (shootProjectile)
            Instantiate(shootProjectile, shootPoint.transform.position, leftRotation).GetComponent<EnemyProjectile>().Initialize(enemyBase);

        Quaternion rightRotation = Quaternion.Euler(0, 15, 0) * lookRotation;
        if (shootProjectile)
            Instantiate(shootProjectile, shootPoint.transform.position, rightRotation).GetComponent<EnemyProjectile>().Initialize(enemyBase);


        EndAttack();
    }


    #endregion

    #region MeleeAttack

    private void MeleeAttack()
    {
        StartAttack(0);
        MeleeAnimation();
    }

    private void MeleeAnimation()
    {
        if (animator)
        {
            animator.SetBool("Melee", true);
            animator.speed = 1f;
        }

        if (cannonAnimator)
        {
            cannonAnimator.SetBool("Melee", true);
            cannonAnimator.speed = 1f;
        }
        Invoke("IncreaseSpeed", speedTimer);
    }

    private void IncreaseSpeed()
    {
        if (animator)
        {
            animator.speed = 2.5f;
        }

        if (cannonAnimator)
        {
            cannonAnimator.speed = 2.5f;
        }

        Invoke("Idle", animationTimer);
        Invoke("DonutExplosion", donutExplosionTimer);
    }

    private void DonutExplosion()
    {
        if (donutExplosionVFX)
            Instantiate(donutExplosionVFX, donutExplosionSpawnTransform.position + new Vector3(0, -6.5f, 0), Quaternion.identity);
        if (donutExplosionSFX)
            Instantiate(donutExplosionSFX, donutExplosionSpawnTransform.position + new Vector3(0, -6.5f, 0), Quaternion.identity);

        if (donutExplosion)
        {
            RaycastHit hit;
            if (Physics.Raycast(donutExplosionSpawnTransform.position, -transform.up, out hit, 20, donutLayerMask))
            {
                if (donutExplosion)
                {
                    Instantiate(donutExplosion, hit.point + new Vector3(0, 2.5f, 0), Quaternion.identity).GetComponentInChildren<IInitializeProjectile>().Initialize(enemyBase);

                }
            }
        }

        EndAttack();
    }

    private void ResetMeleeAnimation()
    {
        if (animator)
        {
            animator.SetBool("Melee", false);
            animator.speed = 1f;

        }

        if (cannonAnimator)
        {
            cannonAnimator.SetBool("Melee", false);
            cannonAnimator.speed = 1f;
        }
    }

    #endregion

    public void LeftFootVFX()
    {
        //if (footStepVFX)
        //    Instantiate(footStepVFX, leftfoot.transform.position + new Vector3(0, -1, 0), Quaternion.identity);
    }

    public void RightFootVFX()
    {
        //if (footStepVFX)
        //    Instantiate(footStepVFX, rightfoot.transform.position + new Vector3(0, -1, 0), Quaternion.identity);
    }

}
