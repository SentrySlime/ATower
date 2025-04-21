using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo_Refill : Item, IInteractInterface
{

    public GameObject FX;
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        transform.Rotate(Vector3.up, 100 * Time.deltaTime);
    }
    public void Interact()
    {
        RefillAmmo();
    }

    private void RefillAmmo()
    {
        if(!player.GetComponent<WeaponSocket>().equippedWeapon.GetComponentInChildren<BaseWeapon>().CanRefillAmmo()) { return; }

        Instantiate(FX, transform.position, Quaternion.identity);
        BaseWeapon tempWeapon = player.GetComponent<WeaponSocket>().equippedWeapon.GetComponentInChildren<BaseWeapon>();
        tempWeapon.AmmoRefill();
        Destroy(gameObject);
    }

}
