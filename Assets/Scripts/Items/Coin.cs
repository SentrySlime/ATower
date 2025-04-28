using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    PlayerStats playerStats;

    public AudioSource audioSource;
    public AudioClip clip;
    public Inventory inventory;

    bool canTrigger = true;
    float timer = 0.025f;
    float counter = 0.025f;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if(counter < timer)
        {
            canTrigger = false;
            counter += Time.deltaTime;
        }
        else 
            canTrigger = true;
    }

    private void OnParticleCollision(GameObject other)
    {
        GiveMoney();
        if(!canTrigger) { return; }

        if (other.CompareTag("iconParent"))
        {
            audioSource.PlayOneShot(clip);
            counter = 0;
        }
    }

    private void GiveMoney()
    {
        if(playerStats.increasedMoneyDrop)
            inventory.IncreaseMoney(5);
        else
        {
            int coinAmount = Random.Range(1, 3);
            inventory.IncreaseMoney(coinAmount);
        }
    }
}
