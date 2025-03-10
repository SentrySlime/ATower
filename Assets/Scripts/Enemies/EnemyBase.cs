using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour, IDamageInterface
{
    [Header("Stats")]
    [Tooltip("This decides how much this enemy should have")]
    public float maxHealth = 100;
    public float currentHealth = 100;
    public int moneyAmount = 100;

    [Header("Explosion")]
    public bool canExplode = false;
    public bool explodeOnDeath = false;
    public int explosionDamage = 100;

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
        if(currentHealth == maxHealth)
        {
            if(gameObject.GetComponent<INoticePlayer>() != null)
                gameObject.GetComponent<INoticePlayer>().NoticePlayer();
        }

        currentHealth -= CalculateDamage(damage);

        //hitMarkerLogic.EnableHitMarker();
        if (damagedPS != null)
        {
            damagedPS.emissionRate += damage;
            damagedPS.emissionRate = Mathf.Clamp(damagedPS.emissionRate, 0, 150);
        }

        if (currentHealth <= 0 && !dead)
            Die();
    }

    private float CalculateDamage(float damage)
    {
        float finalDamage = damage;

        //int critChance = Random.Range(0, 100);
        //if(playerStats.criticalChance > critChance)
        //{
        //    finalDamage *= playerStats.criticalMultiplier;
        //}

        return finalDamage;
    }

    private void Die()
    {
        dead = true;

        DetachProjectileChildren();

        if (gauranteeItemDrop)
            lootSystem.DropItem(moneySpawnPoint.position);
        else
            lootSystem.DropLoot(moneySpawnPoint.position, dropChance);

        enemyManager.ReportDeath(moneyAmount);
        if (moneySpawnPoint != null)
        {
            if(moneyPrefab)
            {
                ParticleSystem ps = Instantiate(moneyPrefab, moneySpawnPoint.position, Quaternion.identity).GetComponent<ParticleSystem>();
                ps.trigger.AddCollider(player.GetComponent<Collider>());
            }

            if (CanExplode())
            {
                aMainSystem.SpawnExplosion(moneySpawnPoint.position, 7, explosionDamage);
            }
            else if (deathParticles)
            {
               GameObject tempPS = Instantiate(deathParticles, moneySpawnPoint.position, Quaternion.identity);
               tempPS.transform.localScale = new Vector3(deathParticleSize, deathParticleSize, deathParticleSize);
            }
        }

        
        if(dieSFX)
            Instantiate(dieSFX);

        playerHealth.HealOnKill();

        Destroy(gameObject);

        //Felix was here
        if (itemPrefab)
        {
            if(Random.Range(0.1f, 100.0f) <= dropChance)
                Instantiate(itemPrefab, moneySpawnPoint.transform.position, Quaternion.identity);   
        }

    }

    private bool CanExplode()
    {

        if(!canExplode)
            return false;

        bool shouldExplode = false;
        
        if (explodeOnDeath)
            shouldExplode = true;
        else if (playerStats.canExplode)
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

    public void HealPlayer()
    {
        float newDistance = DistanceToPlayer();
        if (newDistance < 17)
        {
            float newHeal = MapValue(newDistance, 1, 21, 1f, 0f);
            newHeal *= newHeal * (maxHealth / 10);
            newHeal = Mathf.Clamp(newHeal, 5, 20);
            playerHealth.Heal(newHeal);
        }
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
