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



    public WeaponRarity weaponRarity;
    [SerializeField] GameObject weaponPrefab;
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
        BaseWeapon weapon = weaponPrefab.GetComponentInChildren<BaseWeapon>();
        itemName = weapon.aName;
        itemDescription = weapon.aDescription;
        itemIcon = weapon.weaponIcon;
    }

}
