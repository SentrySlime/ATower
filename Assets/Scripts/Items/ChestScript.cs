using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestScript : MonoBehaviour, IInteractInterface
{
    public Transform spawnPos;
    public GameObject coins;
    public Collider collider;
    public AudioSource chestOpenSFX;

    bool open = false;

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
    }

    void Update()
    {
        
    }

    IEnumerator SpawnWeapon()
    {
        animation["Armature|open"].speed = 5f;
        animation.Play();
        yield return new WaitForSeconds(0.25f);
        collider.enabled = false;
        if(coins)
            Instantiate(coins, spawnPos.position, Quaternion.identity).GetComponent<ParticleSystem>().trigger.AddCollider(player.GetComponent<Collider>());
        yield return new WaitForSeconds(0.1f);
        lootSystem.DropWeapon(spawnPos.position);
    }

    public void Interact()
    {
        if(open) { return; }
        open = true;
        if(chestOpenSFX != null)
            chestOpenSFX.Play();

        StartCoroutine(SpawnWeapon());
    }
}
