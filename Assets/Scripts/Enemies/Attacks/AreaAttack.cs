using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAttack : MonoBehaviour
{
    AMainSystem mainSystem;

    public int damage = 3;

    float rate = 0.25f;
    float timer = 0;

    private void Start()
    {
        mainSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();
    }

    private void Update()
    {
        if (timer < rate)
            timer += Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(timer >= rate)
            {
                mainSystem.DealDamage(other.transform.gameObject, 3, false);               
                timer = 0;
            }
        }
    }

}
