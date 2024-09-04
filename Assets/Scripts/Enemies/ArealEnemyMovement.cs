using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArealEnemyMovement : MonoBehaviour
{

    [Header("Movement")]
    public float minDistance = 20;
    public float movementSpeed = 0.5f;
    public GameObject player;

    public float turnRadius = 1;
    public float maxDelta = 1;

    [Header("Attack Stats")]
    public float timer;
    public float attackTimer = 0;
    public AudioSource attackSFX;
    public float rangedAttackDist = 10;

    [Header("Projectile Attack")]
    public bool beamAttacking = false;
    public float attackDuration = 1;

    public float attackRate;
    public float projectileAmount;
    public float projectileForce;
    public float Accuracy = 0;
    public GameObject shootPoint;
    public GameObject projectile;

    [Header("Beam Attack")]
    public float beamDamage = 1;
    public float beamFollowSpeed = 1;
    public float maxBeamFollowSpeed = 1;
    public float beamCooldown = 10;
    float beamCooldownTimer = 0;
    LineRenderer lineRenderer;

    

    public GameObject partToRotate;

    [Header("Melee Attack")]
    public GameObject telegraphVFX;
    public GameObject attackVFX;
    public float meleeRange = 1;
    public float meleeRadius = 1;
    public int meleeDamage = 10;
    public bool meleeAttacking = false;

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
    }

    void Update()
    {

        Vector3 playerDirection = player.transform.position - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, playerDirection, turnRadius * Time.deltaTime, maxDelta);
        newDirection = new Vector3(newDirection.x, 0, newDirection.z);
        transform.rotation = Quaternion.LookRotation(newDirection);

        //timer += Time.deltaTime;
        //if (timer < fireRate && !beamAttacking && !meleeAttacking)
        //    timer += Time.deltaTime;

        if (beamCooldownTimer < beamCooldown)
            beamCooldownTimer += Time.deltaTime;

        float distanceToPlayer = Vector3.Distance(player.transform.position, shootPoint.transform.position);


        if (distanceToPlayer > rangedAttackDist && !beamAttacking && !meleeAttacking)
        {
            MoveToPlayer();
        }
        else if (timer < attackRate && !beamAttacking && !meleeAttacking)
        {
            timer += Time.deltaTime;
        }
        else if (timer >= attackRate && !beamAttacking)
        {
            if(distanceToPlayer <= meleeRange)
            {
                meleeAttacking = true;
                MeeleAttack();
                timer = 0;
            }
            else if (distanceToPlayer <= rangedAttackDist)
            {
                
                

                if(beamCooldownTimer >= beamCooldown)
                {
                    beamCooldownTimer = 0;
                    beamAttacking = true;
                    StartAttacking();
                }
                else
                {
                    Shootshotgun();
                    timer = 0;
                }


            }
        }
        else if(beamAttacking)
        {
            
            if (attackTimer < attackDuration)
            {
                attackTimer += Time.deltaTime;
                BeamAttack();
                //Shootshotgun();
            }
            else
            {
                StopAttacking();
                beamAttacking = false;
                attackTimer = 0;
                timer = 0;
                shootPoint.transform.rotation = gameObject.transform.root.rotation;
                //shootPoint.transform.rotation = partToRotate.transform.rotation;
            }


        }

    }

    private void MoveToPlayer()
    {
        
        Vector3 directionToPlayer = player.transform.position - shootPoint.transform.position;

        RaycastHit hit;
        if (Physics.Raycast(shootPoint.transform.position, directionToPlayer, out hit))
        {
            if (hit.transform.CompareTag("Player"))
            {
                transform.position += directionToPlayer * movementSpeed * Time.deltaTime;
            }
        }
    }

    private void StartAttacking()
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