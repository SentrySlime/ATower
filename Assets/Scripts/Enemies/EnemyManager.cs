using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    int enemyDeaths = 0;
    float countDown = 0;

    float slowCounter = 0;

    bool slowDown = false;
    public float slowDuration = 1;

    Inventory inventory;

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
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

    public void ReportDeath(int moneyToDrop)
    {
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

}
