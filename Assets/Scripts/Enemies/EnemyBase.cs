using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class EnemyBase : MonoBehaviour, IDamageInterface
{
    [Header("Stats")]
    [Tooltip("This decides how much this enemy should have")]
    public float maxHealth = 100;
    public float currentHealth = 100;
    public bool elite = false;

    [Header("Explosion")]
    public bool canExplode = false;
    public bool explodeOnDeath = false;
    public bool explosiveBarrel = false;

    [Header("VFX  SFX")]
    [Tooltip("This spawns the money prefab")]
    public GameObject moneyPrefab;
    [Tooltip("This spawns the SFX when you kill the enemy")]
    public GameObject dieSFX;
    [Tooltip("This spawns the body parts when you kill the enemy")]
    public GameObject deathParticles;
    public float deathParticleSize = 1;

    [Header("transform points")]
    public Transform moneySpawnPoint;
    public GameObject killMarker;
    public GameObject homingTarget;

    //HealingGun healingGun;
    [HideInInspector] public Spawner spawner;
    HitmarkerLogic hitMarkerLogic;
    LayerMask layerMask;
    PlayerHealth playerHealth;
    PlayerStats playerStats;
    EnemyManager enemyManager;
    LootSystem lootSystem;
    AMainSystem aMainSystem;
    ExplosionSystem explosionSystem;
    GameObject player;
    
    Inventory inventory;
    //Felix was here
    [Header("Loot")]
    public bool dropLoot;
    public GameObject itemPrefab;
    public int itemTokens = 1;
    [HideInInspector] public bool dead = false;

    public bool gauranteeItemDrop;

    [Header("Extra")]
    public bool enableOnHitEffects = true;
    public bool shouldReportDeath = true;
    public bool canDropAmmo = true;
    public ParticleSystem damagedPS;
    List<GameObject> projectileChildren = new List<GameObject>();

    [HideInInspector] public event System.Action<EnemyBase> OnEnemyDied;

    [Header("DistanceBasedCulling")]
    [HideInInspector] public RoomScript roomScript;
    public GameObject meshObject;
    public Enemy_Movement enemy_Movement;
    bool inDistanceLastFrame = false;
    float timer = 0;

    void Start()
    {
        currentHealth = maxHealth;

        //healingGun = GameObject.FindGameObjectWithTag("Player").GetComponent<HealingGun>();
        player = GameObject.FindGameObjectWithTag("Player");

        if(player)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerStats = player.GetComponent<PlayerStats>();
            inventory = player.GetComponent<Inventory>();
        }


        GameObject hitMarkerGO = GameObject.FindGameObjectWithTag("HitMarker");

        if (hitMarkerGO != null)
        {
            hitMarkerLogic = hitMarkerGO.GetComponent<HitmarkerLogic>();

            if (hitMarkerLogic == null)
            {
                Debug.LogWarning("HitMarker GameObject found, but it is missing the HitmarkerLogic component.");
            }
        }

        GameObject gameManagerGO = GameObject.FindGameObjectWithTag("GameManager");

        if (gameManagerGO != null)
        {
            aMainSystem = gameManagerGO.GetComponent<AMainSystem>();
            if (aMainSystem == null)
            {
                Debug.LogWarning("GameManager found, but missing AMainSystem component.");
            }

            explosionSystem = gameManagerGO.GetComponent<ExplosionSystem>();

            enemyManager = gameManagerGO.GetComponent<EnemyManager>();
            if (enemyManager == null)
            {
                Debug.LogWarning("GameManager found, but missing EnemyManager component.");
            }

            lootSystem = gameManagerGO.GetComponent<LootSystem>();
            if (lootSystem == null)
            {
                Debug.LogWarning("GameManager found, but missing LootSystem component.");
            }
        }
        else
        {
            Debug.LogWarning("No GameObject with the 'GameManager' tag was found.");
        }


        if (TryGetComponent<Enemy_Movement>(out var enemyMovement))
        {
            enemy_Movement = enemyMovement;

        }

        layerMask = LayerMask.GetMask("Enemy");

        //------Particle system when damaged
        if(damagedPS != null)
        {
            var emission = damagedPS.emission;
            emission.rateOverTime = 0;
        }

        //StartCoroutine(InDistance());

    }

    private void Update()
    {
        if(timer < 0.2)
        {
            timer += Time.deltaTime;
        }
        else if (meshObject && enemy_Movement)
        {
            if (enemy_Movement.playerDistance > 130 && meshObject.activeInHierarchy)
                meshObject.SetActive(false);
            else if (enemy_Movement.playerDistance < 130 && !meshObject.activeInHierarchy)
            {
                meshObject.SetActive(true);
            }
        }
    }

    public void Damage(float damage, bool criticalHit, EnemyBase enemy)
    {
    
        if (gameObject.GetComponent<INoticePlayer>() != null)
            gameObject.GetComponent<INoticePlayer>().NoticePlayer();


        float finalDamage = CalculateDamage(damage);
        float overkillDamage = finalDamage - currentHealth;

        currentHealth -= finalDamage;

        if (damagedPS != null)
        {
            damagedPS.emissionRate += damage;
            damagedPS.emissionRate = Mathf.Clamp(damagedPS.emissionRate, 0, 150);
        }

        if (currentHealth <= 0 && !dead)
            Die(criticalHit, overkillDamage);
    }

    private float CalculateDamage(float damage)
    {

        if(enableOnHitEffects)
            aMainSystem.HitEffect();
        
        float finalDamage = damage;
        return finalDamage;
    }

    public void Die(bool criticalDeath, float overkillDamage)
    {
        dead = true;

        DetachProjectileChildren();

        if (dropLoot)
        {
            if (gauranteeItemDrop)
                lootSystem.DropItem(moneySpawnPoint.position);
            else
                lootSystem.DropLoot(moneySpawnPoint.position, itemTokens);
        }

        if(shouldReportDeath && moneySpawnPoint)
        {
            enemyManager.ReportDeath(moneySpawnPoint.position, canDropAmmo, elite, overkillDamage);
        }

        if (moneySpawnPoint != null)
        {
            if(moneyPrefab)
            {
                ParticleSystem ps = Instantiate(moneyPrefab, moneySpawnPoint.position, Quaternion.identity).GetComponent<ParticleSystem>();
                ps.trigger.AddCollider(player.GetComponent<Collider>());
            }

            if (explosiveBarrel)
                explosionSystem.SpawnExplosion(moneySpawnPoint.position, 7, 100);
            else if (CanExplode(criticalDeath))
            {
                if(explodeOnDeath)
                    explosionSystem.SpawnExplosion(moneySpawnPoint.position, 7, 15, enemyBase: this);
                else
                    explosionSystem.SpawnExplosion(moneySpawnPoint.position, 7, (int)maxHealth);
            }
            else if (deathParticles)
            {
               GameObject tempPS = Instantiate(deathParticles, moneySpawnPoint.position, Quaternion.identity);
               tempPS.transform.localScale = new Vector3(deathParticleSize, deathParticleSize, deathParticleSize);
            }
        }

        

        OnEnemyDied?.Invoke(this);

        if (spawner)
            spawner.DecreaseMinionCount();
            
        if(dieSFX)
            Instantiate(dieSFX);

        playerHealth.HealOnKill();


        //Felix was here

        Destroy(gameObject);
    }

    private bool CanExplode(bool criticalDeath)
    {

        if(!canExplode)
            return false;

        bool shouldExplode = false;
        
        if (explodeOnDeath)
            shouldExplode = true;
        else if (playerStats.canExplode > 0 && criticalDeath)
            shouldExplode = true;

        return shouldExplode;

    }

    private void OnDeathEffect()
    {

    }

    public GameObject ReturnTarget()
    {
        return homingTarget;
    }

    public float DistanceToPlayer()
    {
        if (player)
            return Vector3.Distance(transform.position, player.transform.position);
        else
            return 0;
    }

    float MapValue(float mainValue, float inValueMin, float inValueMax, float outValueMin, float outValueMax)
    {
        return (mainValue - inValueMin) * (outValueMax - outValueMin) / (inValueMax - inValueMin) + outValueMin;
    }

    public void SetProjetile(GameObject incomingProjectile)
    {
        projectileChildren.Add(incomingProjectile);
    }

    private void DetachProjectileChildren()
    {
        for (int i = 0; i < projectileChildren.Count; i++)
        {
            if (projectileChildren[i])
            {
                if (projectileChildren[i].TryGetComponent<IProjectileRelease>(out var releaseComponent))
                {
                    releaseComponent.Release();
                }

                projectileChildren[i].transform.SetParent(null, true);
            }
        }
    }

    IEnumerator InDistance()
    {
        while(!dead)
        {
            if (meshObject && enemy_Movement)
            {
                if (enemy_Movement.playerDistance > 130 && meshObject.activeInHierarchy)
                    meshObject.SetActive(false);
                else if (enemy_Movement.playerDistance < 130 && !meshObject.activeInHierarchy)
                {
                    print("Restored");
                    meshObject.SetActive(true);
                }
            }

            yield return new WaitForSeconds(1);
        }
    }



}
