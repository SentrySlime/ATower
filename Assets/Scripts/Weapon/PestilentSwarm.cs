using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PestilentSwarm : MonoBehaviour
{
    
    AMainSystem mainSystem;
    EnemyBase enemyBase;

    List<GameObject> swarms = new List<GameObject>();

    public GameObject effects;

    int maxSwarmCount = 15;
    int currentSwarmCount = 0;

    private void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        mainSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();
    }

    public void AttachSwarm(GameObject swarm)
    {
        swarms.Add(swarm);
        currentSwarmCount++;
        if(currentSwarmCount >= maxSwarmCount)
        {
            Instantiate(effects, swarms[0].transform.position, Quaternion.identity, gameObject.transform);
           
            mainSystem.DealDamage(gameObject, 150, true, enemyBase: enemyBase, canTriggerChainLightning: true);
            currentSwarmCount = 0;
            for(int i = 0; i < swarms.Count; i++)
            {
                Destroy(swarms[i]);
            }

            swarms.Clear();
        }
    }
}