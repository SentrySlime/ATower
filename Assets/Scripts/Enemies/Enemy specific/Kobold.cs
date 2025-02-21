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

    private float idleBehaviourRate = 0.5f;
    private float idleBehaviourTimer = 0;

    [Header("Misc")]
    public bool FoundPlayer = false;
    public float moveSpeed = 7;
    public float playerDist = 0;

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




    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        SetRandomStats();
        layerMask = ~layerMask;
    }

    
    void Update()
    {
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

    public void MoveToPlayer()
    {
        agent.destination = player.transform.position;
        agent.isStopped = false;
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

        Vector3 direction = player.transform.position - shootPoint.position;

        // Create a rotation that points towards the player
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Instantiate the projectile with the correct rotation
        Instantiate(rangedProjectile, shootPoint.position, lookRotation);
        //rb.AddForce(direction.normalized * projectileSpeed, ForceMode.Impulse);
        animator.SetBool("CloseToAttack", false);

        agent.speed = moveSpeed;
        agent.isStopped = false;
        telegraphSphere.SetActive(false);

    }

    private bool HasLineOfSight()
    {
        Vector3 directionToPlayer = player.transform.position - transform.position;
        float distance = Vector3.Distance(player.transform.position, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, distance, layerMask))
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

        if(dot > 0)
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
            finalRangedDistance = UnityEngine.Random.Range(35, 40);
        else
            finalRangedDistance = UnityEngine.Random.Range(20, 25);

        //Setting random firerate
        firingRate = UnityEngine.Random.Range(0.1f, 0.7f);
        timer = firingRate;
    }
}