using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutExplosion : MonoBehaviour, IInitializeProjectile
{
    public float damage = 20;

    public float scaleSpeed = 1;
    //public float psScaleSpeed = 1;

    public float currentRadius = 1;
    float explosionScale = 1;

    float damageRate = 0.75f;
    float damageTimer = 0.75f;

    public float maxRadius = 50;

    AMainSystem mainSystem;
    EnemyBase enemyBase;

    public GameObject collidingObj;
    public ParticleSystem ps;
    ParticleSystem.MainModule psMain;
    ParticleSystem.ShapeModule donut;

    Vector3 currentScale = new Vector3(6.5f, 0, 6.5f);

    void Start()
    {
        psMain = ps.main;
        donut = ps.shape;
        transform.root.transform.localScale = new Vector3(explosionScale, 1, explosionScale);
        mainSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();
        psMain.startLifetime = maxRadius * 0.018f;
    }

    private void OnTriggerStay(Collider other)
    {
        if (damageTimer < damageRate) { return; }
        if (!other.CompareTag("Player")) { return; }

        float distance = Vector3.Distance(transform.position, other.transform.position);
        float tempRadius = (currentRadius - 1) * 0.5f;

        if (distance >= tempRadius)
        {
            damageTimer = 0;
            mainSystem.DealDamage(other.transform.gameObject, damage, false, false, enemyBase);
        }
        
    }

    void Update()
    {

        if(damageTimer < damageRate)
        {
            damageTimer += Time.deltaTime;
        }

        if (currentRadius < maxRadius -2)
        {
            collidingObj.transform.localScale += currentScale * Time.deltaTime * scaleSpeed;
            currentRadius = collidingObj.transform.localScale.z;
            //donut.radius += Time.deltaTime * psScaleSpeed;
        }
        else if(currentRadius > maxRadius - 2)
        {
            psMain.startColor = Color.clear;
            collidingObj.SetActive(false);         
        }
        else if (currentRadius > maxRadius + 7)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize(EnemyBase enemy)
    {
        enemyBase = enemy;
    }
}
