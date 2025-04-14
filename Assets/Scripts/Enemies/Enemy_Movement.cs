using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Movement : MonoBehaviour
{
    public NavMeshAgent agent;
    GameObject player;
    Vector3 lastValidLocation;

    bool foundPlayer;
    float timer;

    public Transform shootPoint;
    public LayerMask layerMask;
    public float interactDistance_ = 125f;
    public float firingRate = 2;

    [Header("Ranged")]
    public bool rangedAttack;
    public GameObject rangedProjectile;
    public float rangedDistance_ = 75;

    [Header("Melee")]
    public bool meleeAttack;
    public GameObject meleeObj;
    public float meleeDistance_ = 10;

    [Header("Movement")]
    public bool roam = false;
    private float newDestinationRate = 0.5f;
    private float newDestinationTimer = 0;

    [Header("Movement perlin")]
    public float noiseScale = 1.0f;
    public float pathOffsetAmount = 5.0f;
    public float updateRate = 1f;

    private float movementTimer = 0f;
    public float noiseOffset;

    void Start()
    {

        //firingRate = Random.Range(2, 4);
        noiseOffset = Random.Range(0f, 100f);
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
        MeleeAttack();
    }

    void Update()
    {        
        if(roam)
        {
            Roam();
            return;
        }

        movementTimer += Time.deltaTime;
        if (movementTimer >= updateRate)
        {
            if (agent && agent.isOnNavMesh)
            {
                EnemyMove();
                movementTimer = 0f;
            }
        }

        NoticePlayer();

        if (!foundPlayer) { return; }

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);

        //if(agent && agent.isOnNavMesh)
        //EnemyMove();

        #region attacks

        //if (timer < firingRate)
        //{
        //    timer += Time.deltaTime;
        //}
        //else if (playerDistance <= meleeDistance_ && meleeAttack)
        //{
        //    Vector3 playerDirection = player.transform.position - transform.position;

        //    RaycastHit hit;
        //    if (Physics.Raycast(transform.position, playerDirection, out hit))
        //    {
        //        if (hit.transform.CompareTag("Player"))
        //        {
        //            timer = 0;
        //            MeleeAttack();
        //        }
        //    }

        //}
        //else if (playerDistance <= rangedDistance_ && rangedAttack)
        //{

        //            RangedAttack();
        //            timer = 0;
        //    Vector3 playerDirection = player.transform.position - transform.position;

        //    RaycastHit hit;
        //    if (Physics.Raycast(transform.position, playerDirection, out hit))
        //    {


        //        if (hit.transform.CompareTag("Player"))
        //        {

        //        }
        //    }
        //}

        //agent.Move(player.transform.position);

        #endregion
    }

    //public void EnemyMove()
    //{
    //    NavMeshPath navMeshPath = new NavMeshPath();

    //    if (agent.CalculatePath(player.transform.position, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
    //    {
    //        lastValidLocation = player.transform.position;
    //        agent.isStopped = false;
    //        agent.SetDestination(lastValidLocation);

    //    }

    //    if (agent.CalculatePath(lastValidLocation, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
    //    {

    //        if (agent.remainingDistance <= 6 && agent.remainingDistance != 0)
    //        {
    //            //agent.ResetPath();
    //            //agent.isStopped = true;

    //        }
    //        else
    //        {
    //            agent.isStopped = false;
    //            agent.SetDestination(lastValidLocation);
    //        }


    //    }
    //    else
    //    {

    //        agent.ResetPath();
    //        agent.isStopped = true;

    //    }
    //}

    public void EnemyMove()
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        // Direction from enemy to player
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

        // Get Perlin-based offset
        float time = Time.time + noiseOffset;
        float noiseX = Mathf.PerlinNoise(time * noiseScale, 0f);
        float noiseZ = Mathf.PerlinNoise(0f, time * noiseScale);

        Vector3 noiseOffsetVec = new Vector3((noiseX - 0.5f) * 2f, 0, (noiseZ - 0.5f) * 2f) * pathOffsetAmount;

        // Combine direct path with noise offset
        Vector3 targetPosition = player.transform.position + noiseOffsetVec;

        // Validate path
        if (agent.CalculatePath(targetPosition, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
        {
            lastValidLocation = targetPosition;
            agent.isStopped = false;
            agent.SetDestination(lastValidLocation);
        }
        else
        {
            agent.ResetPath();
            agent.isStopped = true;
        }
    }

    private void MeleeAttack()
    {
        float length = Vector3.Distance(player.transform.position, transform.position);
        if (length < meleeDistance_)
        {

            Vector3 direction = player.transform.position - shootPoint.position;

            Instantiate(meleeObj, shootPoint.position + transform.forward * 2, shootPoint.transform.rotation);


            //rb.AddForce(direction.normalized * projectileSpeed, ForceMode.Impulse);

        }
    }

    private void RangedAttack()
    {
        float length = Vector3.Distance(player.transform.position, transform.position);
        if (length < rangedDistance_)
        {

            Vector3 direction = player.transform.position - shootPoint.position;

            Instantiate(rangedProjectile, shootPoint.position, shootPoint.transform.rotation);

            //rb.AddForce(direction.normalized * projectileSpeed, ForceMode.Impulse);

        }
    }

    public virtual void OnButtonPress()
    {

    }

    private void NoticePlayer()
    {
        float length = Vector3.Distance(player.transform.position, transform.position);
        if (length < interactDistance_)
        {
            Vector3 playerDir = player.transform.position - shootPoint.transform.position;

            RaycastHit hit;
            if (Physics.Raycast(shootPoint.transform.position, playerDir, out hit, 400/*, ~layerMask*/))
            {

                Debug.DrawLine(shootPoint.transform.position, playerDir * 200);
                if (hit.transform.CompareTag("Player"))
                {
                    foundPlayer = true;

                }
            }

        }


    }

    public void Roam()
    {
        if(newDestinationTimer < newDestinationRate)
        {
            newDestinationTimer += Time.deltaTime;
        }
        else
        {
            Vector3 randomDirection = Random.insideUnitSphere * 10;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10, 1);
            Vector3 finalPosition = hit.position;
            newDestinationTimer = 0;
            agent.SetDestination(finalPosition);
        }

    }
}
