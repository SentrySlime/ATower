using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Kobold;
using static UnityEngine.UI.Image;

public class Troll : MonoBehaviour, INoticePlayer
{
    GameObject player;

    [Header("Controllers")]
    public Animator trollAnimator;
    public Animator cannonAnimator;

    [Header("Movement")]
    [SerializeField] private NavMeshAgent agent;

    public bool roam = false;
    private float newDestinationRate = 0.5f;
    private float newDestinationTimer = 0;

    [SerializeField] private GameObject footStepVFX;

    [SerializeField] private GameObject rightfoot;
    [SerializeField] private GameObject leftfoot;
    
    [Header("Behaviour")]
    float interactDistance_ = 125f;
    float distanceToPlayer = 0;
    bool foundPlayer;

    [Header("DonutExplosion")]
    bool attacking = false;

    public GameObject donutExplosionVFX;
    public GameObject donutExplosion;
    public Transform donutExplosionSpawnTransform;
    public float donutExplosionTimer = 0.4f;

    public LayerMask donutLayerMask;

    float animationTimer = 0.6f;
    float speedTimer = 1.75f;

    [Header("Misc")]
    [SerializeField] private GameObject shootPoint;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if (agent.velocity.magnitude > 0)
        {
            trollAnimator.SetBool("Moving", true);
            cannonAnimator.SetBool("Moving", true);
            //audioSource.UnPause();
        }
        else
        {
            trollAnimator.SetBool("Moving", false);
            cannonAnimator.SetBool("Moving", false);
            //audioSource.Pause();
        }

        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if(!foundPlayer)
            NoticePlayer();


        if (roam)
        {
            Roam();
            return;
        }


        if(distanceToPlayer < 30)
        {
            if(attacking == false)
            {
                attacking = true;
                Invoke("ShootAnimation", 1);
            }
        }


        if (!foundPlayer) { return; }
            MoveToPlayer();

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
                print("Can't reach player");
            }
        }
        else
        {
            print("Can't reach player 2");
        }
    }

    private void NoticePlayer()
    {
        
        if (distanceToPlayer < interactDistance_)
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
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * 30;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 10, 1);
            Vector3 finalPosition = hit.position;
            newDestinationTimer = 0;
            agent.SetDestination(finalPosition);
        }
    }

    public void LeftFootVFX()
    {
        Instantiate(footStepVFX, leftfoot.transform.position + new Vector3(0, -1, 0), Quaternion.identity);
    }

    public void RightFootVFX()
    {
        Instantiate(footStepVFX, rightfoot.transform.position + new Vector3(0, -1, 0), Quaternion.identity);
    }

    #region MeleeAttack

    private void ShootAnimation()
    {
        if (trollAnimator)
        {
            trollAnimator.SetBool("Melee", true);
            trollAnimator.speed = 1f;
        }

        if (cannonAnimator)
        {
            cannonAnimator.SetBool("Melee", true);
            cannonAnimator.speed = 1f;
        }
        Invoke("IncreaseSpeed", speedTimer);
    }

    private void IncreaseSpeed()
    {
        if (trollAnimator)
        {
            trollAnimator.speed = 2.5f;
        }

        if (cannonAnimator)
        {
            cannonAnimator.speed = 2.5f;
        }

        Invoke("Idle", animationTimer);
        Invoke("DonutExplosion", donutExplosionTimer);
    }

    private void DonutExplosion()
    {
        if(donutExplosionVFX)
            Instantiate(donutExplosionVFX, donutExplosionSpawnTransform.position + new Vector3(0, -6.5f, 0), Quaternion.identity);
        
        if (donutExplosion)
        {
            RaycastHit hit;
            if (Physics.Raycast(donutExplosionSpawnTransform.position, -transform.up, out hit, 20, donutLayerMask))
            {
                Instantiate(donutExplosion, hit.point + new Vector3(0, 3, 0), Quaternion.identity);
            }
        }
    }

    #endregion

    private void Idle()
    {
        
        attacking = false;

        if (trollAnimator)
        {
            trollAnimator.SetBool("Melee", false);
            trollAnimator.speed = 1f;

        }

        if (cannonAnimator)
        {
            cannonAnimator.SetBool("Melee", false);
            cannonAnimator.speed = 1f;
        }
    }

    void INoticePlayer.NoticePlayer()
    {
        foundPlayer = true;
    }
}
