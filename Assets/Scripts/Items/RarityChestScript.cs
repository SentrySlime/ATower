using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RarityChestScript : MonoBehaviour, IInteractInterface
{
    public GameObject weaponSpawnPos;
    public GameObject goldSpawnPos;
    public AudioSource audioSource;

    [Header("Misc")]
    public float chestOpenSpeed = 3;
    public GameObject coins;

    [Header("Common")]
    public GameObject commonChest;
    public Animation commonAnimation;
    public ParticleSystem commonPS;
    public AudioClip commonSFX;

    [Header("Rare")]
    public GameObject rareChest;
    public Animation rareAnimation;
    public ParticleSystem rarePS;
    public AudioClip rareSFX;

    [Header("Epic")]
    public GameObject epicChest;
    public Animation epicAnimation;
    public ParticleSystem epicPS;
    public AudioClip epicSFX;
    public GameObject epicLightningVFX;
    public ParticleSystem epicLightningImpactVFX;

    [Header("Legendary")]
    public GameObject legendaryChest;
    public Animation legendaryAnimation;
    public ParticleSystem legendaryPS;
    public AudioClip legendarySFX;
    public ParticleSystem legendaryOpenVFX;
    public GameObject legendaryBeams;

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

    private void OnEnable()
    {
        if (rarity == LootSystem.Rarity.common)
        {
            commonPS.Play();
            commonChest.SetActive(true);
            commonAnimation["Plane.007|AS_CommonChestOpen"].speed = chestOpenSpeed;
        }
        else if (rarity == LootSystem.Rarity.rare)
        {
            rarePS.Play();
            rareChest.SetActive(true);
            rareAnimation["Plane.001|AS_RareChestOpen"].speed = chestOpenSpeed;
        }
        else if (rarity == LootSystem.Rarity.epic)
        {
            epicPS.Play();
            epicChest.SetActive(true);
            epicAnimation["Sphere|AS_EpicChestOpen"].speed = chestOpenSpeed;
        }
        else if (rarity == LootSystem.Rarity.legendary)
        {
            legendaryPS.Play();
            legendaryChest.SetActive(true);
            legendaryAnimation["Sphere.001|AS_LegendaryChestOpen"].speed = chestOpenSpeed;
        }
    }

    public void Interact()
    {
        if(open) {return;}

        transform.tag = "Untagged";
        open = true;

        if (rarity == LootSystem.Rarity.common)
        {
            commonAnimation.Play();
            audioSource.clip = commonSFX;
        }
        else if (rarity == LootSystem.Rarity.rare)
        {
            rareAnimation.Play();
            audioSource.clip = rareSFX;
        }
        else if (rarity == LootSystem.Rarity.epic)
        {
            epicAnimation.Play();
            audioSource.clip = epicSFX;
            StartCoroutine(LightningStrike());
        }
        else if (rarity == LootSystem.Rarity.legendary)
        {
            StartCoroutine(FadeInBlessedAura());
            legendaryAnimation.Play();
            audioSource.clip = legendarySFX;
        }

        audioSource.Play();

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
            commonPS.Play();
            commonAnimation["Plane.007|AS_CommonChestOpen"].speed = chestOpenSpeed;
        }
        else if (rarity == LootSystem.Rarity.rare)
        {
            rareChest.SetActive(true);
            rarePS.Play();
            rareAnimation["Plane.001|AS_RareChestOpen"].speed = chestOpenSpeed;
        }
        else if (rarity == LootSystem.Rarity.epic)
        {
            epicChest.SetActive(true);
            epicPS.Play();
            epicAnimation["Sphere|AS_EpicChestOpen"].speed = chestOpenSpeed;
        }
        else if (rarity == LootSystem.Rarity.legendary)
        {
            legendaryChest.SetActive(true);
            legendaryPS.Play();
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

    IEnumerator LightningStrike()
    {
        epicLightningVFX.SetActive(true);
        epicLightningImpactVFX.Play();
        yield return new WaitForSeconds(0.3f);
        epicLightningVFX.SetActive(false);
    }

    IEnumerator FadeInBlessedAura()
    {
        legendaryBeams.SetActive(true);

        float duration = 0.5f;
        float elapsed = 0f;

        ParticleSystem.MainModule main = legendaryOpenVFX.main;
        //ParticleSystem.MainModule main2 = holyAura2.main;

        // Define the final target color you want to fade in to
        Color targetColor = new Color(1f, 1f, 1f, 1f); // Example: white, fully opaque
        Color startColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f); // Same color, 0 alpha

        // Set the initial transparent color
        main.startColor = startColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Color newColor = Color.Lerp(startColor, targetColor, t);
            main.startColor = newColor;
            //main2.startColor = newColor;

            yield return null;
        }

        main.startColor = targetColor;
        //main2.startColor = targetColor;


        //holyRotate = true;
        //curseManager.SpawnCurse(reward);
    }

}