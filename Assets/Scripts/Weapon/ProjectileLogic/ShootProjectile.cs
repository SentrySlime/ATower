using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShootProjectile : BaseShootingLogic
{

    [Header("Weapon attributes")]
    public GameObject projectile;
    public float shotSpeed = 1;
    public int shootAmount = 1;
    public float shotSize = 1;
    public int pierceAmount = 1;

    public bool isShotgun = false;

    public bool UseHorizontal = true;
    public bool UseVertical = true;

    float minYOffset = 0;
    float maxYOffset = 0;

    float minXoffset = 0;
    float maxXoffset = 0;

    List<AudioSource> SFXList = new List<AudioSource>();
    int soundCount;

    GameObject raycastShootPoint;
    LayerMask layerMask;
    public GameObject shootSFX;
    public ParticleSystem muzzleFlash;

    public ParticleSystem casingVFX;
    public Animation mantleAnimation;


    private void Awake()
    {
        layerMask = LayerMask.GetMask("Player", "Projectile");
        raycastShootPoint = GameObject.FindGameObjectWithTag("ShootPoint");
    }


    public override void TriggerItem()
    {

        #region NotShooting
        //Show casing jump out
        if (casingVFX != null)
            casingVFX.Play();

        if (aimCamera == null || screenShake == null) { return; }

        screenShake.RecoilFire();

        //Play sound effect
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

        //Show muffle flash effect
        if (muzzleFlash)
            muzzleFlash.Play();

        #endregion





        #region Shooting

        float tempADSAccuracy = ADSAccuracy + playerStats.accuracy;
        float tempHIPAccuracy = HIPAccuracy + playerStats.accuracy;

        if (!isShotgun)
        {

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
            Quaternion rotation = Quaternion.LookRotation(shootPoint.transform.forward, Vector3.up);

            Instantiate(projectile, shootPoint.transform.position, rotation).GetComponent<ProjectileBase>().weaponParent = baseWeapon;
            shootPoint.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {

            bool raycastHit = false;
            Vector3 shootDirection = Vector3.zero;
            Debug.DrawLine(shootPoint.transform.position, shootPoint.transform.forward * 999, Color.red, .2f);
            RaycastHit hit;
            if (Physics.Raycast(raycastShootPoint.transform.position, raycastShootPoint.transform.forward, out hit, 9999, ~layerMask))
            {
                //raycastHit = true;
                //shootDirection = hit.point - shootPoint.transform.position;
                //shootPoint.transform.LookAt(shootDirection);
            }
            for (int i = 0; i < shootAmount; i++)
            {
                

                #region RandomNumbers Accuracy
                if (weaponSocket.adsProgress < 0.9)
                {
                    if (UseHorizontal)
                    {
                        minYOffset = Random.Range(-tempADSAccuracy, 0);
                        maxYOffset = Random.Range(tempADSAccuracy, 0);
                    }

                    if (UseVertical)
                    {
                        minXoffset = Random.Range(-tempADSAccuracy, 0);
                        maxXoffset = Random.Range(tempADSAccuracy, 0);
                    }

                }
                else
                {
                    if (UseHorizontal)
                    {
                        minYOffset = Random.Range(-tempHIPAccuracy, 0);
                        maxYOffset = Random.Range(tempHIPAccuracy, 0);
                    }

                    if (UseVertical)
                    {
                        minXoffset = Random.Range(-tempHIPAccuracy, 0);
                        maxXoffset = Random.Range(tempHIPAccuracy, 0);
                    }

                }

                #endregion




                shootPoint.transform.Rotate(((minXoffset + maxXoffset)), ((minYOffset + maxYOffset)), 0);

                //----
                Instantiate(projectile, shootPoint.transform.position, shootPoint.transform.rotation).GetComponent<ProjectileBase>().weaponParent = baseWeapon;


                //shotProjectile.AddForce(shotProjectile.transform.forward * shotSpeed, ForceMode.Impulse);

                //if(raycastHit)
                //{
                //    shootPoint.transform.LookAt(shootDirection);
                //}
                //else
                //{

                //}
                shootPoint.transform.localRotation = Quaternion.Euler(0, 0, 0);


                //shootPoint.transform.localRotation = Quaternion.Euler(0, 0, 0);

            }

        }



        #endregion

    }

}
