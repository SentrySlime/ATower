using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour
{
    public Transform spawnPos;

    GameObject gameManager;
    LootSystem lootSystem;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        lootSystem = gameManager.GetComponent<LootSystem>();
        StartCoroutine(SpawnWeapon());
    }

    void Update()
    {
        
    }

    IEnumerator SpawnWeapon()
    {
        yield return new WaitForSeconds(1);
        lootSystem.DropWeapon(spawnPos.position);
    }
}
