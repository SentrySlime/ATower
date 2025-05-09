using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyBag : Item, IInteractInterface
{
    public GameObject FX;
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.Rotate(Vector3.up, 100 * Time.deltaTime);
    }
    public void Interact()
    {
        AddMoney();
    }

    private void AddMoney()
    {
        if(FX)
            Instantiate(FX, transform.position, Quaternion.identity);
        
        player.GetComponent<Inventory>().IncreaseMoney(750);
        Destroy(gameObject);
    }
}
