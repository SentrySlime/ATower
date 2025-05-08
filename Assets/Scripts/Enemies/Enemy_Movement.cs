using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Movement : MonoBehaviour
{
    NavMeshAgent agent;
    Vector3 lastValidLocation;
    Animator animator;
    EnemyBase enemyBase;
    GameObject player;



    [Header("Movement")]
    public bool roam = false;
    public AudioSource walkingSFX;
    private float newDestinationRate = 0.5f;
    private float newDestinationTimer = 0;


    [Header("Movement perlin")]
    float noiseScale = 1.0f;
    float pathOffsetAmount = 5.0f;
    float updateRate = 1f;
    float movementTimer = 0f;
    float noiseOffset = 2;

    [Header("Player Detection")]
    public Transform visionPoint;
    public LayerMask layerMask;
    public float interactDistance_ = 125f;
    bool foundPlayer = false; 

    [Header("MISC")]
    public bool attacking = false;
    float playerDistance = 0;
    Vector3 directionToPlayer;

    void Start()
    {
        updateRate = Random.Range(0.5f, 1f);
        noiseOffset = Random.Range(0f, 100f);

        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (roam)
        {
            Roam();
        }

        if (agent.velocity.magnitude > 0)
        {
            if(animator)
                animator.SetBool("Moving", true);

            if(walkingSFX)
                walkingSFX.UnPause();
        }
        else
        {
            if (animator)
                animator.SetBool("Moving", false);

            if (walkingSFX)
                walkingSFX.Pause();
        }

        NoticePlayer();

        if (!foundPlayer) { return; }

        playerDistance = Vector3.Distance(transform.position, player.transform.position);
        directionToPlayer = (player.transform.position - transform.position).normalized;

        if (!agent || !agent.isOnNavMesh) { return; }


        if (attacking) { return; }

        movementTimer += Time.deltaTime;
        if (movementTimer >= updateRate)
        {
            if (agent && agent.isOnNavMesh)
            {
                EnemyMove();
                movementTimer = 0f;
            }
        }
    }

    public void EnemyMove()
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        float time = Time.time + noiseOffset;
        float noiseX = Mathf.PerlinNoise(time * noiseScale, 0f);
        float noiseZ = Mathf.PerlinNoise(0f, time * noiseScale);

        Vector3 noiseOffsetVec = new Vector3((noiseX - 0.5f) * 2f, 0, (noiseZ - 0.5f) * 2f) * pathOffsetAmount;

        Vector3 targetPosition = player.transform.position + noiseOffsetVec;

        NavMeshPath path = new NavMeshPath();

        if (agent.CalculatePath(targetPosition, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                agent.isStopped = false;
                agent.SetDestination(targetPosition);
            }
            else if (path.status == NavMeshPathStatus.PathPartial)
            {
                if (path.corners.Length > 1)
                {
                    Vector3 lastReachablePoint = path.corners[path.corners.Length - 1];
                    agent.isStopped = false;
                    agent.SetDestination(lastReachablePoint);
                }
            }
            else
            {
                Roam();
            }
        }
    }

    private void MeleeAttack()
    {
        

    }

    private void RangedAttack()
    {
        
    }

    public virtual void OnButtonPress()
    {

    }

    private void NoticePlayer()
    {
        if (playerDistance < interactDistance_)
        {

            RaycastHit hit;
            if (Physics.Raycast(visionPoint.transform.position, directionToPlayer, out hit, 400/*, ~layerMask*/))
            {
                Debug.DrawLine(visionPoint.transform.position, directionToPlayer * 200);
                if (hit.transform.CompareTag("Player"))
                {
                    foundPlayer = true;
                    roam = false;
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
