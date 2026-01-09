using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectileFromHitscan : ShootProjectile
{

    public override void Awake()
    {
        base.Awake();
    }

    public override void PlayEffects()
    {
       base.PlayEffects();
    }

    public override void FireProjectile()
    {
        float tempADSAccuracy = ADSAccuracy + playerStats.accuracy;
        float tempHIPAccuracy = HIPAccuracy + playerStats.accuracy;

        Vector3 shootDirection = Vector3.zero;
        Debug.DrawLine(shootPoint.transform.position, shootPoint.transform.forward * 999, Color.red, .2f);
        RaycastHit hit;
        if (Physics.Raycast(raycastShootPoint.transform.position, raycastShootPoint.transform.forward, out hit, 9999, ~layerMask))
        {
            for (int i = 0; i < shootAmount; i++)
            {
                float accuracy = (weaponSocket.adsProgress < 0.9f)
                    ? tempADSAccuracy
                    : tempHIPAccuracy;

                // Random horizontal offsets
                float xOffset = Random.Range(-accuracy, accuracy);
                float zOffset = Random.Range(-accuracy, accuracy);

                Vector3 tempShootPoint = new Vector3(
                    hit.point.x + xOffset,
                    hit.point.y + Random.Range(20, 25),
                    hit.point.z + zOffset
                );

                var proj = Instantiate(
                    projectile,
                    tempShootPoint,
                    Quaternion.LookRotation(Vector3.down)
                );

                proj.GetComponent<ProjectileBase>().weaponParent = baseWeapon;
            }

        }


        //#region RandomNumbers Accuracy
        //if (weaponSocket.adsProgress < 0.9)
        //{
        //    minYOffset = Random.Range(-tempADSAccuracy, 0);
        //    maxYOffset = Random.Range(tempADSAccuracy, 0);

        //    minXoffset = Random.Range(-tempADSAccuracy, 0);
        //    maxXoffset = Random.Range(tempADSAccuracy, 0);
        //}
        //else
        //{
        //    minYOffset = Random.Range(-tempHIPAccuracy, 0);
        //    maxYOffset = Random.Range(tempHIPAccuracy, 0);

        //    minXoffset = Random.Range(-tempHIPAccuracy, 0);
        //    maxXoffset = Random.Range(tempHIPAccuracy, 0);
        //}

        //#endregion

        //shootPoint.transform.Rotate(((minXoffset + maxXoffset)), ((minYOffset + maxYOffset)), 0);
        //Quaternion rotation = Quaternion.LookRotation(shootPoint.transform.forward, Vector3.up);

        //Instantiate(projectile, shootPoint.transform.position, rotation).GetComponent<ProjectileBase>().weaponParent = baseWeapon;
        //shootPoint.transform.localRotation = Quaternion.Euler(0, 0, 0);

    }
}
