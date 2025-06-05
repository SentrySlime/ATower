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
        roomScript = GetComponentInParent<RoomScript>();
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        spawnedEnemy = Instantiate(enemyToSpawn, transform.position, transform.rotation);
        roomScript.AddEnemy(spawnedEnemy);
        spawnedEnemy.GetComponent<EnemyBase>().roomScript = roomScript;
    }
}
