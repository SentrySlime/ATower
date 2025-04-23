using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyToSpawn;
    [SerializeField] GameObject spawnedEnemy;
    RoomScript roomScript;
    
    void Start()
    {
        StartCoroutine(SpawnEnemy());
        roomScript = GetComponentInParent<RoomScript>();
    }

    
    void Update()
    {
        
    }

    IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(0.8f);
        spawnedEnemy = Instantiate(enemyToSpawn, transform.position, transform.rotation);
        roomScript.AddEnemy(spawnedEnemy);
    }

}
