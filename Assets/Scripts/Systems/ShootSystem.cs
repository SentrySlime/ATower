using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootSystem : MonoBehaviour
{
    [Header("Fireball")]
    public GameObject projectile;
    public float accuracy = 18;

    [Header("CrimsonDagger")]
    public GameObject crimsonDagger;
    public GameObject crimsonDaggerSFX;
    public float accuracyDagger = 90;

    [Header("PestilentSwarm")]
    public GameObject pestilentSwarm;

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

    public void CrimsonDagger(Vector3 shootPoint)
    {
        StartCoroutine(DoRadialAttack2(shootPoint));
    }


    IEnumerator DoRadialAttack2(Vector3 shootPoint)
    {
        Instantiate(crimsonDaggerSFX, shootPoint, Quaternion.identity);

        int repeatCount = 3;
        float fireRate = 0.1f;
        int projectileCount = 3;

        float thetaOffsetMultiplier = Mathf.PI * 1.3f;

        for (int round = 0; round < repeatCount; round++)
        {
            Quaternion roundOffset = Quaternion.Euler(0, round * 10f, 0);
            float thetaOffset = round * thetaOffsetMultiplier;

            for (int i = 0; i < projectileCount; i++)
            {
                float t = (i + 0.5f) / projectileCount;


                float biasedT = Mathf.Pow(t, 0.15f);
                float phi = biasedT * (Mathf.PI / 2f);

                float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * i + thetaOffset;

                float x = Mathf.Sin(phi) * Mathf.Cos(theta);
                float z = Mathf.Sin(phi) * Mathf.Sin(theta);
                float y = Mathf.Cos(phi);

                Vector3 localDirection = new Vector3(x, y, z).normalized;
                Vector3 finalDirection = roundOffset * localDirection;

                Quaternion lookRotation = Quaternion.LookRotation(finalDirection);
                Instantiate(crimsonDagger, shootPoint, lookRotation);
            }

            yield return new WaitForSeconds(fireRate);
        }
    }

    public void PestilentSwarm(RaycastHit raycastHit, Vector3 hitDirection, Transform enemy)
    {
        GameObject swarm = Instantiate(pestilentSwarm, raycastHit.point, Quaternion.LookRotation(hitDirection), enemy);
        enemy.GetComponent<PestilentSwarm>().AttachSwarm(swarm);
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

