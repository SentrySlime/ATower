using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour, IStatusEffect
{

    [SerializeField] private GameObject spawnling;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem spawnPS1;
    [SerializeField] private ParticleSystem spawnPS2;

    [HideInInspector] public bool frozen;
    [HideInInspector] public bool burning;

    float moveSpeed = 0;
    float animatorSpeed = 0;

    private List<GameObject> spawnList = new List<GameObject>();

    EnemyBase enemyBase;

    private int minionCount = 0;

    void Start()
    {
        //Invoke("InitateSpawn", 1);
        InvokeRepeating("InitateSpawn", 1, 3);
        enemyBase = GetComponent<EnemyBase>();
    }

    
    void Update()
    {
        
    }

    private void InitateSpawn()
    {
        if(minionCount > 4)  { return; }

        animator.SetBool("Spawn", true);
        Invoke("Spawn", 1f);
    }

    private void Spawn()
    {
        spawnPS1.Play();
        spawnPS2.Play();
        GameObject tempSpawnling = Instantiate(spawnling, spawnPosition.position, transform.rotation);
        spawnList.Add(tempSpawnling);
        enemyBase.roomScript.AddEnemy(tempSpawnling);
        tempSpawnling.GetComponent<EnemyBase>().spawner = this;
        minionCount++;
        animator.SetBool("Spawn", false);
    }

    public void DisableSpawnlings()
    {
        for (int i = 0; i < spawnList.Count; i++)
        {
            if (spawnList[i])
                spawnList[i].SetActive(false);
        }
    }

    public void EnableSpawnlings()
    {
        for (int i = 0; i < spawnList.Count; i++)
        {
            if (spawnList[i])
                spawnList[i].SetActive(true);
        }
    }

    public void DecreaseMinionCount ()
    {
        minionCount--;
    }

    public void Freeze()
    {
        if (animator)
        {
            animatorSpeed = animator.speed;
            animator.speed = 0;
        }

        frozen = true;
    }

    public bool IsFrozen()
    {
        return frozen;
    }

    public void UnFreeze()
    {
        if (animator)
            animator.speed = animatorSpeed;
        frozen = false;
    }

    public void Burn()
    {
        throw new System.NotImplementedException();
    }
}
