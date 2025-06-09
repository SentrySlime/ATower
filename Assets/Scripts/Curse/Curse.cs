using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curse : MonoBehaviour
{

    GameObject gameManager;
    CurseManager curseManager;
    EnemyManager enemyManager;

    GameObject player;
    PlayerStats playerStats;
    PlayerHealth playerHealth;
    
    public CurseCondition curseCondition;
    public CurseDownside curseDownside;
    public CurseReward curseReward;

    private void Awake()
    {


        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        curseManager = gameManager.GetComponent<CurseManager>();
        enemyManager = gameManager.GetComponent<EnemyManager>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        playerHealth = player.GetComponent<PlayerHealth>();
        //InitializeCurse();
    }

    private void Start()
    {
        
    }


    public void InitializeCurse()
    {
        SetCurseCondition();
        SetCurseDownside();
        SetCurseReward();

        curseDownside.ApplyCurseDownSide();
    }


    public void ConditionMet()
    {
        curseDownside.RemoveCurseDownSide();
        curseReward.Reward();
        
        DestroyCurse();
    }

    private void DestroyCurse()
    {
        Destroy(curseCondition.gameObject);
        Destroy(curseDownside.gameObject);
        Destroy(curseReward.gameObject);
        Destroy(gameObject);
    }

    private void SetCurseCondition()
    {
        curseCondition.curse = this;
        curseCondition.gameManager = gameManager;
        curseCondition.curseManager = curseManager;
        curseCondition.enemyManager = enemyManager;
    }
    private void SetCurseDownside()
    {
        curseDownside.curse = this;
        curseDownside.player = player;
        curseDownside.playerStats = playerStats;
    }
    private void SetCurseReward()
    {
        curseReward.curse = this;
        curseReward.player = player;
        curseReward.playerStats = playerStats;
    }
}
