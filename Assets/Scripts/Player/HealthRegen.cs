using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthRegen : MonoBehaviour
{
    PlayerHealth playerHealth;
    PlayerStats playerStats;

    float hpRegen;

    float hpRegenCooldown = 0;
    float hpRegenTimer = 0;

    //--
    bool isRegen = false;
    float hpRegenDuration = 4;

    float regenElapsed = 0f;
    float lastHealTime = 0f;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
        playerStats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        UpdateHPRegen();
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            StartHPRegen();
        }

        Regen();
    }

    public void UpdateHPRegen()
    {
        if (playerStats != null)
            hpRegenCooldown = 1 / playerStats.hpRegen;
    }

    private void Regen()
    {
        if (isRegen)
        {
            regenElapsed += Time.deltaTime;

            while (regenElapsed - lastHealTime >= hpRegenCooldown)
            {
                playerHealth.Heal(1);
                lastHealTime += hpRegenCooldown;
            }

            if (regenElapsed >= hpRegenDuration)
            {
                EndHpRegen();
            }
        }
    }


    public void StartHPRegen()
    {
        if (playerStats == null || playerStats.hpRegen <= 0f) return;

        if (!isRegen)
        {
            isRegen = true;
            regenElapsed = 0f;
            lastHealTime = 0f;
            hpRegenDuration = 4;
        }
        else
        {
            hpRegenDuration += 4;
        }
    }



    public void EndHpRegen()
    {
        isRegen = false;
        regenElapsed = 0f;
        lastHealTime = 0f;
        hpRegenDuration = 4;
    }
    
}
