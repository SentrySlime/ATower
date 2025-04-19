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
    HitmarkerLogic hitMarkerLogic;
    ScreenShake screenShake;
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
    public void InitiateExplosion(AMainSystem incomingMainSystem, float explosionRadius, int damage, bool enemyOwned)
    {
        mainSystem = incomingMainSystem;
        screenShake = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ScreenShake>();

        eDamage = damage;
        eRadius = explosionRadius;
        eEnemyOwned = enemyOwned;

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

        //hitMarkerLogic = GameObject.FindGameObjectWithTag("HitMarker").GetComponent<HitmarkerLogicd>();
        //if (sfxToSpawn)
        //    Instantiate(sfxToSpawn, transform.position, Quaternion.identity);

        screenShake.Screenshake(-1, 1, 1);
        
        if(enemyOwned)
        {
            layermask = LayerMask.GetMask("Player");
            Collider[] player = Physics.OverlapSphere(transform.position, eRadius * 1.5f, layermask);

            if (player.Length <= 0) { return; }

            //Add raycast here
            LayerMask obstacleMask = LayerMask.GetMask("Default", "Environment"); // Adjust based on your layers



            Vector3 directionToPlayer = (player[0].transform.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player[0].transform.position);

            if (Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleMask))
            {
                // Something is in the way
                return;
            }


            if (player.Length != 0)
            {
                GameObject thePlayer = player[0].gameObject;
                DealDamageToPlayer(player[0]);
                Vector3 playerDirection = thePlayer.transform.position - transform.position;
                //Push the player
                thePlayer.transform.GetComponent<Locomotion2>().Push();
                playerDirection = playerDirection.normalized;
                Vector3 force = playerDirection * 300;
                force.y = 30;
                thePlayer.transform.gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            }
        }
        else
        {
            Collider[] enemies = Physics.OverlapSphere(transform.position, eRadius * 1.5f, layermask);
            DealDamage(enemies);
        }
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


    private void DealDamageToPlayer(Collider player)
    {
        if (mainSystem)
            mainSystem.DealDamage(player.gameObject, eDamage, false);
    }


    private void UpdateAlpha()
    {
        if (alpha > 0 && myMaterial)
            alpha -= Time.deltaTime * alphaFadeRate;

        myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, alpha);
    }


}
