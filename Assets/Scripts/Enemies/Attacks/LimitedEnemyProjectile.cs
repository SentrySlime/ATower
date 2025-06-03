using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitedEnemyProjectile : EnemyProjectile
{

    [Header("LimitedProjectile")]
    public float durationUntilStop = 1;

    private void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        base.Start();
        StartCoroutine(StopAfterTime());
    }
    
    void Update()
    {
        base.Update();
    }


    private IEnumerator StopAfterTime()
    {
        yield return new WaitForSeconds(durationUntilStop);

        projectileSpeed = 0;

    }

}
