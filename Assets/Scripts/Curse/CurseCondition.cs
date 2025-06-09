using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CurseCondition : MonoBehaviour
{
    [HideInInspector] public GameObject gameManager;
    [HideInInspector] public CurseManager curseManager;
    [HideInInspector] public EnemyManager enemyManager;
    [HideInInspector] public Curse curse;
    [HideInInspector] public bool complete;


    public abstract void ConditionCheck();

    public void ReturnComplete()
    {
        curse.ConditionMet();
    }
}
