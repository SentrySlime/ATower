using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clip;
    public Inventory inventory;

    bool canTrigger = true;
    float timer = 0.05f;
    float counter = 0.05f;

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
        inventory.IncreaseMoney(5);
    }
}
