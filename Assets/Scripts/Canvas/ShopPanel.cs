using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    ItemPanel itemPanel;
    GameObject gameManager;
    LootSystem lootSystem;
    GameObject player;
    Inventory inventory;
    FindAndEquipWeapons findAndEquipWeapons;
    
    
    public GameObject itemPanelPrefab;
    public Transform itemParent;
    public ItemBase selectedItem;
    public GameObject selectedIcon;
    public MeshFilter renderTextureMeshFilter;
    public MeshRenderer renderTextureMeshRenderer;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Image image;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        findAndEquipWeapons = player.GetComponent<FindAndEquipWeapons>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        lootSystem = gameManager.GetComponent<LootSystem>();
    }

    public void PopulateShop()
    {
        for (int i = 0; i < 5; i++)
        {
            AddItemsToShop();
        }
    }

    IEnumerator WaitForSec()
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < 5; i++)
        {
            AddItemsToShop();
        }
    }

    public void SetFirstItemDisplay()
    {
        ItemBase tempItem = itemParent.GetChild(0).GetComponent<ItemPanel>().itemBase;
        SetTheShowCase(tempItem);

    }

    public void BuyItem()
    {
        if(!selectedItem) return;

        if(inventory.money >= selectedItem.goldCost)
        {
            inventory.money -= selectedItem.goldCost;
            findAndEquipWeapons.EquipItemBase(selectedItem);
            NullTheShowCase();
            Destroy(selectedIcon);
        }
        else
        {
            print("Not enough money");
        }
    }

    public void NullTheShowCase()
    {
        selectedItem = null;
        itemName.text = " ";
        itemDescription.text = " ";
        image.sprite = null;
        renderTextureMeshFilter.mesh = null;
    }

    private void SetTheShowCase(ItemBase tempItem)
    {
        selectedItem = tempItem;
        itemName.text = tempItem.itemName;
        itemDescription.text = tempItem.itemDescription;
        image.sprite = tempItem.itemIcon;


        renderTextureMeshFilter.mesh = tempItem.itemMesh;
        renderTextureMeshRenderer.material = tempItem.itemMaterial;
    }

    private void AddItemsToShop()
    {
        itemPanel = Instantiate(itemPanelPrefab, itemParent).GetComponent<ItemPanel>();
        ItemBase tempItem = lootSystem.GetItemForShop();
        itemPanel.shopPanel = this;
        itemPanel.SetPanelFromItemBase(tempItem);

        itemPanel.itemName = itemName;
        itemPanel.itemDescription = itemDescription;
        itemPanel.image = image;
    }
}
