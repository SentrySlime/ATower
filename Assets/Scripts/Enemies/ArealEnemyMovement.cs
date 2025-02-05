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
    public float attackRate = 4;
    public float attackTimer = 0;

    public float beamDuration = 4;
    public float beamTimer = 0;

    public float projectileDuration = 2;
    public float projectileTimer = 0;

    public AudioSource attackSFX;
    public float rangedAttackDist = 10;

    [Header("Projectile Attack")]
    public bool projectileAttacking = false;
    public float projectileCooldown = 3;
    public float projectileCooldownTimer = 0;

    public float projectileFireRate = 0.25f;
    public float projectileFireRateTimer = 0;

    float projectileAmount = 1;
    float projectileForce = 35;
    float Accuracy = 5;
    public GameObject shootPoint;
    public GameObject projectile;

    [Header("Beam Attack")]
    public bool beamAttacking = false;
    public float beamCooldown = 10;
    public float beamCooldownTimer = 0;
    public GameObject partToRotate;

    float beamDamage = 10;
    float beamFollowSpeed = 0.26f;
    float maxBeamFollowSpeed = 0.26f;
    LineRenderer lineRenderer;

    [Header("Melee Attack")]
    public GameObject telegraphVFX;
    public GameObject attackVFX;
    public float meleeRange = 1;
    public float meleeRadius = 1;
    public int meleeDamage = 10;
    public bool meleeAttacking = false;

    public float meleeCooldown = 4;
    public float meleeCooldownTimer = 0;

    [Header("Notice player")]
    public bool foundPlayer = false;
    public bool canSeePlayer = false;
    public float noticePlayerRange = 75;
    public LayerMask layerMask;

    [Header("Avoidance")]
    Vector3 frontPosition;
    public float avoidanceDistance = 15;

    [Header("Misc")]
    int burstCount = 0;
    public float burstTimer;
    bool chasingPlayer = false;
    bool burst = false;
    AMainSystem aMainSystem;

    void Start()
    {
        player = GameObject.Find("PlayerTargetPoint");
        aMainSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        previousPlayerPosition = transform.position;
        layerMask = ~layerMask;
    }

    void Update()
    {
        if(!foundPlayer)
            IdleBehaviour();


        //print(agent.isOnOffMeshLink);

        //timer += Time.deltaTime;
        //if (timer < fireRate && !beamAttacking && !meleeAttacking)
        //    timer += Time.deltaTime;

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
            //MoveToPlayer();
        }
        else if (attackTimer < attackRate && !beamAttacking && !meleeAttacking)
        {
            attackTimer += Time.deltaTime;
        }
        else if (attackTimer >= attackRate && !beamAttacking)
        {
            //Stop();
            if(distanceToPlayer <= meleeRange && meleeCooldownTimer >= meleeCooldown)
            {
                meleeAttacking = true;
                MeeleAttack();
                attackTimer = 0;
                meleeCooldownTimer = 0;
            }
            else if (distanceToPlayer <= rangedAttackDist)
            {
                if(beamCooldownTimer >= beamCooldown)
                {
                    beamCooldownTimer = 0;
                    beamAttacking = true;
                    StartBeamAttack();
                }
                else
                {
                    
                    projectileAttacking = true;
                    //Shootshotgun();
                    //attackTimer = 0;
                }
            }
        }


        if(projectileAttacking)
        {
            ProjectileMovementSpeed();
            //Stop();
            if (projectileTimer < projectileDuration)
            {
                attackTimer = 0;
                projectileTimer += Time.deltaTime;
                Shootshotgun();
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
                lineRenderer.enabled = false;
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

    private void BeamMovementSpeed ()
    {
        agent.speed = 2;
        agent.angularSpeed = 400;
    }


    private void ProjectileMovementSpeed()
    {
        agent.speed = 7;
        agent.angularSpeed = angularSpeed;
    }


    public void Stop()
    {
        //agent.isStopped = true;
        //agent.velocity = Vector3.zero;
        //agent.ResetPath();
        //print("Stopped");
        agent.speed = 2;
        agent.angularSpeed = angularSpeed;
    }
    
    private void StartBeamAttack()
    {
        StartCoroutine(fade());
        turnRadius = beamFollowSpeed;
        maxDelta = maxBeamFollowSpeed;
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

    private void MeeleAttack()
    {
        GameObject tempObj = Instantiate(telegraphVFX, transform.position, Quaternion.identity, gameObject.transform);
        tempObj.transform.localScale = new Vector3(3, 3, 3);

        StartCoroutine(StopTelegraph(tempObj));
    }

    IEnumerator StopTelegraph(GameObject ps)
    {

        ParticleSystem[] temp = ps.GetComponentsInChildren<ParticleSystem>();
        
        yield return new WaitForSeconds(1);


        meleeAttacking = false;
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].Stop();
        }

        yield return new WaitForSeconds(1);

        aMainSystem.SpawnExplosion(transform.position, meleeRadius, meleeDamage, gameObject, true);
    }

    private void BeamAttack()
    {

        Vector3 playerDirection = player.transform.position - shootPoint.transform.position;
        Vector3 newDirection = Vector3.RotateTowards(shootPoint.transform.forward, playerDirection, beamFollowSpeed * Time.deltaTime, maxBeamFollowSpeed);

        shootPoint.transform.rotation = Quaternion.LookRotation(newDirection);

        Vector3 hitPosition = shootPoint.transform.position + shootPoint.transform.forward * 200;

        RaycastHit hit;
        if (Physics.SphereCast(shootPoint.transform.position, 1.7f, shootPoint.transform.forward, out hit))

        if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out hit))
        {
            hitPosition = hit.point;
            if (hit.transform.CompareTag("Player"))
            {
                    hit.collider.gameObject.GetComponent<IDamageInterface>().Damage(beamDamage);
            }

        }
        else
            hitPosition = shootPoint.transform.position + shootPoint.transform.forward * 200;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, shootPoint.transform.position);
        lineRenderer.SetPosition(1, hitPosition);
    }


    private void Shootshotgun()
    {
        Vector3 playerDirection = player.transform.position - shootPoint.transform.position;
        Vector3 newDirection = Vector3.RotateTowards(shootPoint.transform.forward, playerDirection, beamFollowSpeed * Time.deltaTime, maxBeamFollowSpeed);

        if (projectileFireRateTimer >= projectileFireRate)
        {
            FireShotgun();
            projectileFireRateTimer = 0;
        }
        else
            projectileFireRateTimer += Time.deltaTime;
    }

    private void FireShotgun()
    {
        Vector3 directionToPlayer = player.transform.position - shootPoint.transform.position;

        RaycastHit hit;
        if (Physics.Raycast(shootPoint.transform.position, directionToPlayer, out hit))
        {
            if (hit.transform.CompareTag("Player"))
            {

                for (int i = 0; i < projectileAmount; i++)
                {
                    Vector3 dir = player.transform.position - shootPoint.transform.position;
                    Quaternion rotation = Quaternion.LookRotation(dir); // Create a rotation that looks in the direction of dir
                    Instantiate(projectile, shootPoint.transform.position, rotation);

                    #region RandomNumbers Accuracy


                    //float minYOffset = Random.Range(-Accuracy, 0);
                    //float maxYOffset = Random.Range(Accuracy, 0);

                    //float minXoffset = Random.Range(-Accuracy, 0);
                    //float maxXoffset = Random.Range(Accuracy, 0);


                    //shootPoint.transform.LookAt(player.transform.position);
                    //shootPoint.transform.Rotate(((minXoffset + maxXoffset / 1.5f) + 0.2f), ((minYOffset + maxYOffset / 1.5f) + 0.2f), 0);

                    //Debug.DrawLine(player.transform.position, shootPoint.transform.forward * 999, Color.red, .2f);




                    //Rigidbody rb = Instantiate(projectile, shootPoint.transform.position, shootPoint.transform.rotation).GetComponent<Rigidbody>();

                    //rb.AddForce(shootPoint.transform.forward * projectileForce, ForceMode.Impulse);



                    //------------------




                    //------------

                    //RaycastHit hit;
                    //if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out hit, weaponRange, layermask2))
                    //{
                    //    Debug.LogError("Hit");
                    //    DealDamage(hit.transform.gameObject);
                    //    hitDistance = Vector3.Distance(shootPoint.transform.position, hit.point);
                    //    Instantiate(hitVFX, hit.point, Quaternion.Inverse(transform.rotation));
                    //}

                    //shootPoint.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    #endregion

                    shootPoint.transform.rotation = gameObject.transform.root.rotation;
                }
            }
        }
    }
}