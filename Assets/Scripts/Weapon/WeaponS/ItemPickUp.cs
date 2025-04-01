using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public enum ItemRarity
    {
        S,
        A,
        B,
        C,
        D
    }

    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    

    public ItemRarity weaponRarity;

    public ItemBase itemPrefab;
    //[SerializeField] GameObject itemMesh;
    [HideInInspector] public GameObject currentItem;
    //private GameObject rotatingMesh;
    [SerializeField] Vector3 size;
    //ItemBase itemBase;
    [SerializeField] Rigidbody rb;

    void Start()
    {
        if(itemPrefab)
        {
            //itemBase = itemPrefab.GetComponent<ItemBase>();
            itemName = itemPrefab.itemName;
            itemDescription = itemPrefab.itemDescription;
            itemIcon = itemPrefab.itemIcon;
            GetComponentInChildren<MeshFilter>().mesh = itemPrefab.itemMesh;
            GetComponentInChildren<MeshRenderer>().material = itemPrefab.itemMaterial;
        }
    }


    void Update()
    {
        transform.Rotate(Vector3.up, 100 * Time.deltaTime);
    }

    public void EquipItem()
    {
        if (itemPrefab)
        {
            currentItem = Instantiate(itemPrefab.gameObject);
            itemPrefab.EquipItem();
        }
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

