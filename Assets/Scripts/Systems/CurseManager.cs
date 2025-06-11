using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseManager : MonoBehaviour
{

    GameObject player;
    PlayerStats playerStats;
    EnemyManager enemyManager;
    public GameObject curseObject;

    public GameObject[] curseCondition;
    public GameObject[] curseDownside;
    public GameObject[] curseReward;

    public GameObject[] damageCurse;
    public GameObject[] utilityCurse;
    public GameObject[] healthCurse;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        enemyManager = GetComponent<EnemyManager>();
    }

    public GameObject[] GetCurseRewards()
    {
        GameObject[] rewards = new GameObject[3];
        rewards[0] = damageCurse[Random.Range(0, damageCurse.Length)];
        rewards[1] = utilityCurse[Random.Range(0, utilityCurse.Length)];
        rewards[2] = healthCurse[Random.Range(0, healthCurse.Length)];
        return rewards;
    }


    public void SpawnCurse(GameObject reward)
    {
        Curse curse = Instantiate(curseObject).GetComponent<Curse>();
        curse.curseCondition = Instantiate(curseCondition[Random.Range(0, curseCondition.Length)]).GetComponent<CurseCondition>();
        curse.curseDownside = Instantiate(curseDownside[Random.Range(0, curseDownside.Length)]).GetComponent<CurseDownside>();
        curse.curseReward = Instantiate(reward).GetComponent<CurseReward>();
        curse.InitializeCurse();
    }

}
