using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Curse : MonoBehaviour
{

    GameObject gameManager;
    CurseManager curseManager;
    EnemyManager enemyManager;
    AMainSystem aMainSystem;

    GameObject player;
    PlayerStats playerStats;
    PlayerHealth playerHealth;
    Inventory inventory;
    Coin coin;


    [Header("CursePanel")]
    public GameObject cursePanelObj;
    CursePanel cursePanel;
    public TextMeshProUGUI conditionDescription;
    public TextMeshProUGUI conditionCount;
    public TextMeshProUGUI downSideDescription;
    Transform cursePanelParent;

    [Header("Blessing")]
    public GameObject blessingObj;
    Transform blessingParent;

    [Header("Curse parts")]
    public CurseCondition curseCondition;
    public CurseDownside curseDownside;
    public CurseReward curseReward;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        curseManager = gameManager.GetComponent<CurseManager>();
        enemyManager = gameManager.GetComponent<EnemyManager>();
        aMainSystem = gameManager.GetComponent<AMainSystem>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        playerHealth = player.GetComponent<PlayerHealth>();
        inventory = player.GetComponent<Inventory>();

        cursePanelParent = GameObject.Find("CurseParent").transform;
        blessingParent = GameObject.Find("BlessingParent").transform;

        cursePanel = Instantiate(cursePanelObj, cursePanelParent).GetComponent<CursePanel>();

        conditionDescription = cursePanel.conditionDescription;
        conditionCount = cursePanel.conditionCount;
        downSideDescription = cursePanel.downSideDescription;
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
        string rewardDescription = curseReward.ReturnDescription();
        TextMeshProUGUI blessing = Instantiate(blessingObj, blessingParent).GetComponent<TextMeshProUGUI>();
        blessing.text = rewardDescription;

        cursePanel.conditionCount.color = Color.green;

        curseDownside.RemoveCurseDownSide();
        curseReward.Reward();

        StartCoroutine(DestroyCurse());
    }

    IEnumerator DestroyCurse()
    {
        yield return new WaitForSeconds(1);

        cursePanel.FadeOut();
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
        curseCondition.inventory = inventory;
        curseCondition.aMainSystem = aMainSystem;
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
