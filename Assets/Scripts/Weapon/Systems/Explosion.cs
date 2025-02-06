using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour, IExplosionInterface
{

    SphereCollider attachedCollider;
    public LayerMask layermask;
    public GameObject sfxToSpawn;
    public int eDamage = 150;
    public float eRadius;
    public bool eEnemyOwned = false;

    public GameObject sphere;
    public float SphereScaleSpeed = 1;
    float sphereSize = 0;

    List<GameObject> hitEnemies = new List<GameObject>();
    HitmarkerLogic hitMarkerLogic;
    ScreenShake screenShake;

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

    public void InitiateExplosion(float explosionRadius, int damage, bool enemyOwned)
    {
        
        screenShake = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ScreenShake>();

        eDamage = damage;
        eRadius = explosionRadius;
        eEnemyOwned = enemyOwned;

        
        transform.localScale = new Vector3(eRadius, eRadius, eRadius);


        //hitMarkerLogic = GameObject.FindGameObjectWithTag("HitMarker").GetComponent<HitmarkerLogicd>();
        if (sfxToSpawn)
            Instantiate(sfxToSpawn, transform.position, Quaternion.identity);
        screenShake.Screenshake(-1, 1, 1);
        
        if(enemyOwned)
        {
            layermask = LayerMask.GetMask("Player");
            Collider[] player = Physics.OverlapSphere(transform.position, eRadius * 1.5f, layermask);

            if(player.Length != 0)
                DealDamageToPlayer(player[0]);
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
            //gameObject.SetActive(false);
            Destroy(gameObject);
    }

    private void ResizeSphere()
    {

            sphereSize += Time.deltaTime * SphereScaleSpeed;
        //if (destroyTimer < 0.2 && sphereSize < 1.5)
        //else if (sphereSize > 0)
        //    sphereSize -= Time.deltaTime * SphereScaleSpeed;

        sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
    }

    private void DealDamage(Collider[] enemies)
    {

        for (int i = 0; i < enemies.Length; i++)
        {
            if (!hitEnemies.Contains(enemies[i].transform.root.gameObject))
            {

                if (enemies[i].GetComponentInParent<IDamageInterface>() != null)
                {
                    //hitMarkerLogic.EnableHitMarker();
                    enemies[i].GetComponentInParent<IDamageInterface>().Damage(eDamage);
                    hitEnemies.Add(enemies[i].transform.root.gameObject);
                }

            }
        }

    }

    private void DealDamageToPlayer(Collider player)
    {
        player.gameObject.GetComponent<IDamageInterface>().Damage(eDamage);
        
    }


    private void UpdateAlpha()
    {
        if (alpha > 0 && myMaterial)
            alpha -= Time.deltaTime * alphaFadeRate;

        //pointLight.intensity = alpha;
        myMaterial.color = new Color(myMaterial.color.r, myMaterial.color.g, myMaterial.color.b, alpha);
    }


}
