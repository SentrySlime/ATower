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
    public Sprite weaponIcon;

    [Header("General references")]
    public GameObject shootPoint;
    public GameObject barrel;
    public GameObject objectToRecoil;
    public GameObject iconPrefab;

    public enum WeaponType
    {
        Handgun,
        AssaultRifle,
        Shotgun,
        Arrow,
        Explosive,
        Energy,
        EnergyDark,
        EnergyRifle,
        MagicHandgun
         
    }

    public WeaponType type;

    [Header("Ammunition")]
    [HideInInspector] public int baseMaxAmmo = 0;
    public int maxAmmo = 0;
    public int currentAmmo = 0;
    [HideInInspector] public int baseAmmoPerShot = 0;
    public int ammoPerShot = 1;
    [HideInInspector] public bool baseInfinteAmmo = false;
    public bool infinteAmmo = false;

    [Header("Magazine")]
    public ReloadType reloadType;
    [HideInInspector] public int baseMaxMagazine = 0;
    public int maxMagazine = 0;
    public int currentMagazine = 0;
    [HideInInspector] public float baseReloadTime = 0;
    public float reloadTime = 1;
    [HideInInspector] public int baseReloadAmount = 0;
    public int reloadAmount = 1;

    [Header("DPS")]
    [HideInInspector] public float baseDamage = 0;
    public float damage = 1;
    [HideInInspector] public float baseFireRate = 0;
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
    [HideInInspector] public TextMeshProUGUI currentAmmoText;
    [HideInInspector] public TextMeshProUGUI currentMagazineText;
    [HideInInspector] public TextMeshProUGUI ammoDivider;
    [HideInInspector] public Image ammoFill;

    //[Header("FiringMode")]
    public enum FiringMode { fullAuto, semiAuto, burst };

    public FiringMode firingMode;

    [HideInInspector] public Camera aimCamera;
    [HideInInspector] public Recoil recoil;
    public ScreenShake screenShake;
    [HideInInspector] public WeaponSocket weaponSocket;
    [HideInInspector] public AMainSystem aMainSystem;
    public PlayerStats playerStats;


    public bool interuptReload = false;
    [HideInInspector] public bool finishedReload = false;
    public GameObject[] renderObjects;

    public void Awake()
    {
        playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
        aMainSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();

        recoil = GetComponentInParent<Recoil>();
        screenShake = GetComponentInParent<ScreenShake>();
        weaponSocket = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponSocket>();


        GameObject currentTextObj;


        currentTextObj = GameObject.Find("CurrentMagazineText");
        if (currentTextObj)
            currentMagazineText = currentTextObj.GetComponent<TextMeshProUGUI>();

        //currentTextObj = GameObject.Find("MaxAmmoText");
        //if (currentTextObj)
        //    maxAmmoText = currentTextObj.GetComponent<TextMeshProUGUI>();


        //currentTextObj = GameObject.Find("CurrentAmmoText");
        //if(currentTextObj)
        //    currentAmmoText = currentTextObj.GetComponent<TextMeshProUGUI>();


        //currentTextObj = GameObject.Find("CurrentMagazineText");
        //if (currentTextObj)
        //    currentMagazineText = currentTextObj.GetComponent<TextMeshProUGUI>();


        //currentTextObj = GameObject.Find("AmmoDivider");
        //if (currentTextObj)
        //    ammoDivider = currentTextObj.GetComponent<TextMeshProUGUI>();


        


        SetBaseStatsOnSpawn();
    }

    #region All

    public void Start()
    {
        maxAmmoText = iconPrefab.GetComponent<WeaponIcon>().maxAmmoText;

        currentAmmoText = iconPrefab.GetComponent<WeaponIcon>().currentAmmoText;

        ammoDivider = iconPrefab.GetComponent<WeaponIcon>().ammoDivider;

        currentAmmo = maxAmmo;
        currentMagazine = maxMagazine;

    }

    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if (HasEmptyMagazine()) { return; }

            StartCoroutine(ReloadInteruption());
        }

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
                if (!recoil.holstered)
                    weaponSocket.AmmoVisualRefillMagazine();
            }
            else
            {
                //Check how much ammo we have left
                int tempCurrentAmmo = currentAmmo;
                currentAmmo -= maxMagazine - currentMagazine;

                int remainingAmmo = 0;

                if (currentAmmo < 0)
                {
                    //Use only the ammo we have to reload
                    remainingAmmo = 0 - currentAmmo;
                    currentMagazine = maxMagazine - remainingAmmo;
                    currentAmmo = 0;
                    if (!recoil.holstered)
                    {
                        weaponSocket.AmmoVisualRefillMagazineAmount(tempCurrentAmmo);
                    }
                }
                else
                {
                    currentMagazine = maxMagazine; //If we had more or as much ammo as the magazine can fit, we don't need to do the math
                    if (!recoil.holstered)
                        weaponSocket.AmmoVisualRefillMagazine();
                    
                }

            }

            currentAmmo = Mathf.Clamp(currentAmmo, 0, 99999);
            //Then we update the hud with our ammo info
            
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
                        if (!recoil.holstered)
                            weaponSocket.AmmoVisualOneByOne();

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

                        if (!recoil.holstered)
                            weaponSocket.AmmoVisualOneByOne();
                    }
                }

            }


            currentAmmo = Mathf.Clamp(currentAmmo, 0, 99999);
            finishedReload = true;
            //Then we update the hud with our ammo info
            
            SetAmmoInfo();

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

        StartCoroutine(InitalizeCoroutine());
    }

    IEnumerator InitalizeCoroutine()
    {
        yield return null;

        weaponSocket.SetAds(adsPos, hipPos, fireRate, adsSpeed);
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

    public bool HasEmptyMagazine()
    {
        if (currentMagazine == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool HasFullAmmo()
    {
        if (currentAmmo == maxAmmo)
            return true;
        else
            return false;
    }

    public bool OutOfAmmo()
    {
        if(currentAmmo == 0)
            return true;
        else
            return false;
    }

    public bool HasInfiniteAmmo()
    {
        if (infinteAmmo)
            return true;
        else
            return false;
    }

    public bool CanRefillAmmo()
    {
        if (HasInfiniteAmmo())
            return false;
        else
            return true;
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

    public void GiveAmmoToMagazine(int incomingAmmo)
    {
        if (HasFullMagazine() || OutOfAmmo() || incomingAmmo <= 0)
        {
            return;
        }

        int neededAmmo = maxMagazine - currentMagazine; // How much we need to fully reload
        int ammoToReload = Mathf.Min(incomingAmmo, neededAmmo, currentAmmo); // Take only what's available and allowed

        currentMagazine += ammoToReload;
        currentAmmo -= ammoToReload;

        // Visual and ammo update
        if (!recoil.holstered)
        {
            if (ammoToReload < neededAmmo)
                weaponSocket.AmmoVisualRefillMagazineAmount(ammoToReload);
            else
                weaponSocket.AmmoVisualRefillMagazine();
        }

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
        
        if(!recoil.holstered)
        {
            currentMagazineText.text = currentMagazine.ToString();
            SetCurrentMagazineTextColor();
        }
        
        

        if (infinteAmmo)
        {
            currentAmmoText.text = " ";
            ammoDivider.text = "\u221E";
            maxAmmoText.text = " ";
        }
        else
        {
            currentAmmoText.text = currentAmmo.ToString();
            SetCurrentAmmoTextColor();
            ammoDivider.text = "/";
            maxAmmoText.text = maxAmmo.ToString();

        }

        //if (currentMagazine >= maxMagazine)
        //{
        //    MantleWeapon();
        //}

    }

    private void SetCurrentMagazineTextColor()
    {
        float ammoPercent = (float)currentMagazine / maxMagazine;

        Color color;
        if (ammoPercent > 0.66f)
        {
            // From white to yellow
            float t = (ammoPercent - 0.66f) / (1f - 0.66f);
            color = Color.Lerp(Color.yellow, Color.white, t);
        }
        else if (ammoPercent > 0.33f)
        {
            // From yellow to orange
            float t = (ammoPercent - 0.33f) / (0.66f - 0.33f);
            color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.yellow, t); // Orange to Yellow
        }
        else
        {
            // From orange to red
            float t = ammoPercent / 0.33f;
            color = Color.Lerp(Color.red, new Color(1f, 0.5f, 0f), t); // Red to Orange
        }

        currentMagazineText.color = color;

    }

    private void SetCurrentAmmoTextColor()
    {
        float ammoPercent = (float)currentAmmo / maxAmmo;

        Color color;
        if (ammoPercent > 0.66f)
        {
            // From white to yellow
            float t = (ammoPercent - 0.66f) / (1f - 0.66f);
            color = Color.Lerp(Color.yellow, Color.white, t);
        }
        else if (ammoPercent > 0.33f)
        {
            // From yellow to orange
            float t = (ammoPercent - 0.33f) / (0.66f - 0.33f);
            color = Color.Lerp(new Color(1f, 0.5f, 0f), Color.yellow, t); // Orange to Yellow
        }
        else
        {
            // From orange to red
            float t = ammoPercent / 0.33f;
            color = Color.Lerp(Color.red, new Color(1f, 0.5f, 0f), t); // Red to Orange
        }

        currentAmmoText.color = color;

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

    #endregion

    public void SetBaseStatsOnSpawn()
    {
        baseMaxAmmo = maxAmmo;
        baseAmmoPerShot = ammoPerShot;
        baseInfinteAmmo = infinteAmmo;
        baseMaxMagazine = maxMagazine;
    }
}

