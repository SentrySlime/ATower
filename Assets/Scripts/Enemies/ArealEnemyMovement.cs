using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;

public class ArealEnemyMovement : MonoBehaviour, INoticePlayer
{


    [Header("Movement")]
    public NavMeshAgent agent;
    
    public float movementSpeed = 1f;
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
    public bool projectileAttacking = false;
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
    
    float Accuracy = 5;

    [Header("Beam Attack")] //Beams
    public float beamDamage = 10;
    public float beamCooldown = 10;
    public float beamCooldownTimer = 0;


    public float beamDamageRate = 0.5f;
    float beamDamageRateTimer = 0f;

    public float rotationSpeed = 1;

    public GameObject partToRotate;
    public GameObject telegraphVFX;
    public GameObject telegraphPosition;
    

    float beamFollowSpeed = 0.125f;
    float maxBeamFollowSpeed = 0.125f;
    public float yawSpeed = 0.15f;
    public float pitchSpeed = 0.15f;
    LineRenderer lineRenderer;

    bool beamAttacking = false;
    bool beamIsTelegraphing = false;

    [Header("Melee Attack")] //Melee
    public bool meleeAttacking = false;
    public float meleeCooldown = 4;
    float meleeCooldownTimer = 0;
    
    float telegraphScaleSpeed = 6;
    float meleeRange = 28;

    float meleeForce = 300;
    float meleeForceY = 30;

    public int meleeDamage = 30;
    float meleeRadius = 1;
    public GameObject attackVFX;
    public GameObject telegraphVFX2;
    public GameObject expandingAttackPosition;

    public LayerMask meleeLayerMask;

    bool performingBeamAttack = false;

    [Header("Notice player")]
    public float noticePlayerRange = 75;
    public LayerMask layerMask;
    public bool foundPlayer = false;
    public bool canSeePlayer = false;

    [Header("Avoidance")]
    Vector3 frontPosition;
    public float avoidanceDistance = 15;

    [Header("Misc")]
    public float distanceToPlayer;
    public float burstTimer;
    int burstCount = 0;
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

        if (beamCooldownTimer < beamCooldown && !beamAttacking)
            beamCooldownTimer += Time.deltaTime;

        if(projectileCooldownTimer < projectileCooldown && !projectileAttacking)
            projectileCooldownTimer += Time.deltaTime;

        if (meleeCooldownTimer < meleeCooldown && !meleeAttacking)
            meleeCooldownTimer += Time.deltaTime;

        if (!foundPlayer)
        {
            IdleBehaviour();
            return;
        }


        distanceToPlayer = Vector3.Distance(player.transform.position, shootPoint.transform.position);
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

        if (foundPlayer)
        {
            SetMoveMentSpeed();
            NavMeshMove();
        }

        if (!canSeePlayer && !beamAttacking && !projectileAttacking && !meleeAttacking)
        {
            return;
        }

        if (attackTimer < attackRate && !beamAttacking && !meleeAttacking)
        {
            attackTimer += Time.deltaTime;
        }
        else if (attackTimer >= attackRate && !performingBeamAttack)
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
            //ProjectileMovementSpeed();
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
            //BeamMovementSpeed();
            //Stop();

            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 toOther = Vector3.Normalize(player.transform.position - transform.position);

