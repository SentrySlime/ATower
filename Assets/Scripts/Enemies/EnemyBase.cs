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
    GameObject player;
    Inventory inventory;
    //Felix was here
    [Header("Loot")]
    public bool dropLoot;
    public GameObject itemPrefab;
    public float dropChance = 0.05f;
    [HideInInspector] public bool dead = false;

    public bool gauranteeItemDrop;

    [Header("Extra")]
    public bool enableOnHitEffects = true;
    public bool shouldReportDeath = true;
    public bool canDropAmmo = true;
    public ParticleSystem damagedPS;
    List<GameObject> projectileChildren = new List<GameObject>();

    void Start()
    {
        currentHealth = maxHealth;

        //healingGun = GameObject.FindGameObjectWithTag("Player").GetComponent<HealingGun>();
        player = GameObject.FindGameObjectWithTag("Player");

        if(player)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            playerStats = player.GetComponent<PlayerStats>();
        }
        hitMarkerLogic = GameObject.FindGameObjectWithTag("HitMarker").GetComponent<HitmarkerLogic>();
        aMainSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();
        enemyManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EnemyManager>();
        lootSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LootSystem>();
        inventory = player.GetComponent<Inventory>();

        layerMask = LayerMask.GetMask("Enemy");

        //------Particle system when damaged
        if(damagedPS != null)
        {
            var emission = damagedPS.emission;
            emission.rateOverTime = 0;
        }
    }

    void Update()
    {
        

    }

    public void Damage(float damage)
    {
        if (currentHealth == maxHealth)
        {
            if (gameObject.GetComponent<INoticePlayer>() != null)
                gameObject.GetComponent<INoticePlayer>().NoticePlayer();
        }

        currentHealth -= CalculateDamage(damage);

        if (damagedPS != null)
        {
            damagedPS.emissionRate += damage;
            damagedPS.emissionRate = Mathf.Clamp(damagedPS.emissionRate, 0, 150);
        }

        if (currentHealth <= 0 && !dead)
            Die(false);
    }

    public void Damage(float damage, bool criticalHit)
    {
        if (currentHealth == maxHealth)
        {
            if (gameObject.GetComponent<INoticePlayer>() != null)
                gameObject.GetComponent<INoticePlayer>().NoticePlayer();
        }



        currentHealth -= CalculateDamage(damage);

        if (damagedPS != null)
        {
            damagedPS.emissionRate += damage;
            damagedPS.emissionRate = Mathf.Clamp(damagedPS.emissionRate, 0, 150);
        }

        if (currentHealth <= 0 && !dead)
            Die(criticalHit);
    }

    private float CalculateDamage(float damage)
    {

        if(enableOnHitEffects)
            aMainSystem.HitEffect();
        
        float finalDamage = damage;
        return finalDamage;
    }

    public void Die(bool criticalDeath)
    {
        dead = true;

        DetachProjectileChildren();

        if (dropLoot)
        {
            if (gauranteeItemDrop)
                lootSystem.DropItem(moneySpawnPoint.position);
            else
                lootSystem.DropLoot(moneySpawnPoint.position, dropChance);
        }

        if(shouldReportDeath && moneySpawnPoint)
            enemyManager.ReportDeath(moneySpawnPoint.position, canDropAmmo, elite);

        if (moneySpawnPoint != null)
        {
            if(moneyPrefab)
            {
                ParticleSystem ps = Instantiate(moneyPrefab, moneySpawnPoint.position, Quaternion.identity).GetComponent<ParticleSystem>();
                ps.trigger.AddCollider(player.GetComponent<Collider>());
            }

            if (CanExplode(criticalDeath))
            {
                if(explodeOnDeath)
                    aMainSystem.SpawnExplosion(moneySpawnPoint.position, 7, 15, true);
                else
                    aMainSystem.SpawnExplosion(moneySpawnPoint.position, 7, (int)maxHealth);
            }
            else if (deathParticles)
            {
               GameObject tempPS = Instantiate(deathParticles, moneySpawnPoint.position, Quaternion.identity);
               tempPS.transform.localScale = new Vector3(deathParticleSize, deathParticleSize, deathParticleSize);
            }
        }

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
            if(projectileChildren[i])
                projectileChildren[i].transform.SetParent(null, true);
        }
    }

    
}
