using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinTriggerScript : MonoBehaviour
{
    public int coins = 0;
    Inventory inventory;
    ParticleSystem VFXCoins;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();

        VFXCoins = GetComponent<ParticleSystem>();
        VFXCoins.trigger.AddCollider(GameObject.FindGameObjectWithTag("VFXCollider").GetComponent<Collider>());
        //VFXCoins.trigger = 
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnParticleTrigger()
    {
        //inventory.IncreaseMoney();
    }

}
