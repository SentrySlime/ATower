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


    public GameObject selectedObject;
    [Header("Weapon related")]
    public GameObject weaponPanelPrefab;
    public Transform weaponParent;


    [Header("Item related")]
    public GameObject itemPanelPrefab;
    public Transform itemParent;

    [Header("Misc")]
    public GameObject selectedIcon;
    public MeshFilter renderTextureMeshFilter;
    public MeshRenderer renderTextureMeshRenderer;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Image image;

    public AudioSource purchaseSFX;
    public AudioSource noMoneySFX;

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

        for (int i = 0; i < 2; i++)
        {
            AddWeaponsToShop();
        }
    }

    IEnumerator WaitForSec()
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < 5; i++)
        {
            AddWeaponsToShop();
            AddItemsToShop();

        }
    }

    public void SetFirstItemDisplay()
    {

        if (itemParent.childCount == 0)
        {
            SetFirstWeaponDisplay();
            return;
        }

        ItemPanel tempItem = itemParent.GetChild(0).GetComponent<ItemPanel>();
        tempItem.SetItemDisplay();

    }

    public void SetFirstWeaponDisplay()
    {
        if (weaponParent.childCount == 0)
        {
            NullTheShowCase();
            return;
        }

        ItemPanel tempItem = weaponParent.GetChild(0).GetComponent<ItemPanel>();
        tempItem.SetItemDisplay();
    }

    public void BuyObject()
    {
        if (!selectedObject) return;

        ItemPanel panel = selectedObject.GetComponent<ItemPanel>();

        ItemBase itemBase = panel.itemBase;
        WeaponPickUp weaponPickUp = panel.weaponPickUp;
        if(itemBase != null)
        {
            BuyItem(itemBase);
        }
        else if(weaponPickUp != null)
        {
            BuyWeapon(weaponPickUp);
        }
        
    }


    private void BuyWeapon(WeaponPickUp selectedWeapon)
    {
        BaseWeapon tempWeapon = selectedWeapon.weaponPrefab.GetComponentInChildren<BaseWeapon>();

        if (inventory.money >= selectedWeapon.goldCost)
        {
            purchaseSFX.Play();
            inventory.DecreaseMoney(tempWeapon.goldCost);
            findAndEquipWeapons.InitializeWeapon(selectedWeapon);
            selectedIcon.transform.SetParent(null);
            selectedIcon.SetActive(true);
            Destroy(selectedIcon);
            SetFirstItemDisplay();
        }
        else
        {
            noMoneySFX.Play();
        }
    }

    private void BuyItem(ItemBase selectedItem)
    {
        if (inventory.money >= selectedItem.goldCost)
        {
            purchaseSFX.Play();
            inventory.DecreaseMoney(selectedItem.goldCost);
            findAndEquipWeapons.EquipItemBase(selectedItem);
            selectedIcon.transform.SetParent(null);
            Destroy(selectedIcon);
            SetFirstItemDisplay();
        }
        else
        {
            noMoneySFX.Play();
        }
    }




    public void NullTheShowCase()
    {
        selectedObject = null;
        itemName.text = " ";
        itemDescription.text = " ";
        image.sprite = null;
        renderTextureMeshFilter.mesh = null;
    }

    private void SetShowCaseAsWeapon(BaseWeapon tempWeapon)
    {
        BaseWeapon selectedWeapon = tempWeapon;
        itemName.text = selectedWeapon.aName;
        itemDescription.text = selectedWeapon.aDescription;
        image.sprite = selectedWeapon.weaponIcon;


        renderTextureMeshFilter.mesh = selectedWeapon.weaponMesh;
        renderTextureMeshRenderer.material = selectedWeapon.weaponMaterial;
    }


    private void SetShowcaseAsItem(ItemBase tempItem)
    {
        ItemBase selectedItem = tempItem;
        itemName.text = tempItem.itemName;
        itemDescription.text = tempItem.itemDescription;
        image.sprite = tempItem.itemIcon;


        renderTextureMeshFilter.mesh = tempItem.itemMesh;
        renderTextureMeshRenderer.material = tempItem.itemMaterial;
    }

    private void AddWeaponsToShop()
    {
        itemPanel = Instantiate(weaponPanelPrefab, weaponParent).GetComponent<ItemPanel>();
        
        GameObject tempWeapon = lootSystem.GetWeaponForShop();
        
        itemPanel.shopPanel = this;
        itemPanel.SetPanelFromWeapon(tempWeapon);

        itemPanel.itemName = itemName;
        itemPanel.itemDescription = itemDescription;
        itemPanel.image = image;

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
