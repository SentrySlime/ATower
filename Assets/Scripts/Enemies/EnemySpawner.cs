using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyToSpawn;
    
    void Start()
    {
        StartCoroutine(SpawnEnemy());
    }

    
    void Update()
    {
        
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(0.8f);
        Instantiate(enemyToSpawn, transform.position, transform.rotation);
    }

}
