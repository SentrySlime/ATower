using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour, IInteractInterface
{
    public Transform spawnPos;
    public GameObject coins;
    
    GameObject gameManager;
    LootSystem lootSystem;
    Inventory inventory;
    GameObject player;

    [Header("Animation")]
    public Animation animation;

    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        player = GameObject.FindGameObjectWithTag("Player");

        lootSystem = gameManager.GetComponent<LootSystem>();
        
        inventory = player.GetComponent<Inventory>();
        
        //StartCoroutine(SpawnWeapon());
    }

    void Update()
    {
        
    }

    IEnumerator SpawnWeapon()
    {
        animation.Play();
        yield return new WaitForSeconds(1);
        Instantiate(coins, spawnPos.position, Quaternion.identity).GetComponent<ParticleSystem>().trigger.AddCollider(player.GetComponent<Collider>());
        yield return new WaitForSeconds(0.3f);
        //lootSystem.DropWeapon(spawnPos.position);
        //yield return new WaitForSeconds(1);
    }




    public void Interact()
    {
        StartCoroutine(SpawnWeapon());
    }
}
