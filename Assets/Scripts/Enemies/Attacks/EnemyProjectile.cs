using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Explosions")]
    [Tooltip("If no prefab is slotted in explosion, then we call the explosion system")]
    public bool explode = false;
    public GameObject explosion;
    public int explosionDamage = 20;
    public float explosionRadius = 5;

    bool hasExploded = false;

    public bool affectedByGravity;
    public float gravityScale = 1;
    float projectileSize;
    public LayerMask layerMask;

    //---------- Projectile Stats ----------\\
    [Header("Projectile Stats")]
    public int damage = 100;
    [Range(0f, 350f)] public float projectileSpeed = 100;
    [Tooltip("Decides how long before this projectile will be destroyed")]
    public float lifeDuration = 8;


    [Header("SineWave movement")]
    public bool sineMove = false;
    [Tooltip("Decides the speed of the waves")]
    public float frequency = 20.0f;  // Speed of sine movement
    [Tooltip("Decides how large the waves will be")]
    public float magnitude = 0.5f;   // Size of sine movement
    public bool horizontal = true;
    private Vector3 axis;
    private Vector3 pos;

    //---------- Homing ----------\\
    [Header("Homing stuff")]
    public bool homing = false;

    public float minTurnSpeed = 5;
    public float maxTurnSpeed = 5;

    public float turnSpeed = 12;
    public float nonHomingDuration = 0.35f;

    [Tooltip("Will not home when within this range")]
    public float minHomingDistance = 10;
    public float maxHomeDistance = 200;

    GameObject target;
    float homingTimer = 0;
    float homingCheckTimer = 0;
    bool cantHome = false;

    //---------- Un-Changeable-Stats ----------\\
    public EnemyBase enemyBase;
    Vector3 firstPos;
    Vector3 secondPos;
    Vector3 velocity;
    GameObject player;
    PlayerHealth playerHealth;
    PlayerStats playerStats;
    AMainSystem mainSystem;
    ExplosionSystem explosionSystem;
    float lifeTimer = 0;

    public virtual void Awake()
    {
        mainSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();
        explosionSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ExplosionSystem>();
    }

    public virtual void Start()
    {
        firstPos = transform.position;
        pos = transform.position;
        if (horizontal)
            axis = transform.right;
        else
            axis = transform.up;
        player = GameObject.FindGameObjectWithTag("Player");
        projectileSize = GetComponent<SphereCollider>().radius;
        target = player;
        playerStats = player.GetComponent<PlayerStats>();
        maxHomeDistance = Vector3.Distance(transform.position, player.transform.position);
        //Destroy(gameObject, lifeDuration);
    }

    public virtual void Update()
    {
        if (sineMove)
            MoveSine();
        else if (homing && !cantHome)
            HomingLogic();
        else
            MoveForward();

        LifeTimer();

        AdvancedCollision();


    }

    private void LateUpdate()
    {
        if (affectedByGravity)
            ArcMovement();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            mainSystem.DealDamage(other.transform.gameObject, damage, false, false, enemyBase);
            //other.GetComponent<IDamageInterface>().Damage(damage);
            DestroyProjectile();
        }
        else if (explode)
        {
            Explode();

            hasExploded = true;
            DestroyProjectile();
        }
        else if(other.CompareTag("Untagged"))
        {
            DestroyProjectile();
        }
    }

    public void Initialize(EnemyBase enemy)
    {
        enemyBase = enemy;
    }

    public void ArcMovement()
    {
        velocity += Physics.gravity * gravityScale * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }


    public void MoveForward()
    {
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    private void MoveSine()
    {
        pos += transform.forward * Time.deltaTime * projectileSpeed;
        transform.position = pos + axis * Mathf.Sin(Time.time * frequency) * magnitude;
    }

    public virtual void HomingLogic()
    {
        if (homingTimer < nonHomingDuration)
        {
            homingTimer += Time.deltaTime;
            transform.position += transform.forward * projectileSpeed * Time.deltaTime;
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    public virtual void AdvancedCollision()
    {
        secondPos = transform.position;

        RaycastHit hit;
        Vector3 direction = secondPos - firstPos;
        float length = direction.magnitude;
        if (Physics.SphereCast(firstPos, projectileSize, direction, out hit, length, layerMask))
        {
            OnTriggerEnter(hit.collider);
        }

        firstPos = secondPos;
    }

    public virtual void MoveTowardsTarget()
    {
        if (player)
        {
            //Get distance to player
            float distanceApart = Vector3.Distance(transform.position, player.transform.position);
            
            // If we are too close to the player we stop homing
            if (distanceApart <= minHomingDistance)
            {
                cantHome = true;
                return;
            }

            //We map the turning speed depending on how far away the target is
            float lerp = MapValue(distanceApart, 0, maxHomeDistance, 1f, 0f);
            float newTurnSpeed = turnSpeed * lerp;
            newTurnSpeed = Mathf.Clamp(newTurnSpeed, minTurnSpeed, maxTurnSpeed);

            //Calculating rotation towards target
            var lookPos = target.transform.position - transform.position;
            var rotation = Quaternion.LookRotation(lookPos);
            
            //Smoothing the rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * newTurnSpeed);
            
            //Moving forward towards the target
            transform.position += transform.forward * projectileSpeed * Time.deltaTime;
        }
    }

    float MapValue(float mainValue, float inValueMin, float inValueMax, float outValueMin, float outValueMax)
    {
        return (mainValue - inValueMin) * (outValueMax - outValueMin) / (inValueMax - inValueMin) + outValueMin;
    }

    public void HomingCheckTimer()
    {
        if (homingCheckTimer < nonHomingDuration)
        {
            homingCheckTimer += Time.deltaTime;
        }
        else
        {
            homingCheckTimer = 0;
            //GetTarget();
        }
    }

    public void DestroyProjectile()
    {
        if (explode)
        {
            if (!hasExploded)
                Explode();
        }

        Destroy(gameObject);

    }

    private void Explode()
    {
        Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);

        if (explosion)
        {
            Instantiate(explosion, spawnPosition, Quaternion.identity).GetComponentInChildren<IInitializeProjectile>().Initialize(enemyBase); 
        }
        else
            explosionSystem.SpawnExplosion(transform.position, explosionRadius, explosionDamage, enemyBase: enemyBase);
    }

    public void LifeTimer()
    {
        if (lifeTimer < lifeDuration)
        {
            lifeTimer += Time.deltaTime;
        }
        else
            DestroyProjectile();
    }

}
