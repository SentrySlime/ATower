using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    public GameObject itemTemplate;
    public GameObject itemSFX;
    public GameObject devilItemSFX;

    public List<ItemBase> items;
    public List<ItemBase> devilItems;

    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        items = Resources.LoadAll<ItemBase>("Prefabs/Items/Item_Prefabs").ToList();
        devilItems = Resources.LoadAll<ItemBase>("Prefabs/Items/Devil_Item_Prefabs").ToList();
    }

    
    void Update()
    {
        if(Input.GetKey(KeyCode.I))
        {
            if (items == null || items.Count == 0)
            {
                return;
            }
            DropItem(player.transform.position);
        }
    }

    public void DropItem(Vector3 spawnPos)
    {
        if(items == null || items.Count == 0) { return; }

        ItemPickUp tempItem = Instantiate(itemTemplate, spawnPos, Quaternion.identity).GetComponent<ItemPickUp>();
        
        if(itemSFX)
            Instantiate(itemSFX, spawnPos, Quaternion.identity);
        ItemBase item = GetRandomItem();
        items.Remove(item);
        tempItem.itemPrefab = item;
    }

    public ItemPickUp DropDevilItem(Vector3 spawnPos)
    {
        if (devilItems == null || devilItems.Count == 0) { return null; }

        if (devilItemSFX)
            Instantiate(devilItemSFX, spawnPos, Quaternion.identity);
        ItemPickUp tempItem = Instantiate(itemTemplate, spawnPos, Quaternion.identity).GetComponent<ItemPickUp>();
        ItemBase item = GetRandomDevilItem();
        devilItems.Remove(item);
        tempItem.itemPrefab = item;
        return tempItem;
    }

    private ItemBase GetRandomItem()
    {
        int itemIndex = Random.Range(0, items.Count);
        return items[itemIndex];
    }

    private ItemBase GetRandomDevilItem()
    {
        int itemIndex = Random.Range(0, devilItems.Count);
        return devilItems[itemIndex];
    }

    private void CalculateNextItemType()
    {

    }
}
