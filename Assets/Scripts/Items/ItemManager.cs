using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{

    public GameObject itemTemplate;
    public List<ItemBase> items = new List<ItemBase>();

    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O))
        {
            print("ItemManager");
            DropItem(player.transform.position);
        }
    }

    public void DropItem(Vector3 spawnPos)
    {
        ItemPickUp tempItem = Instantiate(itemTemplate, spawnPos, Quaternion.identity).GetComponent<ItemPickUp>();
        tempItem.itemPrefab = GetRandomItem();

    }


    private ItemBase GetRandomItem()
    {
        int itemIndex = Random.Range(0, items.Count);
        return items[itemIndex];
    }
}
