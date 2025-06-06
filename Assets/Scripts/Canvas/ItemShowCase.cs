using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShowCase : MonoBehaviour
{
    public Transform itemParent;
    public Transform weaponParent;

    public MeshFilter renderTextureMeshFilter;
    public MeshRenderer renderTextureMeshRenderer;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Image image;

    public void NullTheShowCase()
    {
        itemName.text = " ";
        itemDescription.text = " ";
        image.sprite = null;
        renderTextureMeshFilter.mesh = null;
    }

    public void SetFirstItemDisplay()
    {
        if(itemParent.childCount == 0)
        {
            SetFirstWeaponDisplay();
        }
        else
        {
            ItemPanel panel = itemParent.GetChild(0).GetComponent<ItemPanel>();
            panel.SetItemDisplay();
            
        }
    }

    public void SetFirstWeaponDisplay()
    {
        if (weaponParent.childCount == 0)
        {
            NullTheShowCase();
        }
        else
        {
            ItemPanel panel = weaponParent.GetChild(0).GetComponent<ItemPanel>();
            panel.SetItemDisplay();
        }

    }




    //private void SetTheShowCase(ItemBase tempItem)
    //{
    //    itemName.text = tempItem.itemName;
    //    itemDescription.text = tempItem.itemDescription;
    //    image.sprite = tempItem.itemIcon;


    //    renderTextureMeshFilter.mesh = tempItem.itemMesh;
    //    renderTextureMeshRenderer.material = tempItem.itemMaterial;
    //}

    //private void SetTheShowCase(BaseWeapon baseWeapon)
    //{
    //    itemName.text = baseWeapon.aName;
    //    itemDescription.text = baseWeapon.aDescription;
    //    image.sprite = baseWeapon.weaponIcon;


    //    renderTextureMeshFilter.mesh = baseWeapon.weaponMesh;
    //    renderTextureMeshRenderer.material = baseWeapon.weaponMaterial;
    //}

}
