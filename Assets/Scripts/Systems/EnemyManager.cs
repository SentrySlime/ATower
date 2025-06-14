using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public int totalEnemyDeaths = 0;

    int enemyDeaths = 0;
    float countDown = 0;
    float slowCounter = 0;
    bool slowDown = false;
    float slowDuration = 0.03f;

    [Header("Ammo Drop")]
    public GameObject ammoPrefab;
    int ammoKillRequirement = 0;
    int minAmmoSpawnChance = 0;
    int maxAmmoSpawnChance = 10;
    int ammoSpawnChance = 0;

    [HideInInspector] public event System.Action<int> enemyDeathReport;

    [Header("HelpingHandStats")]
    public GameObject helpingHandPrefab;
    int helpingHandKillRequirement = 0;
    int helpingHandSpawnChance = 3;

    GameObject player;
    Inventory inventory;
    PlayerStats playerStats;
    PlayerHealth playerHealth;
    HealthRegen healthRegen;
    WeaponSocket weaponSocket;
    AMainSystem mainSystem;
    ShootSystem shootSystem;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        playerStats = player.GetComponent<PlayerStats>();
        playerHealth = player.GetComponent<PlayerHealth>();
        healthRegen = player.GetComponent<HealthRegen>();
        weaponSocket = player.GetComponent<WeaponSocket>();
        mainSystem = GetComponent<AMainSystem>();
        shootSystem = GetComponent<ShootSystem>();
    }

    
    void Update()
    {
        if(enemyDeaths > 0)
            DeathsCountdown();
    }

    private void LateUpdate()
    {
        if(slowDown)
            SlowDown();
        
    }

    public void ReportDeath(Vector3 deathPosition, bool canDropAmmo, bool elite, float overKillDamage)
    {
        totalEnemyDeaths++;

        if(elite)
            enemyDeathReport?.Invoke(5);
        else
            enemyDeathReport?.Invoke(1);

        PlayerStatsEffects(deathPosition);
        if(canDropAmmo)
            CheckForAmmoDrop(deathPosition);
        if(elite)
        {
            if(playerStats.hpOnEliteKill > 0)
                playerHealth.Heal(playerStats.hpOnEliteKill, true);
        }
        //inventory.IncreaseMoney(moneyToDrop);

        if (overKillDamage > 0)
        {
            float healingAmount = overKillDamage * (playerStats.overkillDamageHeal / 100f);
            float maxAllowedHeal = playerHealth.maxHP * 0.1f;
            healingAmount = Mathf.Min(healingAmount, maxAllowedHeal);

            playerHealth.Heal(healingAmount, false);
        }

        if (playerStats.crimsonDagger > 0)
            shootSystem.CrimsonDagger(deathPosition);


        if (!slowDown)
        {
            enemyDeaths++;
            if (enemyDeaths >= 5)
                ActiveSlowDown();
        }



    }

    private void CheckForAmmoDrop(Vector3 deathPosition)
    {

        ammoKillRequirement++;

        if (ammoKillRequirement > 3)
        {
            int randomNumb = Random.Range(1, 100);

            if (randomNumb <= ammoSpawnChance)
            {
                Instantiate(ammoPrefab, deathPosition, Quaternion.identity);
                ammoKillRequirement = 0;
                ammoSpawnChance = minAmmoSpawnChance;
            }
            else
            {


                if(ammoSpawnChance < maxAmmoSpawnChance)
                {
                    ammoSpawnChance++;
                    ammoKillRequirement = 0;
                }
            }
        }
    }

    public void ActiveSlowDown()
    {
        enemyDeaths = 0;
        slowCounter = 0;
        slowDown = true;
    }

    private void SlowDown()
    {
        Time.timeScale = 0.1f;
        if (slowCounter < slowDuration)
        {

            slowCounter += Time.deltaTime;
        }
        else
        {
            slowDown = false;
            Time.timeScale = 1f;
            slowCounter = 0;
        }

    }

    private void DeathsCountdown()
    {
        if(countDown < 0.1)
        {
            countDown += Time.deltaTime;
        }
        else
        {
            enemyDeaths--;
            countDown = 0;
        }
    }

    private void PlayerStatsEffects(Vector3 deathPosition)
    {
        if(playerStats.hpOnKill > 0)
            playerHealth.Heal(playerStats.hpOnKill, false);

        healthRegen.StartHPRegen();
        HelpingHandLogic(deathPosition);
        StartCoroutine(WaitBefore());
    }

    private void HelpingHandLogic(Vector3 deathPosition)
    {
        helpingHandKillRequirement++;

        if (playerStats.helpingHand > 0 && helpingHandKillRequirement > 2)
        {
            int randomNumb = Random.Range(1, 10);

            if (randomNumb <= helpingHandSpawnChance)
            {
                HelpingHandPickUp tempHelpingHand = Instantiate(helpingHandPrefab, deathPosition, Quaternion.identity).GetComponent<HelpingHandPickUp>();
                tempHelpingHand.playerHealth = playerHealth;
                tempHelpingHand.mainSystem = mainSystem;
                helpingHandKillRequirement = 0;
                helpingHandSpawnChance = 2;
            }
            else
            {
                helpingHandSpawnChance++;
            }

        }

    }

    IEnumerator WaitBefore()
    {
        yield return new WaitForEndOfFrame();

        if (weaponSocket.equippedWeapon != null)
        {
            if (Random.Range(0, 4) == 0)
            {
                weaponSocket.equippedWeapon.GiveAmmoToMagazine(playerStats.returnAmmoOnkill);
            }
        }
    }

}
