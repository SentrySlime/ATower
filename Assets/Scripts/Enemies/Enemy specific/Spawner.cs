using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    [SerializeField] private GameObject spawnling;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem spawnPS1;
    [SerializeField] private ParticleSystem spawnPS2;

    private int minionCount = 0;

    void Start()
    {
        //Invoke("InitateSpawn", 1);
        InvokeRepeating("InitateSpawn", 1, 3);
    }

    
    void Update()
    {
        
    }

    private void InitateSpawn()
    {
        if(minionCount > 5)  { return; }

        animator.SetBool("Spawn", true);
        Invoke("Spawn", 1f);
    }

    private void Spawn()
    {
        spawnPS1.Play();
        spawnPS2.Play();
        GameObject tempSpawnling = Instantiate(spawnling, spawnPosition.position, transform.rotation);
        tempSpawnling.GetComponent<EnemyBase>().spawner = this;
        minionCount++;
        animator.SetBool("Spawn", false);
    }

    public void DecreaseMinionCount ()
    {
        minionCount--;
    }
}
