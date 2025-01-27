using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Kobold : MonoBehaviour
{
    public AudioSource audioSource;

    public enum CurrentState
    {
        idle,
        canSeePlayer,
        canReachPlayer,
    }

    public CurrentState currentState;

    public Animator animator;
    public NavMeshAgent agent;
    GameObject player;

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
    public float rangedDistance = 30;
    public float incompletRangedDistance = 50;
    public float seePlayerRangedDistance = 75;
    public Transform shootPoint;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        firingRate = UnityEngine.Random.Range(0.3f, 0.5f);

    }

    private void Logging()
    {

        NavMeshHit hit;
        if (!NavMesh.Raycast(agent.transform.position, player.transform.position, out hit, NavMesh.AllAreas))
        {
            Debug.Log("Agent and target are connected on the NavMesh.");
        }
        else
        {
            Debug.Log("Agent and target are not directly connected on the NavMesh.");
        }

    }

    void Update()
    {
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
            


        playerDist = Vector3.Distance(transform.position, player.transform.position);

        Logging();

        //print(agent.pathStatus);

        if (currentState == CurrentState.idle)
        {

            //Idle action
            IdleBehaviour();
        }
        else if (currentState == CurrentState.canSeePlayer)
        {

            //See player, shoot at player
            SeePlayerBehaviour();
        }
        else if (currentState == CurrentState.canReachPlayer)
        {

            //Go to player
            ReachPlayerBehaviour();
        }

    }

    public void IdleBehaviour()
    {
        animator.SetBool("CloseToAttack", false);
        //animator.SetBool("Moving", false);

        //NavMeshPath path = new NavMeshPath();

        if (playerDist < seePlayerRangedDistance)
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
        //print("1");

        if (agent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            //print("2");
            if (playerDist < rangedDistance)
            {
                //animator.SetBool("Moving", true);
                Stop();

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
                StartAgent();
                //animator.SetBool("Moving", true);
                MoveToPlayer();
            }
        }
        else if (agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            //print("3");
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
                    animator.SetBool("CloseToAttack", true);
                    timer = 0;
                }
            }
            else
            {
                SeePlayerBehaviour();
                //StartAgent();
                //animator.SetBool("Moving", true);
                //MoveToPlayer();
            }
        }
        else
        {
            currentState = CurrentState.idle;
        }
    }

    public void RangedAttack()
    {

        Console.WriteLine("We attacked");

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
        //animator.SetBool("Moving", true);
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

    }

}