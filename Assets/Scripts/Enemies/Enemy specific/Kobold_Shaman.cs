using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Kobold_Shaman : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;
    Animator animator;
    bool foundPlayer;
    bool attacking = false;
    float distanceToPlayer = 0;

    public float interactDistance_ = 125f;
    public Transform shootPoint;

    [Header("Movement")]
    public bool roam = false;
    private float newDestinationRate = 0.5f;
    private float newDestinationTimer = 0;
    public float noiseOffset;

    [Header("Movement perlin")]
    float noiseScale = 1.0f;
    float pathOffsetAmount = 5.0f;
    float updateRate = 1f;
    float movementTimer = 0f;

    [Header("AttackStats")]
    float attackRate = 1.5f;
    public float attackRateTimer;

    [Header("FireSpire")]
    public GameObject fireSpireTelegraphVFX;
    public GameObject fireSpireVFX;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
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
            animator.SetBool("Move", true);
            //audioSource.UnPause();
        }
        else
        {
            animator.SetBool("Move", false);
            //audioSource.Pause();
        }

        NoticePlayer();

        if (!foundPlayer) { return; }

        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (!agent || !agent.isOnNavMesh) { return; }

        if (attacking) { return; }

        //if (attackRateTimer < attackRate)
        //{
        //    attackRateTimer += Time.deltaTime;
        //}
        //else if (distanceToPlayer < 25)
        //{
        //    if (attacking == false)
        //    {
        //        attacking = true;
                
        //        //Invoke("MeleeAnimation", 1);
        //    }
        //}
        //else
        //{
        //    if (attacking == false)
        //    {
        //        if (IsPlayerInfront() && HasLineOfSight())
        //        {
        //            attacking = true;
        //            InitiateShootAttack();
        //        }

        //    }
        //}

        movementTimer += Time.deltaTime;
        if (movementTimer >= updateRate)
        {
            if (agent && agent.isOnNavMesh)
            {
                EnemyMove();
                movementTimer = 0f;
            }
        }



        if(Input.GetKeyDown(KeyCode.K))
        {
            StartAttack();
            FireSpireTelegraph();
            //agent.speed = 0;
            //animator.SetBool("Evocation", true);
            //Invoke("Revoke", 1);
        }

    }

    private void Revoke()
    {
        agent.speed = 7;
        animator.SetBool("Evocation", false);
    }

    #region Movement
    public void Roam()
    {
        if (newDestinationTimer < newDestinationRate)
        {
            newDestinationTimer += Time.deltaTime;
        }
        else
        {
            Vector3 randomDirection = Random.insideUnitSphere * 30;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10, 1);
            Vector3 finalPosition = hit.position;
            newDestinationTimer = 0;

            if (agent.isOnNavMesh)
            {
                agent.SetDestination(finalPosition);
            }
            else
            {
                Debug.LogWarning("Agent is not on the NavMesh!");
            }

        }
    }

    public void EnemyMove()
    {
        NavMeshPath navMeshPath = new NavMeshPath();

        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;

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

    #endregion

    private void StartAttack()
    {
        agent.speed = 0;
    }

    private void FireSpireTelegraph()
    {
        animator.SetBool("Evocation", true);
        Instantiate(fireSpireTelegraphVFX, player.transform.position, Quaternion.identity);
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
                    roam = false;
                }
            }
        }
    }
}
