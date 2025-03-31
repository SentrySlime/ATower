using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    int enemyDeaths = 0;
    float countDown = 0;
    float slowCounter = 0;
    bool slowDown = false;
    float slowDuration = 0.03f;

    public GameObject helpingHandPrefab;


    GameObject player;
    Inventory inventory;
    PlayerStats playerStats;
    PlayerHealth playerHealth;
    WeaponSocket weaponSocket;
    AMainSystem mainSystem;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        inventory = player.GetComponent<Inventory>();
        playerStats = player.GetComponent<PlayerStats>();
        playerHealth = player.GetComponent<PlayerHealth>();
        weaponSocket = player.GetComponent<WeaponSocket>();
        mainSystem = GetComponent<AMainSystem>();
    }

    
    void Update()
    {
        //if (Input.GetKey(KeyCode.U))
        //    slowDown = true;

        if(enemyDeaths > 0)
            DeathsCountdown();


    }

    private void LateUpdate()
    {
        if(slowDown)
            SlowDown();
        
    }

    public void ReportDeath(int moneyToDrop, Vector3 deathPosition)
    {
        PlayerStatsEffects(deathPosition);
        inventory.IncreaseMoney(moneyToDrop);

        if (!slowDown)
        {
            enemyDeaths++;
            if (enemyDeaths >= 5)
                ActiveSlowDown();
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
            playerHealth.Heal(playerStats.hpOnKill);

        if (playerStats.helpingHand > 0)
        {
            int randomNumb = Random.Range(1, 10);
            print(randomNumb);
            if (randomNumb <= 3)
            {
                HelpingHandPickUp tempHelpingHand = Instantiate(helpingHandPrefab, deathPosition, Quaternion.identity).GetComponent<HelpingHandPickUp>();
                tempHelpingHand.playerHealth = playerHealth;
                tempHelpingHand.mainSystem = mainSystem;
            }
            
        }

        StartCoroutine(WaitBefore());
    }

    IEnumerator WaitBefore()
    {
        yield return new WaitForEndOfFrame();

        weaponSocket.equippedWeapon.GiveAmmoToMagazine(playerStats.returnAmmoOnkill);
    }

}
