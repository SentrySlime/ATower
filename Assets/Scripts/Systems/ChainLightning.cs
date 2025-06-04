using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{

    [Header("Referemces")]
    AMainSystem mainSystem;
    GameObject player;
    PlayerStats playerStats;

    [Header("Effects")]
    public LineRenderer lineRenderer;
    public AudioSource triggerSFX;
    public GameObject hitEnemyVFX;
    public GameObject hitEnemySFX;

    [Header("Stats")]
    public bool curve = false;
    public int chanceToRefract = 50;
    public int refractRange = 50;

    int hitCount = 0;

    private static readonly List<GameObject> _uniqueEnemies = new List<GameObject>();
    private static readonly List<(GameObject enemy, float distance)> _sortedTargets = new List<(GameObject, float)>();

    void Start()
    {
        mainSystem = GetComponent<AMainSystem>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
    }

    public void DoLightning(GameObject enemy)
    {
        FindEnemies(enemy);
    }

    private void FindEnemies(GameObject incomingEnemy)
    {
        GameObject[] enemies = RemoveEnemy(Physics.OverlapSphere(incomingEnemy.transform.position, refractRange, 1 << 9), incomingEnemy);
        
        if(enemies.Length > 0)
        {
            CheckLineOfSight(enemies, incomingEnemy, incomingEnemy.transform.position);
        }
    }

    private GameObject[] RemoveEnemy(Collider[] enemies, GameObject enemy)
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

    //private void CheckLineOfSight(GameObject[] enemies, GameObject enemy, Vector3 hitPos)
    //{


    //    List<GameObject> alreadyAddedEnemies = new List<GameObject>();

    //    if (enemies.Length == 0) { return; }

    //    for (int i = 0; i < enemies.Length; i++)
    //    {

    //        if (enemies[i].gameObject == enemy) { continue; }

    //        GameObject enemyParent = enemies[i].transform.root.gameObject;
    //        if (alreadyAddedEnemies.Contains(enemyParent))
    //        {
    //            continue;
    //        }

    //        alreadyAddedEnemies.Add(enemyParent);
    //        Vector3 startVector = hitPos;
    //        Vector3 direction = CalculateDirection(startVector, enemies[i]);

    //        if(hitCount < maxTargets)
    //        {
    //            hitCount++;
    //            SpawnLinerenderer(startVector, enemies[i].GetComponentInParent<EnemyBase>().homingTarget.transform.position);
    //            DealBouncyDamage(enemies[i].transform.gameObject);
    //        }
    //    }

    //    hitCount = 0;
    //}

    

    private void CheckLineOfSight(GameObject[] enemies, GameObject sourceEnemy, Vector3 hitPos)
    {
        if (enemies.Length == 0) return;

        _uniqueEnemies.Clear();
        _sortedTargets.Clear();

        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject current = enemies[i];
            if (current == sourceEnemy) continue;

            GameObject root = current.transform.root.gameObject;
            if (_uniqueEnemies.Contains(root)) continue;

            _uniqueEnemies.Add(root);

            float dist = (hitPos - current.transform.position).sqrMagnitude; // Avoid expensive sqrt
            _sortedTargets.Add((current, dist));
        }

        // Sort using sqrMagnitude (cheaper than Vector3.Distance)
        _sortedTargets.Sort((a, b) => a.distance.CompareTo(b.distance));

        int hits = 0;
        for (int i = 0; i < _sortedTargets.Count && hits < playerStats.chainLightningTargets; i++)
        {
            GameObject enemy = _sortedTargets[i].enemy;

            var enemyBase = enemy.GetComponentInParent<EnemyBase>();
            if (enemyBase == null || enemyBase.homingTarget == null) continue;

            Vector3 targetPos = enemyBase.homingTarget.transform.position;

            triggerSFX.Play();

            SpawnLinerenderer(hitPos, targetPos);
            DealBouncyDamage(enemy);

            hits++;
        }
    }



    private void DealBouncyDamage(GameObject incomingObj)
    {
        if (incomingObj.GetComponent<IDamageInterface>() != null)
        {
            mainSystem.DealDamage(incomingObj, playerStats.chainLightningDamage, true, canTriggerChainLightning: true);
        }
    }

    private void SpawnLinerenderer(Vector3 start, Vector3 end)
    {
        if (!lineRenderer) { return; }
        LineRenderer tempLineRender = Instantiate(lineRenderer, Vector3.zero, Quaternion.identity).GetComponent<LineRenderer>();
        tempLineRender.enabled = true;

        if (curve)
        {
            int pointCount = 20;
            tempLineRender.positionCount = pointCount;
            float distance = Vector3.Distance(start, end);

            float curveIntensity = Mathf.Clamp(distance * 0.3f, 5f, 50f);

            Vector3 controlPoint = (start + end) * 0.5f + (Random.onUnitSphere + Vector3.up).normalized * curveIntensity;


            if(hitEnemyVFX)
                Instantiate(hitEnemyVFX, end, Quaternion.Inverse(transform.rotation));

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

            if (hitEnemyVFX)
                Instantiate(hitEnemyVFX, end, Quaternion.Inverse(transform.rotation));

        }
    }

    private Vector3 GetQuadraticBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return Mathf.Pow(1 - t, 2) * p0 +
                2 * (1 - t) * t * p1 +
                Mathf.Pow(t, 2) * p2;
    }

    private Vector3 CalculateDirection(Vector3 start, GameObject enemy)
    {
        return (enemy.GetComponentInParent<EnemyBase>().homingTarget.transform.position - start).normalized;
    }
}
