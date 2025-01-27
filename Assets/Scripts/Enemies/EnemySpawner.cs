using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemyToSpawn;
    
    void Start()
    {
        Instantiate(enemyToSpawn, transform.position, transform.rotation);
    }

    
    void Update()
    {
        
    }
}
