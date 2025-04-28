using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPanel : MonoBehaviour
{
    [Header("Border")]
    public Image border_Image;
    public Sprite unselected;
    public Sprite selected;

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
        selectedItem.SetSelectedPanel(this);
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
    }

    public void SetPanel(ItemPickUp tempItem)
    {
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