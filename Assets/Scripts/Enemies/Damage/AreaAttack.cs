using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    PlayerHealth playerHealth;

    public int damage = 3;

    float rate = 0.25f;
    float timer = 0;

    private void Update()
    {
        if (timer < rate)
            timer += Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(playerHealth == null)
                playerHealth = other.GetComponent<PlayerHealth>();

            if(timer >= rate)
            {
                playerHealth.Damage(3);
                timer = 0;
            }
        }
    }

}
