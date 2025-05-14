using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPanel : MonoBehaviour
{
    [Header("Border")]
    [HideInInspector] public ItemBase itemBase;
    public BaseWeapon baseWeapon;

    public Image border_Image;
    public Sprite unselected;
    public Sprite selected;
    public TextMeshProUGUI goldCost;

    public Image buttonImage;

    public string aName;
    public string aDescription;
    public Sprite anIcon;


    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Image image;
    public bool isDevilItem = false;

    GameObject itemPanel;
    SelectedItem selectedItem;
    [HideInInspector] public ShopPanel shopPanel;

    [HideInInspector] public Mesh itemMesh;
    [HideInInspector] public Material itemMaterial;


    void Start()
    {
        selectedItem = GetComponentInParent<SelectedItem>();
    }


    void Update()
    {

    }

    public void Selected()
    {
        if (selectedItem)
        {
            selectedItem.SetSelectedPanel(this);
        }
        border_Image.sprite = selected;
    }

    public void UnSelected()
    {
        border_Image.sprite = unselected;
    }

    public void SetItemDisplay()
    {
        Selected();
        itemName.text = aName;
        itemDescription.text = aDescription;
        image.sprite = anIcon;

        if (shopPanel)
        {
            shopPanel.selectedItem = itemBase;
            shopPanel.selectedIcon = this.gameObject;
        }
    }

    public void SetPanelFromWeapon(BaseWeapon incomingWeapon)
    {
        baseWeapon = incomingWeapon;

        itemMesh = incomingWeapon.weaponMesh;
        itemMaterial = incomingWeapon.weaponMaterial;

        aName = incomingWeapon.aName;
        aDescription = incomingWeapon.aDescription;
        anIcon = incomingWeapon.weaponIcon;

        buttonImage.sprite = anIcon;

        //if (shopPanel)
        //{
        //    goldCost.text = incomingWeapon.goldCost.ToString();

        //}
    }

    public void SetPanelFromItemBase(ItemBase incomingItem)
    {
        itemBase = incomingItem;

        itemMesh = incomingItem.itemMesh;
        itemMaterial = incomingItem.itemMaterial;

        aName = incomingItem.itemName;
        aDescription = incomingItem.itemDescription;
        anIcon = incomingItem.itemIcon;
            
        buttonImage.sprite = anIcon;

        if (shopPanel)
        {
            goldCost.text = incomingItem.goldCost.ToString();

        }
    }

    public void SetPanelFromPickUp(ItemBase tempItem)
    {
        itemBase = tempItem;

        itemMesh = tempItem.itemMesh;
        itemMaterial = tempItem.itemMaterial;

        aName = tempItem.itemName;
        aDescription = tempItem.itemDescription;
        anIcon = tempItem.itemIcon;
        isDevilItem = tempItem.isDevilItem;

        if(isDevilItem)
            border_Image.color = Color.red;


        buttonImage.sprite = anIcon;

    }
}