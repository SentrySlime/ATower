using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RarityChestScript : MonoBehaviour, IInteractInterface
{

    public GameObject spawnPos;

    [Header("Common")]
    public GameObject commonChest;
    public Animation commonAnimation;

    [Header("Common")]
    public GameObject rareChest;
    public Animation rareAnimation;

    [Header("Common")]
    public GameObject epicChest;
    public Animation epicAnimation;

    [Header("Common")]
    public GameObject legendaryChest;
    public Animation legendaryAnimation;

    bool open = false;

    GameObject gameManager;
    LootSystem lootSystem;
    Inventory inventory;
    GameObject player;

    public LootSystem.Rarity rarity;

    void Start()
    {
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
            commonAnimation.Play();
        else if (rarity == LootSystem.Rarity.rare)
            rareAnimation.Play();
        else if (rarity == LootSystem.Rarity.epic)
            epicAnimation.Play();
        else if (rarity == LootSystem.Rarity.legendary)
            legendaryAnimation.Play();


        StartCoroutine(SpawnWeapon());
    }

    IEnumerator SpawnWeapon()
    {
        yield return new WaitForSeconds(1.967f);
        lootSystem.DropWeapon(spawnPos.transform.position, rarity);
    }

    public void EnableCorrectChest()
    {
        if(rarity == LootSystem.Rarity.common)
            commonChest.SetActive(true);
        else if (rarity == LootSystem.Rarity.rare)
            rareChest.SetActive(true);
        else if (rarity == LootSystem.Rarity.epic)
            epicChest.SetActive(true);
        else if (rarity == LootSystem.Rarity.legendary)
            legendaryChest.SetActive(true);
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