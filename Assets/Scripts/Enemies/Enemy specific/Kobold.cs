using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Kobold : MonoBehaviour, INoticePlayer
{
    public AudioSource audioSource;

    public AudioSource shootAudioSource;

    public enum CurrentState
    {
        idle,
        canSeePlayer,
        canReachPlayer,
    }

    public CurrentState currentState;

    public LayerMask layerMask;

    public Animator animator;
    public NavMeshAgent agent;
    GameObject player;
    Transform playerTargetPoint;

    private float idleBehaviourRate = 0.5f;
    private float idleBehaviourTimer = 0;

    [Header("Movement")]
    public bool roam = false;
    private float newDestinationRate = 0.5f;
    private float newDestinationTimer = 0;

    [Header("Misc")]
    public bool FoundPlayer = false;
    public float moveSpeed = 7;
    public float playerDist = 0;
    public GameObject visionTransform;

    [Header("FiringRate")]
    public float firingRate = 2;
    public float timer;

    [Header("Ranged")]
    public bool rangedAttack;
    public GameObject rangedProjectile;
    public float rangedDistance1 = 20;
    public float rangedDistance2 = 50;
    public float finalRangedDistance = 30;
    public float incompletRangedDistance = 50;
    public float seePlayerRangedDistance = 75;
    public Transform shootPoint;
    [Header("RangedTelegraph")]
    public GameObject telegraphSphere;

    public EnemyBase enemyBase;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerTargetPoint = player.GetComponent<PlayerHealth>().GetTargetPoint().transform;
        SetRandomStats();
        layerMask = ~layerMask;
    }

    
    void Update()
    {
        if(roam)
        {
            
        }

        playerDist = Vector3.Distance(transform.position, player.transform.position);
        
        if(agent.isOnOffMeshLink)
        {
            StartAgent();
            MoveToPlayer();
            return;
        }

        if(agent.velocity.magnitude > 0)
        {
            animator.SetBool("Moving", true);
            audioSource.UnPause();
        }
        else
        {
            animator.SetBool("Moving", false);
            audioSource.Pause();
        }

        if (currentState == CurrentState.idle)
        {
            IdleBehaviour();
        }
        else if (currentState == CurrentState.canSeePlayer)
        {
            SeePlayerBehaviour();
        }
        else if (currentState == CurrentState.canReachPlayer)
        {
            ReachPlayerBehaviour();
        }

    }

    public void IdleBehaviour()
    {
        Roam();

        if (idleBehaviourTimer < idleBehaviourRate)
        {
            idleBehaviourTimer += Time.deltaTime;
            return;
        }
        
        idleBehaviourTimer = 0;

        animator.SetBool("CloseToAttack", false);


        if (playerDist < seePlayerRangedDistance && HasLineOfSight())
        {
            if (agent.CalculatePath(player.transform.position, agent.path))
            {
                currentState = CurrentState.canReachPlayer;
            }
            else
            {
                currentState = CurrentState.canSeePlayer;
            }
        }
        else
        {
            currentState = CurrentState.idle;
        }
    }

    public void SeePlayerBehaviour()
    {
        if (playerDist < seePlayerRangedDistance)
        {
            if (!agent.CalculatePath(player.transform.position, agent.path))
            {
                if (timer < firingRate && !animator.GetBool("CloseToAttack"))
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    animator.SetBool("CloseToAttack", true);
                    timer = 0;
                }
            }
            else
            {
                currentState = CurrentState.canReachPlayer;
            }
        }
        else
        {
            currentState = CurrentState.idle;
        }
    }

    public void ReachPlayerBehaviour()
    {
        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            if (playerDist < finalRangedDistance)
            {
                Stop();

                if (timer < firingRate && !animator.GetBool("CloseToAttack"))
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    if (!HasLineOfSight()) { return; }

                    if (IsPlayerInfront())
                    {
                        animator.SetBool("CloseToAttack", true);
                        timer = 0;
                    }
                    else
                    {
                        StartAgent();
                        MoveToPlayer();
                    }
                }
            }
            else
            {
                StartAgent();
                MoveToPlayer();
            }
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            if (playerDist < incompletRangedDistance)
            {
                animator.SetBool("Moving", true);
                Stop();

                if (timer < firingRate && !animator.GetBool("CloseToAttack"))
                {
                    timer += Time.deltaTime;
                }
                else
                {
                    if(!HasLineOfSight()) { return; }

                    if (IsPlayerInfront())
                    {
                        animator.SetBool("CloseToAttack", true);
                        timer = 0;
                    }
                    else
                    {
                        StartAgent();
                        MoveToPlayer();
                    }
                }
            }
            else
            {
                SeePlayerBehaviour();
            }
        }
        else
        {
            currentState = CurrentState.idle;
        }
    }

    public void RangedAttack()
    {
        telegraphSphere.SetActive(true);
        shootAudioSource.Play();
        agent.speed = 0;
        agent.isStopped = true;

        float length = Vector3.Distance(player.transform.position, transform.position);
        StartCoroutine(ShootAfterTime(0.25f));
    }

    public void Roam()
    {
        if (newDestinationTimer < newDestinationRate)
        {
            newDestinationTimer += Time.deltaTime;
        }
        else
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 30;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10, 1);
            Vector3 finalPosition = hit.position;
            newDestinationTimer = 0;
            agent.SetDestination(finalPosition);
        }
    }

    public void MoveToPlayer()
    {
        NavMeshPath path = new NavMeshPath();
        Vector3 targetPosition = player.transform.position;

        if (agent.CalculatePath(targetPosition, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                // Full path is valid — go directly to the player
                agent.isStopped = false;
                agent.SetDestination(targetPosition);
            }
            else if (path.status == NavMeshPathStatus.PathPartial)
            {
                // Partial path — move as far as we can toward the player
                if (path.corners.Length > 1)
                {
                    Vector3 lastReachablePoint = path.corners[path.corners.Length - 1];
                    agent.isStopped = false;
                    agent.SetDestination(lastReachablePoint);
                }
            }
            else
            {
                currentState = CurrentState.idle;
            }
        }
        else
        {
            currentState = CurrentState.idle;
        }
    }


    public void Stop()
    {
        animator.SetBool("Moving", false);
        agent.velocity = Vector3.zero;
        agent.speed = 0;
        agent.ResetPath();
    }

    public void StartAgent()
    {
        animator.SetFloat("EnemySpeed", 7);
        agent.speed = moveSpeed;
    }

    IEnumerator ShootAfterTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if(enemyBase.dead) { yield return null; }

        Vector3 direction = playerTargetPoint.transform.position - shootPoint.position;

        // Create a rotation that points towards the player
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Instantiate the projectile with the correct rotation
        if(rangedProjectile)
        Instantiate(rangedProjectile, shootPoint.position, lookRotation);
        //rb.AddForce(direction.normalized * projectileSpeed, ForceMode.Impulse);
        animator.SetBool("CloseToAttack", false);

        agent.speed = moveSpeed;
        agent.isStopped = false;
        telegraphSphere.SetActive(false);

    }

    private bool HasLineOfSight()
    {
        Vector3 directionToPlayer = player.transform.position - visionTransform.transform.position;
        float distance = Vector3.Distance(player.transform.position, visionTransform.transform.position);

        RaycastHit hit;
        if (Physics.Raycast(visionTransform.transform.position, directionToPlayer, out hit, distance, layerMask))
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

    private bool IsPlayerInfront()
    {
        Vector3 enemyforward = transform.forward;

        Vector3 toPlayer = (player.transform.position - transform.position).normalized;

        float dot = Vector3.Dot(enemyforward, toPlayer);

        if(dot > 0.5)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void NoticePlayer()
    {
        currentState = CurrentState.canSeePlayer;
    }

    private void SetRandomStats()
    {
        //Setting random range
        float randomRange = UnityEngine.Random.Range(1, 100);
        if(randomRange >= 50)
        {
            finalRangedDistance = UnityEngine.Random.Range(25, 30);

        }
        else
        {
            finalRangedDistance = UnityEngine.Random.Range(15, 20);

        }

        //Setting random firerate
        firingRate = UnityEngine.Random.Range(0.1f, 0.7f);
        timer = firingRate;
    }
}