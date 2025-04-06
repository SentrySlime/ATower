using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class ShootRaycast : BaseWeapon
{

    public float hitDistance = 0;

    HitmarkerLogic hitMarkLogic;
    AMainSystem aMainSystem;
    public GameObject hitmarker;

    [Header("Shotgun")]
    public bool isShotgun = false;

    [Header("Weapon Attributes")]
    public int shootAmount = 1;
    public float shotSize = 1;
    public int pierceAmount = 1;

    [Header("Explosive")]
    public bool ShouldExplode = false;
    public int explosiveDamage = 1;
    public float explosiveRadius = 1;

    LayerMask layermask;
    LayerMask layermask2;
    LayerMask layermask3;

    float weaponRange = 99999;

    float minYOffset = 0;
    float maxYOffset = 0;
    
    float minXoffset = 0;
    float maxXoffset = 0;
    public List<AudioSource> SFXList = new List<AudioSource>();
    int soundCount;

    [Header("SFX & VFX")]
    public GameObject shootSFX;
    public GameObject hitSFX;

    public Transform effectPosition;
    public GameObject shootEffect;

    public ParticleSystem muzzleFlash;
    public GameObject hitVFX;
    public GameObject hitEnemyVFX;

    public ParticleSystem casingVFX;

    public Animation mantleAnimation;


    private void Awake()
    {
        SetBaseStatsOnSpawn();

        //This layermask sends a single raycast and should basically only hit terrain
        layermask = LayerMask.GetMask("Player", "Enemy", "Projectile", "Items", "Breakable", "Ignore Raycast");
        
        //This actually only tries to get the enemies in the way
        layermask2 = LayerMask.GetMask("Enemy", "WeaponLayer", "Breakable");
        
        //This layermask is only for shotguns
        layermask3 = LayerMask.GetMask("Player", "Projectile", "Items", "Ignore Raycast");

        //shootPoint = GameObject.Find("FX").GetComponent<GameObject>();
        shootPoint = GameObject.FindGameObjectWithTag("ShootPoint");
        aMainSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();
        //hitMarkLogic = GameObject.FindGameObjectWithTag("HitMarker").GetComponent<HitmarkerLogicd>();

        recoil = GetComponentInParent<Recoil>();
        screenShake = GetComponentInParent<ScreenShake>();
        weaponSocket = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponSocket>();
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        
        maxAmmoText = GameObject.Find("MaxAmmoText").GetComponent<TextMeshProUGUI>();
        currentMagazineText = GameObject.Find("CurrentMagazineText").GetComponent<TextMeshProUGUI>();
        //ammoFill = GameObject.FindGameObjectWithTag("Fill").GetComponent<Image>();
        //ammoFill = GameObject.Find("Fill").GetComponent<Image>();

        currentAmmo = maxAmmo;
        currentMagazine = maxMagazine;

    }

    public override void Start()
    {
        base.Start();
        layermask = ~layermask;


    }


    public override void TriggerItem()
    {


        #region NotShooting
        if (casingVFX != null)
            casingVFX.Play();

        if (aimCamera == null || screenShake == null) { return; }

        screenShake.RecoilFire();

        if (SFXList.Count >= 20)
        {
            SFXList[soundCount].PlayOneShot(SFXList[soundCount].clip);
            if (soundCount >= 18)
                soundCount = 0;
            else
                soundCount++;
        }
        else
        {

            SFXList.Add(Instantiate(shootSFX.GetComponent<AudioSource>()));

        }

        if (muzzleFlash)
            muzzleFlash.Play();

        #endregion
        if(effectPosition)
        {
            Instantiate(shootEffect, effectPosition.position, transform.rotation);
        }

        #region Shooting
        if (!isShotgun)
        {
            //This is where we se the random numbers for the accuracy
            #region RandomNumbers Accuracy
            if (weaponSocket.adsProgress < 0.9)
            {
                minYOffset = Random.Range(-ADSAccuracy, 0);
                maxYOffset = Random.Range(ADSAccuracy, 0);

                minXoffset = Random.Range(-ADSAccuracy, 0);
                maxXoffset = Random.Range(ADSAccuracy, 0);
            }
            else
            {
                minYOffset = Random.Range(-HIPAccuracy, 0);
                maxYOffset = Random.Range(HIPAccuracy, 0);

                minXoffset = Random.Range(-HIPAccuracy, 0);
                maxXoffset = Random.Range(HIPAccuracy, 0);
            }

            #endregion

            shootPoint.transform.Rotate(((minXoffset + maxXoffset)), ((minYOffset + maxYOffset)), 0);

            //Only hitmarkers on enemy hit
            //Different VFX on enemy hit and object hit
            RaycastHit raycastHit;
            //This is a small raycast to check the distance that the SphereCast should go to
            if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out raycastHit, weaponRange, layermask))
            {
                hitDistance = Vector3.Distance(shootPoint.transform.position, raycastHit.point);
            }
            else
            {
                hitDistance = weaponRange;
            }

            //This spherecast gets all enemies that we hit (does not apply to shotguns)
            RaycastHit[] hits = Physics.SphereCastAll(shootPoint.transform.position, shotSize, shootPoint.transform.forward, hitDistance, layermask2);

            hits = hits.OrderBy(go => go.distance).ToArray();


            List<GameObject> alreadyDamaged = new List<GameObject>();
            List<GameObject> enemiesToDamage = new List<GameObject>();

            for (int i = 0; i < hits.Length; i++)
            {

                if (!alreadyDamaged.Contains(hits[i].transform.root.gameObject))
                {

                    if (hits[i].transform.CompareTag("Breakable"))
                    {
                        alreadyDamaged.Add(hits[i].transform.gameObject);
                        enemiesToDamage.Add(hits[i].transform.gameObject);
                    }
                    else
                    {
                        alreadyDamaged.Add(hits[i].transform.root.gameObject);
                        enemiesToDamage.Add(hits[i].transform.gameObject);
                    }

                    
                }
            }

            Debug.DrawLine(aimCamera.transform.position, shootPoint.transform.forward * 999, Color.red, .2f);
            int tempPierce;

            if (pierceAmount <= alreadyDamaged.Count)
            {
                tempPierce = pierceAmount;

            }
            else
            {
                tempPierce = alreadyDamaged.Count;

                if (ShouldExplode)
                {
                    aMainSystem.SpawnExplosion(raycastHit.point, explosiveRadius, explosiveDamage);
                }
                else
                {
                    Vector3 hitDirection = transform.position - raycastHit.point;
                    Instantiate(hitVFX, raycastHit.point, Quaternion.LookRotation(hitDirection));
                }
            }

            for (int i = 0; i < tempPierce; i++)
            {
                if (enemiesToDamage[i].transform.CompareTag("Enemy"))
                {
                    if(ShouldExplode)
                    {
                        aMainSystem.SpawnExplosion(hits[i].point, explosiveRadius, explosiveDamage, alreadyDamaged[i]);
                    }
                    else
                    {
                        DealDamage(alreadyDamaged[i], false);
                        var fwd = Vector3.ProjectOnPlane(transform.forward, raycastHit.normal);
                        var tempRot = Quaternion.LookRotation(fwd, raycastHit.normal);
                        Vector3 hitDirection = transform.position - raycastHit.point;
                        Instantiate(hitEnemyVFX, hits[i].point, Quaternion.LookRotation(hitDirection), alreadyDamaged[i].transform);
                    }

                }
                else if (enemiesToDamage[i].transform.CompareTag("WeakSpot"))
                {
                    if (ShouldExplode)
                    {
                        aMainSystem.SpawnExplosion(hits[i].point, explosiveRadius, explosiveDamage, alreadyDamaged[i]);
                    }
                    else
                    {
                        DealDamage(alreadyDamaged[i], true);
                        var fwd = Vector3.ProjectOnPlane(transform.forward, raycastHit.normal);
                        var tempRot = Quaternion.LookRotation(fwd, raycastHit.normal);
                        Vector3 hitDirection = transform.position - raycastHit.point;
                        Instantiate(hitEnemyVFX, hits[i].point, Quaternion.LookRotation(hitDirection), alreadyDamaged[i].transform);
                    }
                }
                else if(alreadyDamaged[i].transform.CompareTag("Breakable"))
                {
                    if (ShouldExplode)
                    {
                        aMainSystem.SpawnExplosion(hits[i].point, explosiveRadius, explosiveDamage, alreadyDamaged[i]);
                    }
                    else
                    {
                        DealDamage(alreadyDamaged[i], false);
                        var fwd = Vector3.ProjectOnPlane(transform.forward, raycastHit.normal);
                        var tempRot = Quaternion.LookRotation(fwd, raycastHit.normal);
                        Vector3 hitDirection = transform.position - raycastHit.point;
                        Instantiate(hitVFX, hits[i].point, Quaternion.LookRotation(hitDirection), alreadyDamaged[i].transform);
                        tempPierce = 0;
                        
                    }
                }
                else if (alreadyDamaged[i].transform.CompareTag("Ground"))
                {
                    var fwd = Vector3.ProjectOnPlane(transform.forward, raycastHit.normal);
                    var tempRot = Quaternion.LookRotation(fwd, raycastHit.normal);
                    Vector3 hitDirection = transform.position - raycastHit.point;
                    Instantiate(hitVFX, hits[i].point, Quaternion.LookRotation(hitDirection), alreadyDamaged[i].transform);

                    break;
                }

            }

            shootPoint.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            for (int i = 0; i < shootAmount; i++)
            {

                #region RandomNumbers Accuracy
                if (weaponSocket.adsProgress < 0.9)
                {
                    minYOffset = Random.Range(-ADSAccuracy, 0);
                    maxYOffset = Random.Range(ADSAccuracy, 0);

                    minXoffset = Random.Range(-ADSAccuracy, 0);
                    maxXoffset = Random.Range(ADSAccuracy, 0);
                }
                else
                {
                    minYOffset = Random.Range(-HIPAccuracy, 0);
                    maxYOffset = Random.Range(HIPAccuracy, 0);

                    minXoffset = Random.Range(-HIPAccuracy, 0);
                    maxXoffset = Random.Range(HIPAccuracy, 0);
                }

                #endregion

                shootPoint.transform.Rotate(((minXoffset + maxXoffset)), ((minYOffset + maxYOffset)), 0);

                RaycastHit hit;
                if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out hit, weaponRange, ~layermask3))
                {
                    if(hit.transform.CompareTag("Enemy"))
                    {
                        DealDamage(hit.transform.root.gameObject, false);
                        hitDistance = Vector3.Distance(shootPoint.transform.position, hit.point);
                        Instantiate(hitEnemyVFX, hit.point, Quaternion.Inverse(transform.rotation), hit.transform);
                    }
                    if (hit.transform.CompareTag("WeakSpot"))
                    {
                        DealDamage(hit.transform.root.gameObject, true);
                        hitDistance = Vector3.Distance(shootPoint.transform.position, hit.point);
                        Instantiate(hitEnemyVFX, hit.point, Quaternion.Inverse(transform.rotation), hit.transform);
                    }

                    else if(hit.transform.CompareTag("Breakable"))
                    {
                        DealDamage(hit.transform.gameObject, false);
                        hitDistance = Vector3.Distance(shootPoint.transform.position, hit.point);
                        Instantiate(hitVFX, hit.point, Quaternion.Inverse(transform.rotation));
                    }
                    else
                        Instantiate(hitVFX, hit.point, Quaternion.Inverse(transform.rotation));


                    //if (hit.transform.root.CompareTag("Enemy"))
                    //    Instantiate(hitEnemyVFX, hit.point, Quaternion.Inverse(transform.rotation), hit.transform);
                    //else
                    //    Instantiate(hitVFX, hit.point, Quaternion.Inverse(transform.rotation));
                }

                shootPoint.transform.localRotation = Quaternion.Euler(0, 0, 0);

            }
        }

        #endregion

        MantleWeapon();
    }


    public void DealDamage(GameObject incomingObj, bool incomingWeakSpotShot)
    {
        if (incomingObj.GetComponent<IDamageInterface>() != null)
        {
            //hitMarkLogic.EnableHitMarker();
            //if (hitSFX != null)
            //    Instantiate(hitSFX.GetComponent<AudioSource>());

            aMainSystem.DealDamage(incomingObj, damage, true, incomingWeakSpotShot);


            //incomingObj.GetComponent<IDamageInterface>().Damage(damage);
        }
    }
}
