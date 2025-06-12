using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTurret : MonoBehaviour
{
    GameObject gameManager;
    AMainSystem mainSystem;


    GameObject player;
    PlayerHealth playerHealth;
    GameObject target;
    EnemyBase enemyBase;
    IDamageInterface idamageInterface;

    public LineRenderer beamLineRender;

    public GameObject projectile;
    public float attackRate = 5f;
    public LayerMask layerMask;

    [HideInInspector] public Vector3 directionToPlayer;
    public float playerDistance = 0;

    public float attackRateTimer = 0f;


    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        mainSystem = gameManager.GetComponent<AMainSystem>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        target = playerHealth.GetTargetPoint();
        
        enemyBase = GetComponent<EnemyBase>();
        idamageInterface = GetComponent<IDamageInterface>();
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(transform.position, target.transform.position);
        directionToPlayer = (target.transform.position - transform.position).normalized;

        if (!HasLineOfSight()) return;

        if (attackRateTimer < attackRate)
            attackRateTimer += Time.deltaTime;
        else
        {
            print(attackRateTimer);
            FireRayCast();
            attackRateTimer = 0;
            
            //FireProjectile();
            
            
        }
    }

    private void FireProjectile()
    {
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        EnemyProjectile enemyProjectile = Instantiate(projectile, transform.position, lookRotation).GetComponent<EnemyProjectile>();
        enemyProjectile.Initialize(enemyBase);
    }

    private void FireRayCast()
    {
        beamLineRender.enabled = true;
        beamLineRender.SetPosition(0, transform.position);
        Vector3 hitPosition = transform.position + transform.forward * 200;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, playerDistance, layerMask))
        {
            hitPosition = hit.point;
            
            if (hit.transform.CompareTag("Player"))
            {
                mainSystem.DealDamage(player, 5, false, enemyBase: enemyBase, iDamageInterface: idamageInterface);

                beamLineRender.SetPosition(0, transform.position);
                beamLineRender.SetPosition(1, hitPosition);
            }
        }

        beamLineRender.SetPosition(1, hitPosition);
        StartCoroutine(DisableLineRender());
    }

    IEnumerator DisableLineRender()
    {
        yield return new WaitForSeconds(0.25f);
        beamLineRender.enabled = false;
    }

    public bool HasLineOfSight()
    {
        RaycastHit hit;

        Debug.DrawRay(transform.position, directionToPlayer * 200, Color.red, 1f);
        if (Physics.Raycast(transform.position, directionToPlayer, out hit, playerDistance, layerMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
            else
            {
                attackRateTimer = 0;
                return false;
            }
        }

        attackRateTimer = 0;
        return false;
    }

}
