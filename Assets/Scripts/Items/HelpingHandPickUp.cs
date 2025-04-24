using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpingHandPickUp : MonoBehaviour
{
    public float healAmount = 10;
    [HideInInspector] public PlayerHealth playerHealth;
    [HideInInspector] public AMainSystem mainSystem;
    bool triggered = false;

    private void Update()
    {
        transform.Rotate(Vector3.up, 100 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !triggered)
        {
            if (playerHealth.HasFullHP())
                return;

            mainSystem.SpawnPickUpEffects(transform.position);
            
            triggered = true;
            playerHealth.Heal(healAmount, false);
            Destroy(gameObject);
        }
    }
}
