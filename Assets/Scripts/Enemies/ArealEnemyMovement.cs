using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;

public class ArealEnemyMovement : MonoBehaviour
{


    [Header("Movement")]
    public NavMeshAgent agent;
    
    public float movementSpeed = 0.1f;
    public float angularSpeed = 2000;

    float minDistance = 1;
    GameObject player;
    Vector3 previousPlayerPosition;
    public float turnRadius = 3;
    public float maxDelta = 3;

    [Header("Duration & Timers")]
    [Tooltip("The over time between any type of attack")]
    public float attackRate = 4;
    float attackTimer = 0;

    public float beamDuration = 4;
    float beamTimer = 0;

    public float projectileDuration = 2;
    float projectileTimer = 0;

    public float rangedAttackDist = 10;

    [Header("Projectile Attack")] // Projectiles
    public float projectileCooldown = 3;
    float projectileCooldownTimer = 0;

    public float projectileFireRate = 0.25f;
    float projectileFireRateTimer = 0;

    [Tooltip("Increase to shoot lower, Decrease to shoot higher")]
    public float yOffset = 0;

    [Tooltip("Decides how many projectiles to shoot")]
    public float projectileAmount = 1;

    [Tooltip("When shooting a shotgun, decides the distance between the projectiles")]
    public float projectileSpacing = 10;

    public GameObject shootPoint;
    public GameObject projectile;
    
    bool projectileAttacking = false;
    float Accuracy = 5;

    [Header("Beam Attack")] //Beams
    public float beamCooldown = 10;
    float beamCooldownTimer = 0;
    
    public GameObject partToRotate;
    public GameObject telegraphVFX;
    public GameObject telegraphPosition;
    
    bool beamAttacking = false;

    float beamDamage = 10;
    public float beamFollowSpeed = 0.26f;
    public float maxBeamFollowSpeed = 0.26f;
    LineRenderer lineRenderer;


    [Header("Melee Attack")] //Melee
    public float meleeCooldown = 4;
    float meleeCooldownTimer = 0;
    
    public float telegraphScaleSpeed = 5;
    public float meleeRange = 10;

    float meleeRadius = 1;
    public int meleeDamage = 10;
    public GameObject attackVFX;
    public GameObject telegraphVFX2;
    public GameObject expandingAttackPosition;

    public LayerMask meleeLayerMask;

    bool meleeAttacking = false;

    [Header("Notice player")]
    public float noticePlayerRange = 75;
    public LayerMask layerMask;
    bool foundPlayer = false;
    bool canSeePlayer = false;

    [Header("Avoidance")]
    Vector3 frontPosition;
    public float avoidanceDistance = 15;

    [Header("Misc")]
    int burstCount = 0;
    public float burstTimer;
    bool chasingPlayer = false;
    bool burst = false;
    AMainSystem aMainSystem;

    public AudioSource attackSFX;

    void Start()
    {
        player = GameObject.Find("PlayerTargetPoint");
        aMainSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        previousPlayerPosition = transform.position;
        //layerMask = ~layerMask;
    }

    void Update()
    {
        if (!foundPlayer)
            IdleBehaviour();

        if (beamCooldownTimer < beamCooldown && !beamAttacking)
            beamCooldownTimer += Time.deltaTime;

        if(projectileCooldownTimer < projectileCooldown && !projectileAttacking)
            projectileCooldownTimer += Time.deltaTime;

        if (meleeCooldownTimer < meleeCooldown && !meleeAttacking)
            meleeCooldownTimer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(player.transform.position, shootPoint.transform.position);
        Vector3 directionToPlayer = player.transform.position - shootPoint.transform.position;

        RaycastHit hit;
        if (Physics.Raycast(shootPoint.transform.position, directionToPlayer, out hit, rangedAttackDist + 5, layerMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                previousPlayerPosition = hit.transform.position;
                canSeePlayer = true;
            }
            else
                canSeePlayer = false;
        }

        if (distanceToPlayer > rangedAttackDist && !beamAttacking && !meleeAttacking || !canSeePlayer)
        {
            NavMeshMove();
        }
        else if (attackTimer < attackRate && !beamAttacking && !meleeAttacking)
        {
            attackTimer += Time.deltaTime;
        }
        else if (attackTimer >= attackRate && !beamAttacking)
        {
            if (distanceToPlayer <= meleeRange && meleeCooldownTimer >= meleeCooldown)
            {
                meleeAttacking = true;
                attackTimer = 0;
                meleeCooldownTimer = 0;
                telegraphVFX2.transform.localScale = Vector3.one;
                telegraphVFX2.SetActive(true);
            }
            else if (distanceToPlayer <= rangedAttackDist)
            {
                if (beamCooldownTimer >= beamCooldown)
                {
                    StartBeamAttack();
                }
                else
                {

                    projectileAttacking = true;
                }
            }
        }

        if (meleeAttacking)
        {
            StartMeeleAttack();
        }

        if (projectileAttacking)
        {
            ProjectileMovementSpeed();
            //Stop();
            if (projectileTimer < projectileDuration)
            {
                attackTimer = 0;
                projectileTimer += Time.deltaTime;
                ShootProjectile();
            } 
            else
            {
                projectileAttacking = false;
                projectileTimer = 0;
                shootPoint.transform.rotation = gameObject.transform.root.rotation;
            }

        }

        if(beamAttacking)
        {
            BeamMovementSpeed();
            //Stop();
            if (beamTimer < beamDuration)
            {
                attackTimer = 0;
                beamTimer += Time.deltaTime;
                BeamAttack();
            }
            else
            {
                StopAttacking();
                beamAttacking = false;
                beamTimer = 0;
                shootPoint.transform.rotation = gameObject.transform.root.rotation;
            }
        }
    }

