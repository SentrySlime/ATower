using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseManager : MonoBehaviour
{

    GameObject player;
    PlayerStats playerStats;
    EnemyManager enemyManager;
    public GameObject curseObject;

    public GameObject curseCondition;
    public GameObject curseDownside;
    public GameObject curseReward;


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();

        enemyManager = GetComponent<EnemyManager>();

        SpawnCurse();
    }

    
    public void SpawnCurse()
    {
        Curse curse = Instantiate(curseObject).GetComponent<Curse>();
        curse.curseCondition = Instantiate(curseCondition).GetComponent<CurseCondition>();
        curse.curseDownside = Instantiate(curseDownside).GetComponent<CurseDownside>();
        curse.curseReward = Instantiate(curseReward).GetComponent<CurseReward>();
        curse.InitializeCurse();
    }



}
