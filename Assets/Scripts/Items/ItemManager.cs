using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemManager : MonoBehaviour
{
    public GameObject itemTemplate;

    public List<ItemBase> items;

    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        items = Resources.LoadAll<ItemBase>("Prefabs/Items/Item_Prefabs").ToList();
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
        ItemPickUp tempItem = Instantiate(itemTemplate, spawnPos, Quaternion.identity).GetComponent<ItemPickUp>();
        ItemBase item = GetRandomItem();
        items.Remove(item);
        tempItem.itemPrefab = item;
        
    }


    private ItemBase GetRandomItem()
    {
        int itemIndex = Random.Range(0, items.Count);
        return items[itemIndex];
    }
}
