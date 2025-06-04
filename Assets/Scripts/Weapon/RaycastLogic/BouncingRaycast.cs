using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingRaycast : ShootRaycast
{
    [Header("BouncingRaycast")]
    [Tooltip("1 point is equal to 1% chance to crit")]
    public bool onWeakSpot = true;
    public int maxTargets = 1;
    public int chanceToRefract = 50;
    public int refractRange = 50;

    public LineRenderer bouncingLineRender;

    public override void OnWeakSpotHit(GameObject incomingEnemy, bool enemyWeakSpot, Vector3 hitPos)
    {
        if(onWeakSpot)
        {
            int randomNumb = Random.Range(0, 100);
            if(chanceToRefract >= randomNumb)
            {
                
                FindEnemies(incomingEnemy, hitPos);
            }
        }
    }

    public override void OnHit(GameObject incomingEnemy, bool enemyWeakSpot, Vector3 hitPos)
    {
        if (onWeakSpot) { return; }

        int randomNumb = Random.Range(0, 100);
        if (chanceToRefract >= randomNumb)
        {
            FindEnemies(incomingEnemy, hitPos);
        }
    }

    public void FindEnemies(GameObject incomingEnemy, Vector3 hitPos)
    {

        GameObject[] enemies = RemoveEnemy(Physics.OverlapSphere(incomingEnemy.transform.position, refractRange, 1 << 9), incomingEnemy);
        
        CheckLineOfSight(enemies, incomingEnemy, hitPos);
    }

    public GameObject[] RemoveEnemy(Collider[] enemies, GameObject enemy)
    {
        List<GameObject> enemyList = new List<GameObject>();

        foreach (Collider collider in enemies)
        {
            enemyList.Add(collider.transform.root.gameObject);
        }

        GameObject enemyRoot = enemy.transform.root.gameObject;
        if (enemyList.Contains(enemyRoot))
        {
            enemyList.Remove(enemyRoot); 
        }

        return enemyList.ToArray();
    }

    public void CheckLineOfSight(GameObject[] enemies, GameObject enemy, Vector3 hitPos)
    {
        

        List<GameObject> alreadyAddedEnemies = new List<GameObject>();

        if (enemies.Length == 0) { return; }

        for (int i = 0; i < enemies.Length; i++)
        {

            if (enemies[i].gameObject == enemy) { continue; }

            GameObject enemyParent = enemies[i].transform.root.gameObject;
            if (alreadyAddedEnemies.Contains(enemyParent))
            {
                continue;
            }

            alreadyAddedEnemies.Add(enemyParent);
            //Vector3 startVector = enemy.GetComponentInParent<EnemyBase>().homingTarget.transform.position;
            Vector3 startVector = hitPos;
            Vector3 direction = CalculateDirection(startVector, enemies[i]);

            SpawnLinerenderer(startVector, enemies[i].GetComponentInParent<EnemyBase>().homingTarget.transform.position, true);
            DealBouncyDamage(enemies[i].transform.gameObject);

        }
    }

    public void DealBouncyDamage(GameObject incomingObj)
    {
        if (incomingObj.GetComponent<IDamageInterface>() != null)
        {
            aMainSystem.DealDamage(incomingObj, damage, true);
        }
    }

    private Vector3 CalculateDirection(Vector3 start, GameObject enemy)
    {
        return (enemy.GetComponentInParent<EnemyBase>().homingTarget.transform.position - start).normalized;
    }

}
