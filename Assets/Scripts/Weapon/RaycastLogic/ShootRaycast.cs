using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class ShootRaycast : BaseShootingLogic
{
    HitmarkerLogic hitMarkLogic;

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;
    Coroutine lineRenderCoroutine;
    public bool curve = false;
    public float lineDuration = 0.9f;
    public float lineTimer;

    public float hitDistance = 0;

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

    //---
   
    private void Awake()
    {
        if(lineRenderer)
            lineRenderer.enabled = false;

        //This layermask sends a single raycast and should basically only hit terrain
        layermask = LayerMask.GetMask("Player", "Enemy", "Projectile", "Items", "Breakable", "Ignore Raycast");
        
        //This actually only tries to get the enemies in the way
        layermask2 = LayerMask.GetMask("Enemy", "WeaponLayer", "Breakable");
        
        //This layermask is only for shotguns
        layermask3 = LayerMask.GetMask("Player", "Projectile", "Items", "Ignore Raycast");


    }

    public void Start()
    {

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
        if(effectPosition && shootEffect)
        {
            Instantiate(shootEffect, effectPosition.position, transform.rotation);
        }

        float tempADSAccuracy = ADSAccuracy + playerStats.accuracy;
        float tempHIPAccuracy = HIPAccuracy + playerStats.accuracy;

        #region Shooting
        if (!isShotgun)
        {

            

            //This is where we se the random numbers for the accuracy
            #region RandomNumbers Accuracy
            if (weaponSocket.adsProgress < 0.9)
            {
                minYOffset = Random.Range(-tempADSAccuracy, 0);
                maxYOffset = Random.Range(tempADSAccuracy, 0);

                minXoffset = Random.Range(-tempADSAccuracy, 0);
                maxXoffset = Random.Range(tempADSAccuracy, 0);
            }
            else
            {
                minYOffset = Random.Range(-tempHIPAccuracy, 0);
                maxYOffset = Random.Range(tempHIPAccuracy, 0);

                minXoffset = Random.Range(-tempHIPAccuracy, 0);
                maxXoffset = Random.Range(tempHIPAccuracy, 0);
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
                    explosionSystem.SpawnExplosion(raycastHit.point, explosiveRadius, explosiveDamage, weaponParent: baseWeapon);
                }
                else
                {

                    //raycastHit.normal
                    // Create a rotation that faces away from the surface, using only the surface normal
                    Quaternion hitRotation = Quaternion.LookRotation(raycastHit.normal);

                    // Instantiate the VFX at the hit point, facing away from the surface
                    Instantiate(hitVFX, raycastHit.point, hitRotation);

                }
            }


            if(enemiesToDamage == null || enemiesToDamage.Count == 0)
            {

                if (lineRenderer)
                    SpawnLinerenderer(effectPosition.position, raycastHit.point, false);
            }


            for (int i = 0; i < tempPierce; i++)
            {
                if (enemiesToDamage[i].transform.CompareTag("Enemy"))
                {
                    if(ShouldExplode)
                    {
                        explosionSystem.SpawnExplosion(hits[i].point, explosiveRadius, explosiveDamage, alreadyDamaged[i], weaponParent: baseWeapon);
                    }
                    else
                    {
                        var fwd = Vector3.ProjectOnPlane(transform.forward, raycastHit.normal);
                        var tempRot = Quaternion.LookRotation(fwd, raycastHit.normal);
                        Vector3 hitDirection = transform.position - raycastHit.point;

                        GameObject tempVFX = Instantiate(hitEnemyVFX, hits[i].point, Quaternion.LookRotation(hitDirection));

                        StartCoroutine(AttachEffectNextFrame(tempVFX, alreadyDamaged[i].transform));
                        
                        if (lineRenderer)
                            SpawnLinerenderer(effectPosition.position, hits[i].point, true);
                        DealDamage(alreadyDamaged[i], false, hits[i].point);
                    }

                }
                else if (enemiesToDamage[i].transform.CompareTag("WeakSpot"))
                {
                    if (ShouldExplode)
                    {
                        explosionSystem.SpawnExplosion(hits[i].point, explosiveRadius, explosiveDamage, alreadyDamaged[i], weaponParent: baseWeapon);
                    }
                    else
                    {
                        var fwd = Vector3.ProjectOnPlane(transform.forward, raycastHit.normal);
                        var tempRot = Quaternion.LookRotation(fwd, raycastHit.normal);
                        Vector3 hitDirection = transform.position - raycastHit.point;
                        
                        GameObject tempVFX = Instantiate(hitEnemyVFX, hits[i].point, Quaternion.LookRotation(hitDirection));

                        StartCoroutine(AttachEffectNextFrame(tempVFX, alreadyDamaged[i].transform));
                        
                        if (lineRenderer)
                            SpawnLinerenderer(effectPosition.position, hits[i].point, true);
                        DealDamage(alreadyDamaged[i], true, hits[i].point);
                    }
                }
                else if(alreadyDamaged[i].transform.CompareTag("Breakable"))
                {
                    if (ShouldExplode)
                    {
                        explosionSystem.SpawnExplosion(hits[i].point, explosiveRadius, explosiveDamage, alreadyDamaged[i], weaponParent: baseWeapon);
                    }
                    else
                    {
                        var fwd = Vector3.ProjectOnPlane(transform.forward, raycastHit.normal);
                        var tempRot = Quaternion.LookRotation(fwd, raycastHit.normal);
                        Vector3 hitDirection = transform.position - raycastHit.point;
                        GameObject tempVFX = Instantiate(hitVFX, hits[i].point, Quaternion.LookRotation(hitDirection));
                        if (lineRenderer)
                            SpawnLinerenderer(effectPosition.position, hits[i].point, false);
                        StartCoroutine(AttachEffectNextFrame(tempVFX, alreadyDamaged[i].transform));
                        DealDamage(alreadyDamaged[i], false, hits[i].point);
                        tempPierce = 0;
                        
                    }
                }
                else if (alreadyDamaged[i].transform.CompareTag("Ground"))
                {
                    // Project the forward direction of the object onto the plane defined by the hit normal
                    Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, raycastHit.normal).normalized;

                    // Create a rotation that looks in the projected forward direction, with the hit normal as the up direction
                    Quaternion tempRot = Quaternion.LookRotation(fwd, raycastHit.normal);

                    // Determine the direction from the hit point back to the object
                    Vector3 hitDirection = (transform.position - raycastHit.point).normalized;

                    // Optionally spawn a line renderer from effect position to hit point
                    if (lineRenderer)
                        SpawnLinerenderer(effectPosition.position, raycastHit.point, false);

                    // Instantiate the VFX with proper orientation using the surface normal
                    Instantiate(hitVFX, raycastHit.point, Quaternion.LookRotation(hitDirection, raycastHit.normal), alreadyDamaged[i].transform);


                    break;
                }

            }

            shootPoint.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            //for (int i = 0; i < shootAmount; i++)
            //{

            //    #region RandomNumbers Accuracy
            //    if (weaponSocket.adsProgress < 0.9)
            //    {
            //        minYOffset = Random.Range(-tempADSAccuracy, 0);
            //        maxYOffset = Random.Range(tempADSAccuracy, 0);

            //        minXoffset = Random.Range(-tempADSAccuracy, 0);
            //        maxXoffset = Random.Range(tempADSAccuracy, 0);
            //    }
            //    else
            //    {
            //        minYOffset = Random.Range(-tempHIPAccuracy, 0);
            //        maxYOffset = Random.Range(tempHIPAccuracy, 0);

            //        minXoffset = Random.Range(-tempHIPAccuracy, 0);
            //        maxXoffset = Random.Range(tempHIPAccuracy, 0);
            //    }

            //    #endregion

            //    shootPoint.transform.Rotate(((minXoffset + maxXoffset)), ((minYOffset + maxYOffset)), 0);

            //    RaycastHit hit;
            //    if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out hit, weaponRange, ~layermask3))
            //    {
            //        if(hit.transform.CompareTag("Enemy"))
            //        {
            //            DealDamage(hit.transform.root.gameObject, false, hit.point);
            //            hitDistance = Vector3.Distance(shootPoint.transform.position, hit.point);

            //            Instantiate(hitEnemyVFX, hit.point, Quaternion.Inverse(transform.rotation), hit.transform);

            //        }
            //        if (hit.transform.CompareTag("WeakSpot"))
            //        {
            //            DealDamage(hit.transform.root.gameObject, true, hit.point);
            //            hitDistance = Vector3.Distance(shootPoint.transform.position, hit.point);

            //            Instantiate(hitEnemyVFX, hit.point, Quaternion.Inverse(transform.rotation), hit.transform);
            //        }

            //        else if(hit.transform.CompareTag("Breakable"))
            //        {
            //            DealDamage(hit.transform.gameObject, false, hit.point);
            //            hitDistance = Vector3.Distance(shootPoint.transform.position, hit.point);
            //            Instantiate(hitVFX, hit.point, Quaternion.Inverse(transform.rotation));
            //        }
            //        else
            //            Instantiate(hitVFX, hit.point, Quaternion.Inverse(transform.rotation));

            //    }

            //    shootPoint.transform.localRotation = Quaternion.Euler(0, 0, 0);

            //}

            for (int i = 0; i < shootAmount; i++)
            {
                // Accuracy offsets
                float xOffset, yOffset;

                if (weaponSocket.adsProgress < 0.9f)
                {
                    yOffset = Random.Range(-tempADSAccuracy, tempADSAccuracy);
                    xOffset = Random.Range(-tempADSAccuracy, tempADSAccuracy);
                }
                else
                {
                    yOffset = Random.Range(-tempHIPAccuracy, tempHIPAccuracy);
                    xOffset = Random.Range(-tempHIPAccuracy, tempHIPAccuracy);
                }

                // Calculate adjusted direction
                Quaternion yaw = Quaternion.AngleAxis(xOffset, Vector3.up); // horizontal spread
                Quaternion pitch = Quaternion.AngleAxis(yOffset, Vector3.left); // vertical spread

                //Vector3 adjustedForward = new Vector3(shootPoint.transform.forward.x, 0, shootPoint.transform.forward.z);

                //Vector3 adjustedDirection = yaw * pitch * adjustedForward;

                Vector3 adjustedDirection = yaw * pitch * shootPoint.transform.forward;

                // Perform raycast
                if (Physics.Raycast(shootPoint.transform.position, adjustedDirection, out RaycastHit hit, weaponRange, ~layermask3))
                {
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        DealDamage(hit.transform.root.gameObject, false, hit.point);
                        hitDistance = Vector3.Distance(shootPoint.transform.position, hit.point);
                        Instantiate(hitEnemyVFX, hit.point, Quaternion.Inverse(transform.rotation), hit.transform);
                    }
                    else if (hit.transform.CompareTag("WeakSpot"))
                    {
                        DealDamage(hit.transform.root.gameObject, true, hit.point);
                        hitDistance = Vector3.Distance(shootPoint.transform.position, hit.point);
                        Instantiate(hitEnemyVFX, hit.point, Quaternion.Inverse(transform.rotation), hit.transform);
                    }
                    else if (hit.transform.CompareTag("Breakable"))
                    {
                        DealDamage(hit.transform.gameObject, false, hit.point);
                        hitDistance = Vector3.Distance(shootPoint.transform.position, hit.point);
                        Instantiate(hitVFX, hit.point, Quaternion.Inverse(transform.rotation));
                    }
                    else
                    {
                        Instantiate(hitVFX, hit.point, Quaternion.Inverse(transform.rotation));
                    }
                }
            }

        }

        #endregion

    }


    public void DealDamage(GameObject incomingObj, bool incomingWeakSpotShot, Vector3 hitPos)
    {
        if (incomingObj.GetComponent<IDamageInterface>() != null)
        {
            aMainSystem.DealDamage(incomingObj, damage, true, incomingWeakSpotShot);
            
            if(incomingWeakSpotShot)
            {
                OnHit(incomingObj, incomingWeakSpotShot, hitPos);
                OnWeakSpotHit(incomingObj, incomingWeakSpotShot, hitPos);
            }
            else
            {
                OnHit(incomingObj, incomingWeakSpotShot, hitPos);
            }
        }
    }

    public virtual void OnHit(GameObject incomingEnemy, bool enemyWeakSpot, Vector3 hitPos)
    {

    }

    public virtual void OnWeakSpotHit(GameObject incomingEnemy, bool enemyWeakSpot, Vector3 hitPos)
    {
        
    }


    public void SpawnLinerenderer(Vector3 start, Vector3 end, bool enemy)
    {
        if (!lineRenderer) { return; }
        LineRenderer tempLineRender = Instantiate(lineRenderer, Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
        tempLineRender.enabled = true;

        if(curve)
        {
            int pointCount = 20;
            tempLineRender.positionCount = pointCount;
            float distance = Vector3.Distance(start, end);

            float curveIntensity = Mathf.Clamp(distance * 0.3f, 5f, 50f);

            Vector3 controlPoint = (start + end) * 0.5f + (Random.onUnitSphere + Vector3.up).normalized * curveIntensity;
            
            if(enemy)
                Instantiate(hitEnemyVFX, end, Quaternion.Inverse(transform.rotation));
            else
                Instantiate(hitVFX, end, Quaternion.Inverse(transform.rotation));

            for (int i = 0; i < pointCount; i++)
            {
                float t = i / (float)(pointCount - 1);
                Vector3 pointOnCurve = GetQuadraticBezierPoint(start, controlPoint, end, t);
                tempLineRender.SetPosition(i, pointOnCurve);
            }
        }
        else
        {
            tempLineRender.SetPosition(0, start);
            tempLineRender.SetPosition(1, end);
            if (enemy)
                Instantiate(hitEnemyVFX, end, Quaternion.Inverse(transform.rotation));
            else
                Instantiate(hitVFX, end, Quaternion.Inverse(transform.rotation));
        }

    }

    

    IEnumerator AttachEffectNextFrame(GameObject effect, Transform parent)
    {
        yield return null; // wait one frame
        if (effect && parent) effect.transform.SetParent(parent, true);
    }

    private Vector3 GetQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return Mathf.Pow(1 - t, 2) * p0 +
               2 * (1 - t) * t * p1 +
               Mathf.Pow(t, 2) * p2;
    }


}
