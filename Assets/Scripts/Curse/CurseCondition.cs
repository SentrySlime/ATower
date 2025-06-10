using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CurseCondition : MonoBehaviour
{
    [HideInInspector] public GameObject gameManager;
    [HideInInspector] public CurseManager curseManager;
    [HideInInspector] public EnemyManager enemyManager;
    [HideInInspector] public AMainSystem aMainSystem;
    [HideInInspector] public Inventory inventory;
    [HideInInspector] public Curse curse;
    [HideInInspector] public bool complete;

    public string description;

    public abstract void ConditionCheck(int value);

    public void ReturnComplete()
    {
        curse.ConditionMet();
    }
}
