using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Kobold;
using static UnityEngine.UI.Image;

public class Troll : MonoBehaviour, INoticePlayer
{
    GameObject player;
    Transform playerTargetPoint;

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
    public bool foundPlayer;

    [Header("AttackStats")]
    float attackRate = 1.5f;
    public float attackRateTimer;

    [Header("ShootAttack")]
    public GameObject shootTelegraphVFX;
    public GameObject shootExplosionSFX;
    public GameObject shootTelegraphVFXPosition;
    public GameObject shootProjectile;
    public GameObject cannonObject;

    [Header("MeleeAttack")]
    public GameObject donutExplosionVFX;
    public GameObject donutExplosionSFX;
    public GameObject donutExplosion;
    public Transform donutExplosionSpawnTransform;
    float donutExplosionTimer = 0.4f;

    public LayerMask donutLayerMask;

    public bool attacking = false;
    float animationTimer = 0.6f;
    float speedTimer = 1.75f;

    [Header("Misc")]
    [SerializeField] private GameObject shootPoint;
    [SerializeField] private GameObject visionTransform;
    public LayerMask layerMask;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerTargetPoint = player.GetComponent<PlayerHealth>().GetTargetPoint().transform;
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
            //print("Roaming");
            Roam();
            return;
        }

        if(attackRateTimer < attackRate)
        {
            attackRateTimer += Time.deltaTime;
        }
        else if (distanceToPlayer < 25)
        {
            if(attacking == false)
            {
                attacking = true;
                MeleeAnimation();
                //Invoke("MeleeAnimation", 1);
            }
        }
        else
        {
            if (attacking == false)
            {
                if (IsPlayerInfront() && HasLineOfSight())
                {
                    attacking = true;
                    InitiateShootAttack();
                }

            }
        }


        if (!foundPlayer) { return; }
            MoveToPlayer();

    }

    public void MoveToPlayer()
    {
        //print("Moving");

        NavMeshPath path = new NavMeshPath();
        Vector3 targetPosition = player.transform.position;

        if (agent.CalculatePath(targetPosition, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                //print("Path complete");

                // Full path is valid — go directly to the player
                agent.isStopped = false;
                agent.SetDestination(targetPosition);
            }
            else if (path.status == NavMeshPathStatus.PathPartial)
            {
                //print("On my way");

                // Partial path — move as far as we can toward the player
                if (path.corners.Length > 1)
                {
                    //print("Moving along path");
                    Vector3 lastReachablePoint = path.corners[path.corners.Length - 1];
                    agent.isStopped = false;
                    agent.SetDestination(lastReachablePoint);
                }
            }
            else
            {
                //print("Can't reach player");
            }
        }
        else
        {
            //print("Can't reach player 2");
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
        if(footStepVFX)
            Instantiate(footStepVFX, leftfoot.transform.position + new Vector3(0, -1, 0), Quaternion.identity);
    }

    public void RightFootVFX()
    {
        if (footStepVFX)
            Instantiate(footStepVFX, rightfoot.transform.position + new Vector3(0, -1, 0), Quaternion.identity);
    }

    #region ShootAttack

    private void InitiateShootAttack()
    {
        cannonObject.transform.localRotation = new Quaternion(-0.185198382f, -0.0976984948f, 0.263889641f, 0.941551268f);
        if(shootTelegraphVFX)
            Instantiate(shootTelegraphVFX, shootTelegraphVFXPosition.transform.position, shootTelegraphVFXPosition.transform.rotation, shootTelegraphVFXPosition.transform);
        Invoke("ShootProjectile", 1.5f);
    }

    private void ShootProjectile()
    {
        if(shootExplosionSFX)
            Instantiate(shootExplosionSFX, shootTelegraphVFXPosition.transform.position, Quaternion.identity);

        Vector3 direction = playerTargetPoint.transform.position - shootPoint.transform.position;

        // Create a rotation that points towards the player
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        // Instantiate the projectile with the correct rotation
        // Original projectile
        if(shootProjectile)
            Instantiate(shootProjectile, shootPoint.transform.position, lookRotation);

        // Left projectile (35 degrees counter-clockwise)
        Quaternion leftRotation = Quaternion.Euler(0, -15, 0) * lookRotation;
        if (shootProjectile)
            Instantiate(shootProjectile, shootPoint.transform.position, leftRotation);

        // Right projectile (35 degrees clockwise)
        Quaternion rightRotation = Quaternion.Euler(0, 15, 0) * lookRotation;
        if (shootProjectile)
            Instantiate(shootProjectile, shootPoint.transform.position, rightRotation);


        Invoke("Idle", 1);
    }

    #endregion

    #region MeleeAttack

    private void MeleeAnimation()
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
        if(donutExplosionSFX)
            Instantiate(donutExplosionSFX, donutExplosionSpawnTransform.position + new Vector3(0, -6.5f, 0), Quaternion.identity);
        
        if (donutExplosion)
        {
            RaycastHit hit;
            if (Physics.Raycast(donutExplosionSpawnTransform.position, -transform.up, out hit, 20, donutLayerMask))
            {
                if (donutExplosion)
                    Instantiate(donutExplosion, hit.point + new Vector3(0, 2.5f, 0), Quaternion.identity);
            }
        }
    }

    #endregion

    private void Idle()
    {
        cannonObject.transform.localRotation = new Quaternion(0, 0, 0, 0);
        attacking = false;
        ResetAttackTimer();
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

    private bool IsPlayerInfront()
    {
        Vector3 enemyforward = transform.forward;

        Vector3 toPlayer = (player.transform.position - transform.position).normalized;

        float dot = Vector3.Dot(enemyforward, toPlayer);

        if (dot > 0.75)
        {
            return true;
        }
        else
        {
            return false;
        }

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

    private void ResetAttackTimer()
    {
        attackRateTimer = 0;
    }

    void INoticePlayer.NoticePlayer()
    {
        foundPlayer = true;
    }
}