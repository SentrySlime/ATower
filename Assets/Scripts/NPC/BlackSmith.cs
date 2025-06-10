using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BlackSmith : MonoBehaviour, IInteractInterface
{

    GameObject player;
    Inventory inventory;
    WeaponSocket weaponSocket;
    BaseWeapon baseWeapon;

    [Header("Stats")]
    public int goldCost = 950;

    [Header("SFX & VFX")]
    public AudioSource noMoneySFX;
    public AudioSource purchaseSFX;

    [Header("SFX & VFX")]
    public ParticleSystem hitVFX;
    public AudioSource hitSFX;
    public MeshFilter renderTextureMeshFilter;
    public MeshRenderer renderTextureMeshRenderer;

    [Header("Default meshAttributes")]
    public Material defaultMaterial;
    public Mesh defaultMesh;

    bool active = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<Inventory>();
        weaponSocket = player.GetComponent<WeaponSocket>();
    }

    public void Interact()
    {
        if (active || weaponSocket.equippedWeapon == null || weaponSocket.interupptedBool || weaponSocket.equippedWeapon.upgraded) return;

        if(inventory.money <= goldCost)
        {
            noMoneySFX.Play();
            return;
        }
        else
        {
            purchaseSFX.Play();
            inventory.DecreaseMoney(goldCost);
            StartCoroutine(StartBlackSmith());
        }
    }

    private IEnumerator StartBlackSmith()
    {
        yield return new WaitForSeconds(0.1f);

        active = true;
        baseWeapon = weaponSocket.equippedWeapon;
        weaponSocket.interupptedBool = true;
        baseWeapon.HideWeapon();
        SetMesh();
    }

    private void SetMesh()
    {
        renderTextureMeshFilter.mesh = baseWeapon.weaponMesh;
        renderTextureMeshRenderer.material = baseWeapon.weaponMaterial;
        StartCoroutine(PlayVFXRepeatedly());
    }

    private void NullMesh()
    {
        renderTextureMeshRenderer.material = defaultMaterial;
        renderTextureMeshFilter.mesh = defaultMesh;
    }

    
    private IEnumerator PlayVFXRepeatedly()
    {
        yield return new WaitForSeconds(1);

        int repeatCount = 3;
        float interval = 1f;

        for (int i = 0; i < repeatCount; i++)
        {
            PlayVFX();
            yield return new WaitForSeconds(interval);
        }

        Invoke("UpgradeWeapon", 0.3f);
    }

    private void PlayVFX()
    {
        hitVFX.Play();
        hitSFX.Play();
    }

    private void UpgradeWeapon()
    {
        BaseShootingLogic shootingLogic = weaponSocket.equippedWeapon.baseShootingLogic1;
        shootingLogic.damage *= 1.5f;
        
        baseWeapon.fireRate *= 0.5f;
        baseWeapon.SetAdsFor();

        baseWeapon.baseMaxMagazine = Mathf.CeilToInt(baseWeapon.baseMaxMagazine * 1.5f);
        baseWeapon.maxMagazine = baseWeapon.baseMaxMagazine;
        baseWeapon.currentMagazine = baseWeapon.maxMagazine;
        
        baseWeapon.upgraded = true;

        baseWeapon.SetAmmoInfo();

        weaponSocket.UpdateAmmo();

        ReturnWeapon();
    }

    private void ReturnWeapon()
    {
        NullMesh();
        baseWeapon.ShowWeapon();
        weaponSocket.interupptedBool = false;
        active = false;
    }

}
