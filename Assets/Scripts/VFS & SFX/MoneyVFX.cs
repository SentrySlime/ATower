using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyVFX : MonoBehaviour
{
    public Inventory inventory;

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnParticleTrigger()
    {
        inventory.IncreaseMoney(5);
    }
}
