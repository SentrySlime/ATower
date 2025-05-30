using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackSmith : MonoBehaviour, IInteractInterface
{
    GameObject player;
    WeaponSocket weaponSocket;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        weaponSocket = player.GetComponent<WeaponSocket>();
    }

    public void Interact()
    {
        UpgradeWeapon();
    }

    private void UpgradeWeapon()
    {
        BaseShootingLogic shootingLogic = weaponSocket.equippedWeapon.baseShootingLogic1;
        shootingLogic.damage *= 1.5f;

        BaseWeapon baseWeapon = weaponSocket.equippedWeapon;
        baseWeapon.fireRate *= 0.5f;
        baseWeapon.SetAdsFor();

        baseWeapon.maxMagazine = Mathf.CeilToInt(baseWeapon.maxMagazine * 1.5f);
        baseWeapon.currentMagazine = baseWeapon.maxMagazine;
        
        baseWeapon.SetAmmoInfo();

        weaponSocket.UpdateAmmo();
    }

}
