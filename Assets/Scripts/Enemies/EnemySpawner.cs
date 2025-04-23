using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyToSpawn;
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
        GameObject tempEnemy = Instantiate(enemyToSpawn, transform.position, transform.rotation);
        roomScript.AddEnemy(tempEnemy);
    }

}
