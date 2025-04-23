using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSystem : MonoBehaviour
{
    public WeaponManager weaponManager;
    public ItemManager itemManager;

    public int itemKillRequirement = 0;

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

        itemManager.DropItem(spawnPos);
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

    public ItemPickUp DropDevilItem(Vector3 pos)
    {
        return itemManager.DropDevilItem(pos);
    }

}