            if(Vector3.Dot(forward, toOther) < 0)
            {
                beamTimer = beamDuration;
            }

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
                performingBeamAttack = false;
                beamTimer = 0;
                shootPoint.transform.rotation = gameObject.transform.root.rotation;
            }
        }
    }

    private void IdleBehaviour()
    {

        if (distanceToPlayer < noticePlayerRange)
        {
            Vector3 directionToPlayer = player.transform.position - shootPoint.transform.position;

            RaycastHit hit;
            if (Physics.Raycast(shootPoint.transform.position, directionToPlayer, out hit, noticePlayerRange, layerMask))
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
        agent.angularSpeed = angularSpeed;

        agent.speed = movementSpeed;
    }

    public void Stop()
    {
        agent.speed = 0;
        agent.angularSpeed = angularSpeed;
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

        if(distanceToPlayer >= 25) { return; }

        Vector3 playerDirection = player.transform.position - shootPoint.transform.position;

        RaycastHit hit;
        if (Physics.Raycast(shootPoint.transform.position, playerDirection, out hit, distanceToPlayer + 1, meleeLayerMask))
        {

            if (hit.transform.CompareTag("Player"))
            {
                aMainSystem.DealDamage(hit.transform.gameObject, meleeDamage, false);

                //Push the player
                hit.transform.GetComponent<Locomotion2>().Push();
                playerDirection = playerDirection.normalized; 
                Vector3 force = playerDirection * meleeForce; 
                force.y = meleeForceY; 
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            }
        }
    }

    #endregion

    #region Beam
    private void StartBeamAttack()
    {
        performingBeamAttack = true;

        turnRadius = beamFollowSpeed;
        maxDelta = maxBeamFollowSpeed;

        if (beamIsTelegraphing) { return; }

        StartCoroutine(BeamTelegraph());
    }

    private void BeamAttack()
    {
        RotateTowardsPlayer();

        if (beamDamageRateTimer < beamDamageRate)
        {
            beamDamageRateTimer += Time.deltaTime;
        }

        //Old rotate code
        Vector3 playerDirection = player.transform.position - shootPoint.transform.position;
        Vector3 horizontalDirection = new Vector3(playerDirection.x, 0, playerDirection.z);
        Vector3 verticalDirection = new Vector3(playerDirection.x, 0, playerDirection.z);

        Vector3 newDirection = Vector3.RotateTowards(shootPoint.transform.forward, playerDirection, yawSpeed * Time.deltaTime, maxBeamFollowSpeed);
        Vector3 finalDir = Vector3.RotateTowards(shootPoint.transform.forward, playerDirection, pitchSpeed * Time.deltaTime, maxBeamFollowSpeed);

        Vector3 finalFinalDir = new Vector3(newDirection.x, finalDir.y, newDirection.z);

        shootPoint.transform.rotation = Quaternion.LookRotation(finalFinalDir);

        Vector3 hitPosition = shootPoint.transform.position + shootPoint.transform.forward * 200;


        RaycastHit hit;

        if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out hit))
        {
            hitPosition = hit.point;

            if (hit.transform.CompareTag("Player"))
            {
                if(beamDamageRateTimer >= beamDamageRate)
                {
                    aMainSystem.DealDamage(hit.transform.gameObject, beamDamage, false);
                    beamDamageRateTimer = 0;
                }
                    //hit.collider.gameObject.GetComponent<IDamageInterface>().Damage(beamDamage);
            }

        }
        else
            hitPosition = shootPoint.transform.position + shootPoint.transform.forward * 200;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, shootPoint.transform.position);
        lineRenderer.SetPosition(1, hitPosition);
    }

    IEnumerator BeamTelegraph()
    {

        beamIsTelegraphing = true;

        Instantiate(telegraphVFX, telegraphPosition.transform.position, telegraphPosition.transform.rotation, telegraphPosition.transform);
        yield return new WaitForSeconds(1);
        beamAttacking = true;
        StartCoroutine(fade());
        beamIsTelegraphing = false;
    }

    IEnumerator fade()
    {
        yield return new WaitForSeconds(.1f);
        lineRenderer.enabled = true;
        attackSFX.Play();
        beamCooldownTimer = 0;
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
        agent.angularSpeed = angularSpeed;
    }

    private void RotateTowardsPlayer()
    {
        if (!canSeePlayer) { return; }

        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void SetMoveMentSpeed()
    {
        if (player == null) return;

        if(meleeAttacking)
        {
            movementSpeed = 0;
            return;
        }
        else if (beamAttacking)
        {
            movementSpeed = 0;
            return;
        }
        else if(projectileAttacking)
        {
            movementSpeed = 0;
            return;
        }
        else
        {
            // Map the distance to speed while clamping
            movementSpeed = Mathf.Clamp((distanceToPlayer / 30) * 11, 1, 11);
        }
    }

    public void NoticePlayer()
    {
        foundPlayer = true;
    }
}