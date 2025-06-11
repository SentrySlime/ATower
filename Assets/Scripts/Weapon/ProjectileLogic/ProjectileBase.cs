using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ProjectileBase : MonoBehaviour
{

    public GameObject explosivePrefab;

    //---------- Calculations ----------\\
    [Tooltip("This decides what layers CAN be hit")]
    LayerMask layerMask;
    LayerMask enemyMask;


    [Tooltip("Use raycast to interact with objects [NEEDS TO BE TURNED ON FOR HIGH SPEED PROJECTILES]")]
    public bool UseAdvancedCalculations = true;

    //---------- Projectile Stats ----------\\
    [Header("Projectile Stats")]
    public int damage = 100;
    [Range(0f, 350f)] public float projectileSpeed = 100;

    //---------- Piercing ----------\\
    [Header("Piercing")]
    [Tooltip("Decides if we can pierce objects such as walls")]
    public bool pierceObjects = false;

    [Tooltip("Decides how many enemies (or objects) we can pierce before destroy")]
    public int pierceAmount = 0;

    //---------- Explosive ----------\\
    [Header("Explosive Stats")]
    [Tooltip("If this is explosive, it only deals explosive damage")]
    public bool isExplosive = false;

    [Tooltip("How large the explosive radius is")]
    public float explosiveRadius = 1;
    public int explosiveDamage = 10;

    public GameObject explosiveImpactSFX;

    bool hasExploded = false;

    //---------- Homing ----------\\
    [Header("Homing stuff")]
    public bool homing = false;
    public float turnSpeed = 12;
    public float nonHomingDuration = 0.35f;
    public float homingRange = 25;

    public GameObject target;
    float homingTimer = 0;
    float homingCheckTimer = 0;
    bool cantHome = false;

    //---------- Burning ----------\\
    [Header("Burning Effect")]
    public bool burning = false;
    public float burningDamage = 1;

    [Tooltip("1 is equal to 1% chance to apply the status effect")]
    [Range(0f, 100f)] public int burnChance = 0;

    //---------- Gravity ----------\\
    [Header("Gravity")]
    public bool useGravity = false;
    public float gravityMagnitude = 1;
    Vector3 velocity;

    [Header("VFX")]
    public GameObject hitVFX;
    public GameObject hitEnemyVFX;

    //[Tooltip("If true, this OBJ can interact with other projectiles")]
    //bool projectileInteraction = false;
    //bool onlyHitEnemies = false;

    public float lifeDuration = 8;
    public float lifeTimer = 0;
    public bool canDestroy = true;
    float projectileSize;
    Vector3 firstPos;
    Vector3 secondPos;
    PlayerStats playerStats;
    [HideInInspector] public BaseWeapon weaponParent;
    //ExplosionSystem explosionSystem;
    [HideInInspector] public AMainSystem aMainSysteM;
    [HideInInspector] public ExplosionSystem explosionSystem;
    [HideInInspector] public ShootSystem shootSystem;
    HitmarkerLogic hitMarkerLogic;
    List<GameObject> hitEnemies = new List<GameObject>();

    public virtual void Start()
    {
        firstPos = transform.position;
        projectileSize = GetComponent<SphereCollider>().radius;
        layerMask = LayerMask.GetMask("Default", "Water", "Ground", "Enemy", "Breakable");

        //hitMarkerLogic = GameObject.FindGameObjectWithTag("HitMarker").GetComponent<HitmarkerLogicd>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        aMainSysteM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();
        shootSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ShootSystem>();
        explosionSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ExplosionSystem>();
        enemyMask = LayerMask.GetMask("Enemy");


    }

    public virtual void Update()
    {
        CheckLifeTimer();

        if (homing)
        {
            HomingLogic();
        }
        else
        {
            MoveForward();
        }

        CallOnUpdate();

        if (UseAdvancedCalculations)
            AdvancedCollision();

    }


    private void OnTriggerEnter(Collider other)
    {    
        OnHitTrigger(other);
    }

    protected void OnHitTrigger(Collider other, Vector3 hitPos)
    {
        if (other.gameObject.layer == 2)
        {
            return;
        }

        if (other.gameObject.layer == 0 && !isExplosive)
        {
            if(hitVFX)
                Instantiate(hitVFX, hitPos, Quaternion.LookRotation(-transform.forward));
            EndObjectLife();
        }

        if (other.CompareTag("Enemy"))
        {
            if (isExplosive)
            {
                ExplosiveShot(other.transform.root.gameObject);
            }
            else
            {
                if (hitEnemyVFX)
                    Instantiate(hitEnemyVFX, hitPos, Quaternion.LookRotation(-transform.forward));
                ImpactBehaviour(other.transform.root.gameObject, false, hitPos);
            }
        }
        if (other.CompareTag("WeakSpot"))
        {
            if (isExplosive)
            {
                ExplosiveShot(other.transform.root.gameObject);
            }
            else
            {
                if (hitEnemyVFX)
                    Instantiate(hitEnemyVFX, hitPos, Quaternion.LookRotation(-transform.forward));
                ImpactBehaviour(other.transform.root.gameObject, true, hitPos);
            }
        }
        else if(other.CompareTag("Breakable"))
        {
            if (isExplosive)
            {
                ExplosiveShot(other.gameObject);
            }
            else
            {
                if (hitVFX)
                    Instantiate(hitVFX, hitPos, Quaternion.LookRotation(-transform.forward));
                ImpactBehaviour(other.gameObject, false, hitPos);
                pierceAmount = 0;
                CheckPierce();
            }
        }
        else if (isExplosive)
        {
            ExplosiveShot(other.transform.root.gameObject);
        }
        else if (!pierceObjects)
        {
            if (other.CompareTag("Player")) { return; }
            EndObjectLife();


        }
        else
            CheckPierce();
    }

    protected void OnHitTrigger(Collider other)
    {
        if (other.gameObject.layer == 2)
        {
            return;
        }

        if (other.gameObject.layer == 0 && !isExplosive)
        {
            if (hitVFX)
                Instantiate(hitVFX, transform.position, Quaternion.LookRotation(-transform.forward));
            EndObjectLife();
        }

        if (other.CompareTag("Enemy"))
        {
            if (isExplosive)
            {
                ExplosiveShot(other.transform.root.gameObject);
            }
            else
            {
                if (hitEnemyVFX)
                    Instantiate(hitEnemyVFX, transform.position, Quaternion.LookRotation(-transform.forward));
                ImpactBehaviour(other.transform.root.gameObject, false, transform.position);
            }
        }
        if (other.CompareTag("WeakSpot"))
        {
            if (isExplosive)
            {
                ExplosiveShot(other.transform.root.gameObject);
            }
            else
            {
                if (hitEnemyVFX)
                    Instantiate(hitEnemyVFX, transform.position, Quaternion.LookRotation(-transform.forward));
                ImpactBehaviour(other.transform.root.gameObject, true, transform.position);
            }
        }
        else if (other.CompareTag("Breakable"))
        {
            if (isExplosive)
            {
                ExplosiveShot(other.gameObject);
            }
            else
            {
                if (hitVFX)
                    Instantiate(hitVFX, transform.position, Quaternion.LookRotation(-transform.forward));
                ImpactBehaviour(other.gameObject, false, transform.position);
                pierceAmount = 0;
                CheckPierce();
            }
        }
        else if (isExplosive)
        {
            ExplosiveShot(other.transform.root.gameObject);
        }
        //else if (!pierceObjects)
        //{
        //    if (other.CompareTag("Player")) { return; }
        //    EndObjectLife();


        //}
        else
            CheckPierce();
    }


    public virtual void ImpactBehaviour(GameObject hitEnemy, bool incomingWeakSpotShot, Vector3 hitPos)
    {
        if (!hitEnemies.Contains(hitEnemy))
        {
            if (hitEnemy.GetComponent<IDamageInterface>() != null)
            {
                DealDamage(hitEnemy, incomingWeakSpotShot);
            }
        }

        CheckPierce();
    }


    

    protected void DealDamage(GameObject enemyRoot, bool incomingWeakSpotShot)
    {
        //hitMarkerLogic.EnableHitMarker();
        aMainSysteM.DealDamage(enemyRoot, damage, true, incomingWeakSpotShot);
        
        if(burning)
            ApplyBurningEffect(enemyRoot);
        
        //enemyRoot.GetComponentInParent<IDamageInterface>().Damage(damage);
        hitEnemies.Add(enemyRoot);
        CheckPierce();
    }

    protected void ApplyBurningEffect(GameObject enemyRoot)
    {
        if(enemyRoot.TryGetComponent<BurningStatusEffect>(out var burn))
            burn.StartEffect(burnChance, burningDamage);
    }

    protected void AdvancedCollision()
    {
        secondPos = transform.position;

        RaycastHit hit;
        Vector3 direction = secondPos - firstPos;
        float length = direction.magnitude;
        if (Physics.SphereCast(firstPos, projectileSize, direction, out hit, length, layerMask))
        {
            OnHitTrigger(hit.collider, hit.point);
        }

        firstPos = secondPos;
    }



    protected void ExplosiveShot(GameObject parent)
    {
        if (hasExploded) return;

        explosionSystem.SpawnExplosion(transform.position, explosiveRadius, explosiveDamage, parent, weaponParent);
 
        hasExploded = true;
        
        if(pierceObjects)
            CheckPierce();
    }



    protected void CheckPierce()
    {
        if(!pierceObjects)
            EndObjectLife();

        pierceAmount--;

        if (pierceAmount < 0)
        {
            EndObjectLife();
        }
    }

    protected void CheckLifeTimer()
    {
        if (lifeTimer < lifeDuration)
        {
            lifeTimer += Time.deltaTime;
        }
        else
        {
            canDestroy = true;
            EndObjectLife();
        }
    }

    protected void EndObjectLife()
    {
        if(canDestroy)
        //-----Eventually pool this object
        Destroy(gameObject);
    }

    #region Movement Logic -----------------------------------
    protected void MoveForward()
    {
        if (useGravity)
        {
            velocity += Physics.gravity * gravityMagnitude * Time.deltaTime;
            transform.position += velocity * Time.deltaTime;
            transform.position += transform.forward * projectileSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += transform.forward * projectileSpeed * Time.deltaTime;
        }
    }

    protected void HomingLogic()
    {
        if (homingTimer < nonHomingDuration)
        {
            MoveForward();
            homingTimer += Time.deltaTime;
        }
        else if (cantHome)
        {
            MoveForward();
            HomingCheckTimer();
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    protected void MoveTowardsTarget()
    {
        if (!target)
        {
            GetTarget();
        }
        else
        {
            var lookPos = target.transform.position - transform.position;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * turnSpeed);
            transform.position += transform.forward * projectileSpeed * Time.deltaTime;
        }
    }

    protected void HomingCheckTimer()
    {
        if (homingCheckTimer < nonHomingDuration)
        {
            homingCheckTimer += Time.deltaTime;
        }
        else
        {
            homingCheckTimer = 0;
            GetTarget();
        }
    }

    protected void GetTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, homingRange, enemyMask);

        if (targets.Length == 0)
        {
            cantHome = true;
        }
        else
        {
            cantHome = false;
            var orderedByProximity = targets.OrderBy(c => (transform.position - c.transform.position).sqrMagnitude).ToArray();
            target = orderedByProximity[0].transform.gameObject;
            target = target.transform.root.GetComponent<EnemyBase>().ReturnTarget();
        }

    }

    public virtual void CallOnUpdate()
    {

    }

    #endregion
}
