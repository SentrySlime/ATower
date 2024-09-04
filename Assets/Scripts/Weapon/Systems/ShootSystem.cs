using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootSystem : MonoBehaviour
{
    public GameObject projectile;
    public float accuracy = 18;

    float minYOffset;
    float maxYOffset;

    float minXoffset;
    float maxXoffset;


    public void FireHomingRocket(bool isShotgun, GameObject shootPoint, float accuracy, int rocketChance)
    {
        if (!RandomChance(rocketChance)) { return; }


        #region Shooting


        if (!isShotgun)
        {

            #region RandomNumbers Accuracy

            minYOffset = Random.Range(-accuracy, 0);
            maxYOffset = Random.Range(accuracy, 0);

            minXoffset = Random.Range(-accuracy, 0);
            //maxXoffset = Random.Range(accuracy, 0);

            #endregion

            shootPoint.transform.Rotate(((minXoffset + maxXoffset)), ((minYOffset + maxYOffset)), 0);


            Quaternion rotation = Quaternion.LookRotation(shootPoint.transform.forward, Vector3.up);

            //Rigidbody shotProjectile = Instantiate(projectile.GetComponent<Rigidbody>(), shootPoint.transform.position, rotation);
            Instantiate(projectile, shootPoint.transform.position, rotation);

            //shotProjectile.AddForce(shootPoint.transform.forward * shotSpeed, ForceMode.Impulse);
        }
        else
        {
            //Rigidbody shotProjectile = Instantiate(projectile.GetComponent<Rigidbody>(), shootPoint.transform.position, Quaternion.identity);
            Instantiate(projectile, shootPoint.transform.position, Quaternion.identity);
            //shotProjectile.AddForce(shootPoint.transform.forward * shotSpeed, ForceMode.Impulse);
        }

        shootPoint.transform.localRotation = Quaternion.Euler(0, 0, 0);

        #endregion
    }

    //IncomingChance determines your luck
    //If incoming chance is 32 you have a 32% success rate
    private bool RandomChance(int incomingChance)
    {
        int ignoreChance = Random.Range(0, 100);

        if (incomingChance > ignoreChance)
        {
            return true;

        }
        else
            return false;
    }
}

