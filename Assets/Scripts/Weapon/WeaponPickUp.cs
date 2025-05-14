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


    public BaseWeapon baseWeapon;
    public WeaponRarity weaponRarity;
    [SerializeField] GameObject weaponPrefab;
    private GameObject currentWeapon;
    [HideInInspector] public Mesh weaponMesh;
    [HideInInspector] public Material weaponMaterial;



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
