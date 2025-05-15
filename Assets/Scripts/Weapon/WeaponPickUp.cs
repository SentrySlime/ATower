using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : Item, IInteractWeaponInterface
{
    

    public enum WeaponRarity
    {
        S,
        A,
        B,
        C,
        D
    }

    [Header("Item Info")]
    public string aName;
    public string aDescription;
    public Sprite weaponIcon;

    [Header("General references")]
    public GameObject shootPoint;
    public GameObject barrel;
    public GameObject objectToRecoil;
    public GameObject iconPrefab;
    public int goldCost = 500;

    [Header("Mesh & Material")]
    public Mesh weaponMesh;
    public Material weaponMaterial;
    
    public WeaponRarity weaponRarity;


    [Header("Misc")]
    public BaseWeapon baseWeapon;
    public GameObject weaponPrefab;
    private GameObject currentWeapon;



    void Start()
    {
        if (weaponPrefab)
        {
            SetItemInfo();
        }
    }



    void Update()
    {
        transform.Rotate(Vector3.up, 100 * Time.deltaTime);
    }

    public GameObject InteractWeaponPickUp()
    {
        return ReturnWeapon();
    }

    public GameObject ReturnWeapon()
    {
        currentWeapon = Instantiate(weaponPrefab);
        return currentWeapon;
    }

    private void SetItemInfo()
    {
        baseWeapon = weaponPrefab.GetComponentInChildren<BaseWeapon>();
        itemName = baseWeapon.aName;
        itemDescription = baseWeapon.aDescription;
        itemIcon = baseWeapon.weaponIcon;
        weaponMaterial = baseWeapon.weaponMaterial;
        weaponMesh = baseWeapon.weaponMesh;
    }

}
