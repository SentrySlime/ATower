using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BaseWeapon : MonoBehaviour
{


    public enum ReloadType
    {
        Magazine,
        ShotByShot
    }

    [Header("Item Info")]
    public string aName;
    public string aDescription;
    public Sprite sprite;
    public GameObject shootPoint;
    public GameObject barrel;
    public GameObject objectToRecoil;
    public Sprite weaponIcon;
    public GameObject iconPrefab;

    public enum WeaponType
    {
        handgun,
        SMG,
        AssaultRifle,
        LMG,
        Shotgun,
        Sniper,
        Throwing,
    }

    [Header("Ammunition")]
    public int maxAmmo = 0;
    public int currentAmmo = 0;
    public int ammoPerShot = 1;
    public bool infinteAmmo = false;

    [Header("Magazine")]
    public ReloadType reloadType;
    public int maxMagazine = 0;
    public int currentMagazine = 0;
    public float reloadTime = 1;
    public int reloadAmount = 1;

    [Header("DPS")]
    public float damage = 1;
    public float fireRate = 1;

    [Header("Burst")]
    public float burstDelay = 1;
    public float burstAmount = 1;

    [Header("Recoil Info")]
    public int recoilAmount = 0;
    public float moveAmount = -0.5f;

    [Header("Accuracy")]
    public float ADSAccuracy = 0;
    public float HIPAccuracy = 0;

    [Header("Screenshake")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;

    [Header("ADS")]
    public Vector3 hipPos;
    public Vector3 adsPos;
    public float adsSpeed = 6;


    [HideInInspector] public TextMeshProUGUI maxAmmoText;
    [HideInInspector] public TextMeshProUGUI currentMagazineText;
    [HideInInspector] public Image ammoFill;

    //[Header("FiringMode")]
    public enum FiringMode { fullAuto, semiAuto, burst };

    public FiringMode firingMode;

    [HideInInspector] public Camera aimCamera;
    [HideInInspector] public Recoil recoil;
    public ScreenShake screenShake;
    [HideInInspector] public WeaponSocket weaponSocket;
    public PlayerStats playerStats;


    public bool interuptReload = false;

    public GameObject[] renderObjects;

    private void Awake()
    {

    }

    public virtual void Start()
    {
        //This is a virtual "override start function" so the children inherit and use this (So don't remove)
    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0) /*|| Input.GetKeyDown(KeyCode.R)*/)
            StartCoroutine(ReloadInteruption());

        if (currentMagazine == 0 && currentAmmo == 0)
        {
            if (playerStats.ammoRefills > 0 && !infinteAmmo)
            {
                currentMagazine = maxMagazine;
                currentAmmo = maxAmmo;
                SetAmmoInfo();
                playerStats.ammoRefills--;
            }
        }
    }


    public virtual void ReloadWeapon()
    {


        //We don't want to reload if we don't have enough ammo
        if (currentAmmo == 0 && !infinteAmmo) { return; }
        //if (infinteAmmo) { return; }

        if (reloadType == ReloadType.Magazine)
        {

            if (infinteAmmo)
            {
                currentMagazine = maxMagazine;
            }
            else
            {
                //Check how much ammo we have left
                currentAmmo -= maxMagazine - currentMagazine;

                int remainingAmmo = 0;

                if (currentAmmo < 0)
                {
                    //Use only the ammo we have to reload
                    remainingAmmo = 0 - currentAmmo;
                    currentMagazine = maxMagazine - remainingAmmo;
                    currentAmmo = 0;
                }
                else
                    currentMagazine = maxMagazine; //If we had more or as much ammo as the magazine can fit, we don't need to do the math
            }


            currentAmmo = Mathf.Clamp(currentAmmo, 0, 99999);
            //Then we update the hud with our ammo info
            if(!recoil.holstered)
                SetAmmoInfo();
        }
        else if (reloadType == ReloadType.ShotByShot)
        {
            if (currentMagazine >= maxMagazine) { return; }

            if (infinteAmmo)
            {
                for (int i = 0; i < reloadAmount + playerStats.reloadAmount; i++)
                {
                    if (currentMagazine != maxMagazine)
                    {
                        currentMagazine++;
                    }
                }
            }
            else
            {

                int reloadNumber = Mathf.Clamp(reloadAmount + playerStats.reloadAmount, 1, maxMagazine - currentMagazine);
             
                for (int i = 0; i < reloadNumber; i++)
                {
                    if (currentAmmo != 0 || currentMagazine != maxMagazine)
                    {
                        currentAmmo--;
                        currentMagazine++;
                    }
                }

            }

            currentAmmo = Mathf.Clamp(currentAmmo, 0, 99999);
            //Then we update the hud with our ammo info
            if (!recoil.holstered)
                SetAmmoInfo();

            if (recoil.holstered)
                return;

            if (currentMagazine < maxMagazine && !interuptReload)
                StartCoroutine(weaponSocket.Reloading());
            else if (reloadType == ReloadType.ShotByShot)
                MantleWeapon();



        }

    }

    //This should be called when switching weapons
    public virtual void Initialize(Camera camera)
    {

        TopLayer();
        //Setting up references to other classes
        aimCamera = camera;
        recoil.InitializeRecoil(moveAmount, recoilAmount, fireRate, (int)firingMode);
        screenShake.InitializeShake(recoilX, recoilY, recoilZ);
        weaponSocket.SetAds(adsPos, hipPos, fireRate, adsSpeed);

        //Text based elements for ammo
        SetAmmoInfo();
    }

    public bool HasFullMagazine()
    {
        if(currentMagazine == maxMagazine)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool OutOfAmmo()
    {
        if(currentAmmo == 0)
            return true;
        else
            return false;
    }

    //This is called everytime you shot, as it takes ammo 
    public virtual void TakeAmmo()
    {

        currentMagazine -= ammoPerShot;
        SetAmmoInfo();
    }

    //Refill a percentage of max ammo
    public void AmmoRefill()
    {
        currentAmmo = maxAmmo;
        RefillMag();
        SetAmmoInfo();

    }

    public void RefillMag()
    {
        currentMagazine = maxMagazine;
        currentMagazine = maxMagazine;
    }

    //Set the HUD info for the weapons
    public void SetAmmoInfo()
    {
        currentMagazineText.text = currentMagazine.ToString();

        if (infinteAmmo)
        {

            maxAmmoText.text = "\u221E";
        }
        else
        {
            maxAmmoText.text = currentAmmo.ToString();

        }

        //This is for the bar which fills depending on how much ammo you have
        ammoFill.fillAmount = (float)currentMagazine / (float)maxMagazine;

        if (currentMagazine >= maxMagazine)
        {
            MantleWeapon();
        }

    }

    //All guns use this *Do not remove*
    public virtual void TriggerItem()
    {

    }

    public void TopLayer()
    {

        if (renderObjects == null) { return; }


        int topLayer = LayerMask.NameToLayer("WeaponLayer");
        for (int i = 0; i < renderObjects.Length; i++)
        {

            renderObjects[i].layer = topLayer;

        }
    }

    public void DefaultLayer()
    {
        if (renderObjects == null) { return; }

        int defaultLayer = LayerMask.NameToLayer("Default");
        for (int i = 0; i < renderObjects.Length; i++)
        {
            Debug.LogError("Default layer" + i);
            renderObjects[i].layer = defaultLayer;

        }
    }

    IEnumerator ReloadInteruption()
    {
        interuptReload = true;
        yield return new WaitForSeconds(0.4f);
        interuptReload = false;
    }

    public void MantleWeapon()
    {
        weaponSocket.currentTimer = 0;

        if (GetComponentInChildren<Animation>() != null)
            GetComponentInChildren<Animation>().Play();

    }
}

