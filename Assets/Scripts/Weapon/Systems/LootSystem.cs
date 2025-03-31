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

    }


    public void DropLoot(Vector3 spawnPos, float dropChance)
    {
        if (!DropCheck(dropChance))
            return;

        int weaponOrItem = Random.Range(0, 2);
     
            itemManager.DropItem(spawnPos);
        //if (weaponOrItem == 1)
        //{
        //    weaponManager.DropWeapon(spawnPos);
        //}
        //else
        //{
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

    public void DropWeapon(Vector3 pos)
    {
        weaponManager.DropWeapon(pos);
    }

    public void DropItem(Vector3 pos)
    {
        itemManager.DropItem(pos);
    }

}
