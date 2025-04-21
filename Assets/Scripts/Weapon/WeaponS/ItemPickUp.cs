using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : Item, IInteractInterface
{
    public enum ItemRarity
    {
        S,
        A,
        B,
        C,
        D
    }
    
    public ItemRarity weaponRarity;

    public ItemBase itemPrefab;
    //[SerializeField] GameObject itemMesh;
    [HideInInspector] public GameObject currentItem;
    //private GameObject rotatingMesh;
    [SerializeField] Vector3 size;
    //ItemBase itemBase;
    [SerializeField] Rigidbody rb;

    [HideInInspector] public ItemPickUp itemPickUp;

    [HideInInspector] public Mesh itemMesh;
    [HideInInspector] public Material itemMaterial;

    void Start()
    {
        if(itemPrefab)
        {
            SetItemInfo();
        }
    }

    

    void Update()
    {
        transform.Rotate(Vector3.up, 100 * Time.deltaTime);
    }

    public void Interact()
    {
        EquipItem();
    }

    public void EquipItem()
    {
        if (itemPrefab)
        {
            currentItem = Instantiate(itemPrefab.gameObject);
            itemPrefab.EquipItem();
        }
    }

    private void SetItemInfo()
    {
        itemName = itemPrefab.itemName;
        itemDescription = itemPrefab.itemDescription;
        itemIcon = itemPrefab.itemIcon;
        GetComponentInChildren<MeshFilter>().mesh = itemPrefab.itemMesh;
        itemMesh = itemPrefab.itemMesh;
        GetComponentInChildren<MeshRenderer>().material = itemPrefab.itemMaterial;
        itemMaterial = itemPrefab.itemMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 0)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

  
}

