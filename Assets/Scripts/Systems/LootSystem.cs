using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootSystem : MonoBehaviour
{
    public WeaponManager weaponManager;
    public ItemManager itemManager;

    public int itemTokens = 0;
    public int tokenRequirement = 100;

    void Start()
    {
        tokenRequirement = Random.Range(85, 100);
        int tempReduction = Random.Range(8, 15);
        itemTokens = tokenRequirement - tempReduction;
    }


    public void DropLoot(Vector3 spawnPos, int enemyToken)
    {

        if (!DropCheck(enemyToken))
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

    public bool DropCheck(int incomingToken)
    {
        itemTokens += incomingToken;
        if (itemTokens >= tokenRequirement)
        {
            itemTokens = 0;
            return true;
        }
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

    public ItemBase GetItemForShop()
    {
        return itemManager.GetItemForShop();
    }

    public GameObject GetWeaponForShop()
    {
        return  weaponManager.GetWeaponForShop();
    }

}
