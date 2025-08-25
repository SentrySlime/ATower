using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RarityChestScript : MonoBehaviour, IInteractInterface
{
    public GameObject weaponSpawnPos;
    public GameObject goldSpawnPos;

    [Header("Misc")]
    public float chestOpenSpeed = 3;
    public GameObject coins;

    [Header("Common")]
    public GameObject commonChest;
    public Animation commonAnimation;
    public ParticleSystem commonPS;

    [Header("Rare")]
    public GameObject rareChest;
    public Animation rareAnimation;
    public ParticleSystem rarePS;

    [Header("Epic")]
    public GameObject epicChest;
    public Animation epicAnimation;
    public ParticleSystem epicPS;

    [Header("Legendary")]
    public GameObject legendaryChest;
    public Animation legendaryAnimation;
    public ParticleSystem legendaryPS;

    bool open = false;

    GameObject gameManager;
    LootSystem lootSystem;
    Inventory inventory;
    GameObject player;

    [Header("Rarity")]
    public bool randomRarity = true;
    public LootSystem.Rarity rarity;

    void Start()
    {
        if(randomRarity)
            RandomizeChest();

        EnableCorrectChest();

        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        player = GameObject.FindGameObjectWithTag("Player");
        lootSystem = gameManager.GetComponent<LootSystem>();
        inventory = player.GetComponent<Inventory>();
    }

    
    public void Interact()
    {
        if(open) {return;}

        transform.tag = "Untagged";
        open = true;

        if (rarity == LootSystem.Rarity.common)
        {
            commonAnimation.Play();
            commonPS.Play();
        }
        else if (rarity == LootSystem.Rarity.rare)
        {
            rareAnimation.Play();
            rarePS.Play();
        }
        else if (rarity == LootSystem.Rarity.epic)
        {
            epicAnimation.Play();
            epicPS.Play();
        }
        else if (rarity == LootSystem.Rarity.legendary)
        {
            legendaryAnimation.Play();
            legendaryPS.Play();
        }


        StartCoroutine(SpawnWeapon());
    }

    IEnumerator SpawnWeapon()
    {
        yield return new WaitForSeconds(1.967f / chestOpenSpeed);
        lootSystem.DropWeapon(weaponSpawnPos.transform.position, rarity);
        if (coins)
            Instantiate(coins, goldSpawnPos.transform.position, Quaternion.identity).GetComponent<ParticleSystem>().trigger.AddCollider(player.GetComponent<Collider>());
    }

    public void EnableCorrectChest()
    {
        if(rarity == LootSystem.Rarity.common)
        {
            commonChest.SetActive(true);
            commonAnimation["Plane.007|AS_CommonChestOpen"].speed = chestOpenSpeed;
        }
        else if (rarity == LootSystem.Rarity.rare)
        {
            rareChest.SetActive(true);
            rareAnimation["Plane.001|AS_RareChestOpen"].speed = chestOpenSpeed;
        }
        else if (rarity == LootSystem.Rarity.epic)
        {
            epicChest.SetActive(true);
            epicAnimation["Sphere|AS_EpicChestOpen"].speed = chestOpenSpeed;
        }
        else if (rarity == LootSystem.Rarity.legendary)
        {
            legendaryChest.SetActive(true);
            legendaryAnimation["Sphere.001|AS_LegendaryChestOpen"].speed = chestOpenSpeed;
        }
    }

    public void RandomizeChest()
    {
        float rarityChance = Random.Range(0, 100);

        if(rarityChance <= 35)
            rarity = LootSystem.Rarity.common;
        if (rarityChance <= 75 && rarityChance > 35)
            rarity = LootSystem.Rarity.rare;
        if (rarityChance <= 95 && rarityChance > 75)
            rarity = LootSystem.Rarity.epic;
        if (rarityChance <= 100 && rarityChance > 90)
            rarity = LootSystem.Rarity.legendary;

    }
}