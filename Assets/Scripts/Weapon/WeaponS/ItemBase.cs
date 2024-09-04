using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ItemBase : MonoBehaviour
{

    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public Mesh itemMesh;
    public Material itemMaterial;

    void Start()
    {

    }


    void Update()
    {

    }

    public virtual void EquipItem()
    {

    }

    public virtual void UnEquipItem()
    {

    }

}