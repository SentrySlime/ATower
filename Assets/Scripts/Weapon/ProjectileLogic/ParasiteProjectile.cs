using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ParasiteProjectile : ProjectileBase, IProjectileRelease
{   
    public GameObject enemy;
    public EnemyBase enemyBase;

    bool canImpact = true;

    float damageInterval = 1;
    float damageIntervalTimer = 0;

    float baseSpeed;

    public override void Start()
    {
        base.Start();
        baseSpeed = projectileSpeed;
    }

    public override void ImpactBehaviour(GameObject hitEnemy, bool incomingWeakSpotShot, Vector3 hitPos)
    {
        if (!canImpact) return;


        base.ImpactBehaviour(hitEnemy, incomingWeakSpotShot, hitPos);
        enemy = hitEnemy;

        projectileSpeed = 0;

        ParasiteBehaviour(hitPos);
    }

    public override void CallOnUpdate()
    {
        base.CallOnUpdate();

        if (!enemy)
        {
            projectileSpeed = baseSpeed;
            return;
        }
        
        if (damageIntervalTimer < damageInterval)
            damageIntervalTimer += Time.deltaTime;
        else
        {

            DealDamage(enemy, false);
            Instantiate(hitEnemyVFX, transform.position, Quaternion.LookRotation(-transform.forward));
            damageIntervalTimer = 0;
        }
    }

    private void ParasiteBehaviour(Vector3 pos)
    {
        enemyBase = enemy.GetComponent<EnemyBase>();
        transform.position = pos;
        
        transform.parent = enemy.transform;
        enemyBase.SetProjetile(this.gameObject);
    }

    public void Release()
    {
        canImpact = false;
        
        enemyBase = null;
        projectileSpeed = baseSpeed;
        StartCoroutine(ParasiteCooldown());
    }

    IEnumerator ParasiteCooldown()
    {
        yield return new WaitForSeconds(0.3f);
        canImpact = true;
    }
}
