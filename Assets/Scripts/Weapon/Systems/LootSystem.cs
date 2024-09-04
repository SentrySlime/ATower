using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSystem : MonoBehaviour
{
    public WeaponManager weaponManager;
    public ItemManager itemManager;

    void Start()
    {
        
    }

    void Update()
    {
        //numberOfRoles++;
        //float number = Random.Range(0f, 1f);

        //print(number + " " + numberOfRoles);

        //if (number <= 0.001)
        //{
        //    Debug.LogError("Found the number " + number);
        //}
    }

    public bool DropCheck(float dropChance)
    {
        float number = Random.Range(0f, 1f);

        if (number <= dropChance)
            return true;
        else
            return false;
    }

    public void DropLoot(Vector3 spawnPos, float dropChance)
    {
        if (!DropCheck(dropChance))
            return;


        int randomChance = Random.Range(0, 2);
     
        if (randomChance == 1)
        {
            weaponManager.DropWeapon(spawnPos);
        }
        else
        {
            itemManager.DropItem(spawnPos);
        }

    }

    //public GameObject DropWeapon()
    //{
    //    return weaponManager.GetRandomWeapon();
    //}

    //public GameObject DropItem()
    //{
    //    return itemManager.GetRandomItem();
    //}

}
