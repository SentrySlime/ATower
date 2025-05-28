using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour, IExplosionInterface
{

    SphereCollider attachedCollider;
    public LayerMask layermask;
    //public GameObject sfxToSpawn;
    public AudioSource audioSource;
    public int eDamage = 150;
    public float eRadius;
    public bool eEnemyOwned = false;

    public GameObject sphere;
    public float SphereScaleSpeed = 1;
    float sphereSize = 0;

    List<GameObject> hitEnemies = new List<GameObject>();
    GameObject player;
    PlayerHealth playerHealth;
    GameObject playerTargetPoint;
    HitmarkerLogic hitMarkerLogic;
    ScreenShake screenShake;
    PlayerStats playerStats;
    EnemyBase enemyBase;
    public BaseWeapon weaponParent;
    public AMainSystem mainSystem;

    [Header("Light")]
    public Light pointLight;

    [Header("MaterialAlpha")]
    public GameObject mesh;
    public Material myMaterial;
    public float alpha = 1;
    private float alphaFadeRate = 2;
    

    float destroyTimer;

    private void Start()
    {
        myMaterial = mesh.GetComponent<MeshRenderer>().material;
    }

    //Interface function
    public void InitiateExplosion(AMainSystem incomingMainSystem, float explosionRadius, int damage, EnemyBase incomingEnemy, BaseWeapon weaponParent)
    {
        mainSystem = incomingMainSystem;

        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerTargetPoint = playerHealth.playerTargetPoint;
        screenShake = player.GetComponentInChildren<ScreenShake>();
        playerStats = player.GetComponent<PlayerStats>();

        if(weaponParent != null)
            this.weaponParent = weaponParent;

        eDamage = damage;
        eRadius = explosionRadius;
        if(incomingEnemy)
        {
            eEnemyOwned = true;
            enemyBase = incomingEnemy;
        }
        else
            eEnemyOwned = false;

        transform.localScale = new Vector3(eRadius, eRadius, eRadius);

        if(eRadius <= 1)
        {
            audioSource.volume = 0.01f;
        }
        else if(eRadius <= 2)
        {
            audioSource.volume = 0.03f;
        }
        else if(eRadius >= 3)
        {
            audioSource.volume = 0.05f;
        }

        screenShake.Screenshake(-1, 1, 1);

        if (eEnemyOwned)
        {
            float maxRadius = eRadius * 1.5f;

            float distanceToPlayer = Vector3.Distance(transform.position, playerTargetPoint.transform.position);
            if (distanceToPlayer > maxRadius)
                return;

            Vector3 directionToPlayer = (playerTargetPoint.transform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, directionToPlayer, maxRadius, layermask))
            {

                DealDamageToPlayer(player);
                Vector3 playerDirection = player.transform.position - transform.position;
                //Push the player
                player.GetComponent<Locomotion2>().Push();
                playerDirection = playerDirection.normalized;
                Vector3 force = playerDirection * 300;
                force.y = 30;
                player.transform.gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            }

        }
        else
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, eRadius * 1.5f, layermask);
            DealDamage(enemies);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, eRadius * 1.5f);
    }


    void Update()
    {
        UpdateAlpha();

        destroyTimer += Time.deltaTime;

        if (destroyTimer > 0.5)
            pointLight.enabled = false;

        if (destroyTimer < 2)
            ResizeSphere();
        else
            Destroy(gameObject);
    }

    private void ResizeSphere()
    {
        sphereSize += Time.deltaTime * SphereScaleSpeed;
        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
    }

    private void DealDamage(Collider[] enemies)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].CompareTag("Player")) { return; }


            if (!hitEnemies.Contains(enemies[i].transform.gameObject))
            {
                if (enemies[i].transform.CompareTag("Breakable"))
                {
                    

                    if(mainSystem)
                        mainSystem.DealDamage(enemies[i].transform.gameObject, eDamage, true);
                    
                    hitEnemies.Add(enemies[i].transform.gameObject);
                }

            }

            if (!hitEnemies.Contains(enemies[i].transform.root.gameObject))
            {
                if (enemies[i].transform.CompareTag("Enemy"))
                {
                    if (mainSystem)
                        mainSystem.DealDamage(enemies[i].transform.root.gameObject, eDamage, true);
                    
                    hitEnemies.Add(enemies[i].transform.root.gameObject);
                }
            }
        }

    }


    private void DealDamageToPlayer(GameObject player)
    {
        if (mainSystem)
            mainSystem.DealDamage(player, eDamage, false, false, enemyBase);
    }


    private void UpdateAlpha()
    {
        if (alpha > 0 && myMaterial)
            alpha -= Time.deltaTime * alphaFadeRate;

        myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, alpha);
    }


}
