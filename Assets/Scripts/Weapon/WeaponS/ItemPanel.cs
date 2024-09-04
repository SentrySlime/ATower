using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPanel : MonoBehaviour
{
    public Image buttonImage;

    public string aName;
    public string aDescription;
    public Sprite anIcon;

    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Image image;

    GameObject itemPanel;

    void Start()
    {

    }


    void Update()
    {

    }


    public void SetItemDisplay()
    {
        itemName.text = aName;
        itemDescription.text = aDescription;
        image.sprite = anIcon;
    }

    public void SetPanel(ItemPickUp tempItem)
    {
        aName = tempItem.itemName;
        aDescription = tempItem.itemDescription;
        anIcon = tempItem.itemIcon;

        buttonImage.sprite = anIcon;

        //itemName.text = tempItem.itemName;
        //itemDescription.text = tempItem.itemDescription;
        //image.sprite = tempItem.itemIcon;

    }


}