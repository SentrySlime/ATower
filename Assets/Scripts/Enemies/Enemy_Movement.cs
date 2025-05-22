using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Movement : MonoBehaviour, INoticePlayer
{
    Vector3 lastValidLocation;
    [HideInInspector] public AMainSystem aMainSystem;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public Animator animator;
    [HideInInspector] public EnemyBase enemyBase;
    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject playerTargetPoint;

    [Header("Movement")]
    public bool roam = false;
    public AudioSource walkingSFX;
    [HideInInspector] public float movementSpeed = 0;
    private float newDestinationRate = 0.85f;
    private float newDestinationTimer = 0;

    [Header("Movement perlin")]
    float noiseScale = 1.0f;
    float pathOffsetAmount = 5.0f;
    public float minUpdateRate = 0.5f;
    public float maxUpdateRate = 1f;
    float updateRate = 1f;
    float movementTimer = 0f;
    float noiseOffset = 2;

    [Header("Player Detection")]
    [Tooltip("This changes how much in front of the enemy the player has to be. \nWhere 1 is right in front and -1 is right behind.")]
    [Range(-1f, 1f)] public float dotProductForInfront = 0.5f;
    public Transform visionPoint;
    public LayerMask layerMask;
    [HideInInspector] public float interactDistance_ = 125f;
    bool foundPlayer = false;
    float playerLookForTimer = 0;
    float lineOfSightTimer = 0;

    [Header("MISC")]
    public bool isAttacking = false;
    public float playerDistance = 0;
    [HideInInspector] public Vector3 directionToPlayer;

    [Header("Base Attack Stuff")]
    public float attackRate = 2;
    [HideInInspector] public float attackRateTimer = 0;

    public void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        movementSpeed = agent.speed;

        enemyBase = GetComponent<EnemyBase>();
        animator = GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerTargetPoint = player.GetComponent<PlayerHealth>().playerTargetPoint;

        aMainSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();

        updateRate = Random.Range(minUpdateRate, maxUpdateRate);
        noiseOffset = Random.Range(0f, 100f);

    }

    public void Update()
    {
        if (!visionPoint) return;
        
        playerDistance = Vector3.Distance(visionPoint.position, playerTargetPoint.transform.position);

        if (playerDistance > 130)
        {
            print("Enemy movement don't do anything when player is too far");
            return;
        }

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

        directionToPlayer = (playerTargetPoint.transform.position - visionPoint.position).normalized;
        
        if(!foundPlayer)
            LookForPlayer();

        if (!foundPlayer) { return; }

        if (!agent || !agent.isOnNavMesh) { return; }

        if (isAttacking) { return; }

        movementTimer += Time.deltaTime;
        if (movementTimer >= updateRate)
        {
            if (agent && agent.isOnNavMesh)
            {
                CanMoveToPlayer();
                movementTimer = 0f;
            }
        }

        AttackLogic();

    }

    #region Attacks

    public virtual void StartAttack(float speed)
    {
        isAttacking = true;
        agent.speed = speed;
    }
    public virtual void AttackLogic ()
    {

    }
    public virtual void EndAttack()
    {
        isAttacking = false;
        attackRateTimer = 0f;
    }

    #endregion

    public virtual bool ReasonToStop()
    {
        return false;
    }

    private void CanMoveToPlayer()
    {

        if (ReasonToStop())
            agent.speed = 0f;
        else
            MoveToPlayer();
    }

    public void MoveToPlayer()
    {
        agent.speed = movementSpeed;

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

    private void LookForPlayer()
    {
        if (playerDistance < interactDistance_)
        {
            if (playerLookForTimer < 1)
            {
                playerLookForTimer += Time.deltaTime;
                return;
            }
            else
            {
                playerLookForTimer = 0;

                RaycastHit hit;
                if (Physics.Raycast(visionPoint.position, directionToPlayer, out hit, 200/*, ~layerMask*/))
                {

                    if (hit.transform.CompareTag("Player"))
                    {
                        foundPlayer = true;
                        roam = false;
                    }
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
            if (agent.isOnNavMesh)
            {
                agent.SetDestination(finalPosition);
            }
        }
    }

    public bool IsPlayerInfront()
    {
        Vector3 enemyforward = transform.forward;

        float dot = Vector3.Dot(enemyforward, directionToPlayer);

        if (dot > dotProductForInfront)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public bool HasLineOfSight()
    {
        RaycastHit hit;

        Debug.DrawRay(visionPoint.position, directionToPlayer * 200, Color.red, 1f);
        if (Physics.Raycast(visionPoint.transform.position, directionToPlayer, out hit, playerDistance, layerMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
            else
                return false;
        }

        return false;
    }

    void INoticePlayer.NoticePlayer()
    {
        roam = false;
        foundPlayer = true;
    }
}
