using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterProjectile : ProjectileBase
{
    public float fireRate = 0.3f;
    public float fireRateTimer = 0f;

    [SerializeField] private GameObject shootPoint;

    public override void CallOnUpdate()
    {
        base.CallOnUpdate();

        if(fireRateTimer < fireRate)
        {
            fireRateTimer += Time.deltaTime;
        }
        else
        {
            FireHomingProjectile();
            fireRateTimer = 0f;
        }
        
    }

    private void FireHomingProjectile()
    {
        shootSystem.FireHomingRocket(false, shootPoint, 360, 100);
    }

}
