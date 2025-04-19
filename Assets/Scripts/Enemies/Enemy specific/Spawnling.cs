using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawnling : MonoBehaviour
{
    public NavMeshAgent agent;
    Vector3 lastValidLocation;
    
    GameObject player;
    ExplosionSystem explosionSystem;
    Animator animator;
    EnemyBase enemy;

    [HideInInspector] public Spawner spawner;

    bool foundPlayer;
    float timer;

    public Transform shootPoint;
    public LayerMask layerMask;
    public float interactDistance_ = 125f;
    public float firingRate = 2;

    bool attacking = false;

    [Header("Melee")]
    public bool meleeAttack;
    public GameObject meleeObj;
    public float meleeDistance_ = 10;

    [Header("Particle System")]
    public GameObject attackTelegraph;

    [Header("Movement")]
    public bool roam = false;
    private float newDestinationRate = 0.5f;
    private float newDestinationTimer = 0;

    [Header("Movement perlin")]
    float noiseScale = 1.0f;
    float pathOffsetAmount = 5.0f;
    float updateRate = 1f;

    float movementTimer = 0f;
    public float noiseOffset;

    void Start()
    {
        updateRate = Random.Range(0.5f, 1f);
        noiseOffset = Random.Range(0f, 100f);

        enemy = GetComponent<EnemyBase>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        
        player = GameObject.FindGameObjectWithTag("Player");
        explosionSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ExplosionSystem>();
        //MeleeAttack();
    }

    void Update()
    {
        
        if (roam)
        {
            Roam();
        }

        if (agent.velocity.magnitude > 0)
        {
            animator.SetBool("Moving", true);
            //audioSource.UnPause();
        }
        else
        {
            animator.SetBool("Moving", false);
            //audioSource.Pause();
        }
        NoticePlayer();

        if (!foundPlayer) { return; }

        float playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if(!agent || !agent.isOnNavMesh) { return; }


        if(attacking) { return; }

        movementTimer += Time.deltaTime;
        if (movementTimer >= updateRate)
        {
            if (agent && agent.isOnNavMesh)
            {
                EnemyMove();
                movementTimer = 0f;
            }
        }
        
        if(playerDistance < 6)
        {
            attacking = true;
            InitiateAttack();
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

    private void InitiateAttack()
    {
        if (attackTelegraph)
            Instantiate(attackTelegraph, shootPoint.transform.position, Quaternion.identity, transform);
        agent.speed = 0;
        agent.angularSpeed = 0;
        agent.velocity = Vector3.zero;
        animator.SetBool("Attacking", true);
        Invoke("DoAttack", 0.75f);
    }

    private void DoAttack()
    {
        explosionSystem.SpawnExplosion(transform.position, 10, 15, true);
        enemy.Die(false);
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
            agent.SetDestination(finalPosition);
        }
    }
}