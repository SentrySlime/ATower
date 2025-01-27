using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArealEnemyMovement : MonoBehaviour
{


    [Header("Movement")]
    public NavMeshAgent agent;
    public float movementSpeed = 0.1f;
    float minDistance = 1;
    GameObject player;
    Vector3 previousPlayerPosition;
    float turnRadius = 3;
    float maxDelta = 3;

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

    [Header("Notice player")]
    public bool foundPlayer = false;
    public float noticePlayerRange = 75;

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
        previousPlayerPosition = transform.position;
    }

    void Update()
    {
        if(!foundPlayer)
            IdleBehaviour();

        

        //timer += Time.deltaTime;
        //if (timer < fireRate && !beamAttacking && !meleeAttacking)
        //    timer += Time.deltaTime;

        if (beamCooldownTimer < beamCooldown && !beamAttacking)
            beamCooldownTimer += Time.deltaTime;

        if(projectileCooldownTimer < projectileCooldown && !projectileAttacking)
            projectileCooldownTimer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(player.transform.position, shootPoint.transform.position);

        if (distanceToPlayer > rangedAttackDist && !beamAttacking && !meleeAttacking)
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
            if(distanceToPlayer <= meleeRange)
            {
                meleeAttacking = true;
                MeeleAttack();
                attackTimer = 0;
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
            if (beamTimer < beamDuration)
            {
                attackTimer = 0;
                beamTimer += Time.deltaTime;
                BeamAttack();
                //Shootshotgun();
            }
            else
            {
                StopAttacking();
                beamAttacking = false;
                beamTimer = 0;
                shootPoint.transform.rotation = gameObject.transform.root.rotation;
                //shootPoint.transform.rotation = partToRotate.transform.rotation;
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
        agent.SetDestination(player.transform.position);
    }

    private void MoveToPlayer()
    {
        Vector3 directionToPlayer = player.transform.position - shootPoint.transform.position;

        RaycastHit hit;
        if (Physics.Raycast(shootPoint.transform.position, directionToPlayer, out hit))
        {
            if (hit.transform.CompareTag("Player"))
            {
                Avoidance();
                TurnToPosition(player.transform.position);
                previousPlayerPosition = hit.transform.position;
                transform.position += directionToPlayer * movementSpeed * Time.deltaTime;
            }
            //else
            //{
            //    TurnToPosition(previousPlayerPosition);
            //    Vector3 directionToPreviousPosition = previousPlayerPosition - shootPoint.transform.position;
            //    transform.position += directionToPreviousPosition * movementSpeed * Time.deltaTime;
            //}
        }
    }

    private void Avoidance()
    {
        Vector3 rightDirection = Quaternion.Euler(0, 45, 0) * shootPoint.transform.forward;
        Vector3 leftDirection = Quaternion.Euler(0, -45, 0) * shootPoint.transform.forward;
        Vector3 upDirection = Quaternion.Euler(45, 0, 0) * shootPoint.transform.forward;
        Vector3 downDirection = Quaternion.Euler(-45, 0, 0) * shootPoint.transform.forward;
        RaycastHit hit;


        #region Right
        if (Physics.Raycast(shootPoint.transform.position, rightDirection, out hit, avoidanceDistance))
        {
            Debug.DrawLine(shootPoint.transform.position, hit.point, Color.red);
            if (hit.transform.CompareTag("Player"))
            {

            }
        }
        else
        {
            
            Debug.DrawLine(shootPoint.transform.position, shootPoint.transform.position + rightDirection * avoidanceDistance, Color.red);
        }
        #endregion

        #region Left
        if (Physics.Raycast(shootPoint.transform.position, leftDirection, out hit, avoidanceDistance))
        {
            Debug.DrawLine(shootPoint.transform.position, hit.point, Color.red);
            if (hit.transform.CompareTag("Player"))
            {

            }
        }
        else
        {

            Debug.DrawLine(shootPoint.transform.position, shootPoint.transform.position + leftDirection * avoidanceDistance, Color.red);
        }
        #endregion

        #region Downwards
        if (Physics.Raycast(shootPoint.transform.position, downDirection, out hit, avoidanceDistance))
        {
            Debug.DrawLine(shootPoint.transform.position, hit.point, Color.red);
            if (hit.transform.CompareTag("Player"))
            {

            }
        }
        else
        {
            Debug.DrawLine(shootPoint.transform.position, shootPoint.transform.position + downDirection * avoidanceDistance, Color.red);
        }
        #endregion

        #region Upwards
        if (Physics.Raycast(shootPoint.transform.position, upDirection, out hit, avoidanceDistance))
        {
            Debug.DrawLine(shootPoint.transform.position, hit.point, Color.red);
            if (hit.transform.CompareTag("Player"))
            {

            }
        }
        else
        {
            Debug.DrawLine(shootPoint.transform.position, shootPoint.transform.position + upDirection * avoidanceDistance, Color.red);
        }
        #endregion

        #region Forward
        if (Physics.Raycast(shootPoint.transform.position, transform.forward, out hit, avoidanceDistance))
        {
            Debug.DrawLine(shootPoint.transform.position, hit.point, Color.red);
            if (hit.transform.CompareTag("Player"))
            {
                
            }
        }
        else
        {
            Debug.DrawLine(shootPoint.transform.position, shootPoint.transform.position + shootPoint.transform.forward * avoidanceDistance, Color.red);
        }
        #endregion

    }


    private void TurnToPosition(Vector3 position)
    {
        Vector3 playerDirection = position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, playerDirection, turnRadius * Time.deltaTime, maxDelta);
        newDirection = new Vector3(newDirection.x, 0, newDirection.z);
        transform.rotation = Quaternion.LookRotation(newDirection);
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
        turnRadius = 3;
        maxDelta = 3;
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
        //GameObject tempObj = Instantiate(attackVFX, transform.position, Quaternion.identity, gameObject.transform);
        //tempObj.transform.localScale = new Vector3(3, 3, 3);
    }

    private void BeamAttack()
    {

        Vector3 playerDirection = player.transform.position - shootPoint.transform.position;
        Vector3 newDirection = Vector3.RotateTowards(shootPoint.transform.forward, playerDirection, beamFollowSpeed * Time.deltaTime, maxBeamFollowSpeed);
        //newDirection = new Vector3(newDirection.x, newDirection.y, newDirection.z);
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

        //print(hitPosition);
        lineRenderer.SetPosition(0, shootPoint.transform.position);
        lineRenderer.SetPosition(1, hitPosition);
    }


    private void Shootshotgun()
    {
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
                //transform.position += directionToPlayer * movementSpeed * Time.deltaTime;

                for (int i = 0; i < projectileAmount; i++)
                {
                    #region RandomNumbers Accuracy


                    float minYOffset = Random.Range(-Accuracy, 0);
                    float maxYOffset = Random.Range(Accuracy, 0);

                    float minXoffset = Random.Range(-Accuracy, 0);
                    float maxXoffset = Random.Range(Accuracy, 0);

                    #endregion

                    //shootPoint.transform.LookAt(player.transform.position);
                    //shootPoint.transform.Rotate(((minXoffset + maxXoffset / 1.5f) + 0.2f), ((minYOffset + maxYOffset / 1.5f) + 0.2f), 0);

                    //Debug.DrawLine(player.transform.position, shootPoint.transform.forward * 999, Color.red, .2f);



                    Instantiate(projectile, shootPoint.transform.position, shootPoint.transform.rotation);



                    //Rigidbody rb = Instantiate(projectile, shootPoint.transform.position, shootPoint.transform.rotation).GetComponent<Rigidbody>();

                    //rb.AddForce(shootPoint.transform.forward * projectileForce, ForceMode.Impulse);



                    //------------------

                    //Vector3 dir = player.transform.position - shootPoint.transform.position;



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
                    shootPoint.transform.rotation = gameObject.transform.root.rotation;
                }
            }
        }
    }
}