    private void IdleBehaviour()
    {
        float playerDist = Vector3.Distance(transform.position, player.transform.position);
        if (playerDist < noticePlayerRange)
        {
            Vector3 directionToPlayer = player.transform.position - shootPoint.transform.position;

            RaycastHit hit;
            if (Physics.Raycast(shootPoint.transform.position, directionToPlayer, out hit))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    previousPlayerPosition = hit.transform.position;
                    foundPlayer = true;
                }
            }
        }
    }

    private void NavMeshMove()
    {
        agent.isStopped = false;
        agent.SetDestination(player.transform.position);
        agent.speed = 11;
        agent.angularSpeed = angularSpeed;
    }

    public void Stop()
    {
        agent.speed = 2;
        agent.angularSpeed = angularSpeed;
    }
    
    IEnumerator fade()
    {
        yield return new WaitForSeconds(.1f);
        lineRenderer.enabled = true;
        attackSFX.Play();
    }

    private void StopAttacking()
    {
        lineRenderer.enabled = false;
        attackSFX.Stop();
        turnRadius = 6;
        maxDelta = 6;
    }

    #region Melee

    private void StartMeeleAttack()
    {
        if (telegraphVFX2.transform.localScale.y < 10)
        {
            telegraphVFX2.transform.localScale += Vector3.one * Time.deltaTime * telegraphScaleSpeed;
        }
        else
        {
            DoMeleeAttack();
        }
    }

    private void DoMeleeAttack()
    {
        MeleeDealDamage();
        Instantiate(attackVFX, expandingAttackPosition.transform.position, expandingAttackPosition.transform.rotation, transform);
        meleeAttacking = false;
        telegraphVFX2.SetActive(false);
    }

    private void MeleeDealDamage()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, shootPoint.transform.position);

        if(distanceToPlayer >= 25) { return; }

        Vector3 playerDirection = player.transform.position - shootPoint.transform.position;


        RaycastHit hit;
        if (Physics.Raycast(shootPoint.transform.position, playerDirection, out hit, distanceToPlayer + 1, meleeLayerMask))
        {

            print(hit.transform.name);

            if (hit.transform.CompareTag("Player"))
            {
                aMainSystem.DealDamage(hit.transform.gameObject, meleeDamage, false);
                //hit.collider.gameObject.GetComponent<IDamageInterface>().Damage(meleeDamage);
            }
        }
    }

    #endregion

    #region Beam
    private void StartBeamAttack()
    {
        beamCooldownTimer = 0;
        beamAttacking = true;
        StartCoroutine(fade());
        turnRadius = beamFollowSpeed;
        maxDelta = maxBeamFollowSpeed;
    }

    private void BeamAttack()
    {
        RotateTowardsPlayer();

        Vector3 playerDirection = player.transform.position - shootPoint.transform.position;
        Vector3 newDirection = Vector3.RotateTowards(shootPoint.transform.forward, playerDirection, beamFollowSpeed * Time.deltaTime, maxBeamFollowSpeed);

        shootPoint.transform.rotation = Quaternion.LookRotation(newDirection);

        Vector3 hitPosition = shootPoint.transform.position + shootPoint.transform.forward * 200;

        RaycastHit hit;
        if (Physics.SphereCast(shootPoint.transform.position, 1.7f, shootPoint.transform.forward, out hit, 999, layerMask))

        if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out hit))
        {
            hitPosition = hit.point;
            if (hit.transform.CompareTag("Player"))
            {
                    aMainSystem.DealDamage(hit.transform.gameObject, meleeDamage, false);
                    //hit.collider.gameObject.GetComponent<IDamageInterface>().Damage(beamDamage);
            }

        }
        else
            hitPosition = shootPoint.transform.position + shootPoint.transform.forward * 200;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, shootPoint.transform.position);
        lineRenderer.SetPosition(1, hitPosition);
    }

    private void BeamMovementSpeed()
    {
        agent.speed = 2;
        agent.angularSpeed = 400;
    }

    #endregion

    #region Projectiles

    private void ShootProjectile()
    {
        RotateTowardsPlayer();

        if (projectileFireRateTimer >= projectileFireRate)
        {
            ShootFan();
            projectileFireRateTimer = 0;
        }
        else
            projectileFireRateTimer += Time.deltaTime;
    }

    public void ShootFan()
    {
        Vector3 directionToPlayer = (player.transform.position - shootPoint.transform.position).normalized;
        Quaternion baseRotation = Quaternion.LookRotation(directionToPlayer) * Quaternion.Euler(yOffset, 0, 0); // Apply Y offset

        float startAngle = -(projectileAmount - 1) / 2f * projectileSpacing;

        for (int i = 0; i < projectileAmount; i++)
        {
            float angle = startAngle + (i * projectileSpacing);
            Quaternion rotation = Quaternion.Euler(0, angle, 0) * baseRotation; // Apply horizontal spread
            Instantiate(projectile, shootPoint.transform.position, rotation);
        }
    }

    #endregion

    private void ProjectileMovementSpeed()
    {
        agent.speed = 7;
        agent.angularSpeed = angularSpeed;
    }

    private void RotateTowardsPlayer()
    {
        

        Vector3 newDirection1 = Vector3.RotateTowards(transform.forward, player.transform.position, 1 * Time.deltaTime, 0);
        transform.rotation = Quaternion.LookRotation(newDirection1);
    }
}