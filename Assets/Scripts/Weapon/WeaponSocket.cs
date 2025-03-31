using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSocket : MonoBehaviour
{

    public float baseCameraFOV;
    public float zoomedCameraFOV;

    public float baseSensitivity;
    public float zoomedSensitivity;

    public Camera weaponCamera;
    public Camera cameraObj;
    public CameraMovement cameraMovement;
    public GameObject obj;
    public AudioSource noAmmo;
    ShootSystem shootSystem;
    PlayerStats playerStats;
    AmmoScript ammoScript;

    [Header("Visible Cursor")]
    public bool hideCursor;

    public BaseWeapon equippedWeapon;
    ScreenShake screenShake;
    Recoil recoilScript;

    [Header("Weapon pos")]
    Transform pistol;
    Transform rifle;
    Transform smg;
    Transform shotgun;
    Transform rocketlauncher;

    [Header("SFX")]
    public AudioSource wSwitch_SFX;
    public AudioSource reload_SFX;

    //public Quaternion hipRot;
    //public Quaternion adsRot;

    [Header("Aiming")]
    public Vector3 hipPos;
    public Vector3 adsPos;
    public float adsProgress = 0;
    public float adsSpeed = 6;
    public CanvasGroup crosshair;

    [Header("Reload")]
    public CanvasGroup reloadGroup;
    public Image reloadIcon;
    public Image reloadFinish;

    [Header("Misc")]
    PauseMenu pauseMenu;
    public Image weaponIcon;
    public bool interupptedBool = false;

    float maxTimer = 0.08f;
    public float currentTimer = 0;

    float burstDelay = 0.1f;
    //float burstTimer = 0;

    public enum FireMode { fullAuto, semi, burst };
    public FireMode fireMode;

    private void Awake()
    {
        GameObject tempObj = GameObject.FindGameObjectWithTag("ShootPoint");
        screenShake = GetComponentInChildren<ScreenShake>();
        cameraMovement = GetComponentInChildren<CameraMovement>();
        reloadIcon = GameObject.FindGameObjectWithTag("ReloadImage").GetComponent<Image>();
        //weaponIcon = GameObject.FindGameObjectWithTag("WeaponIcon").GetComponent<Image>();
        crosshair = GameObject.FindGameObjectWithTag("Crosshair").GetComponent<CanvasGroup>();
        reloadIcon.enabled = false;
        pauseMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PauseMenu>();
        reloadGroup = GameObject.FindGameObjectWithTag("ReloadGroup").GetComponent<CanvasGroup>();
        reloadFinish = reloadGroup.transform.Find("ReloadFinish").GetComponent<Image>();
        shootSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ShootSystem>();
        ammoScript = GameObject.Find("AmmoBar").GetComponent<AmmoScript>();
        playerStats = GetComponent<PlayerStats>();
        reloadGroup.alpha = 0;
    }

    void Start()
    {
        reloadFinish.enabled = false;

        SetFOVnSens();

        if (hideCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (pauseMenu.paused || equippedWeapon == null) { return; }

        if (equippedWeapon)
        {
            if (Input.GetKeyDown(KeyCode.R) && equippedWeapon.currentMagazine < equippedWeapon.maxMagazine)
            {
                Reload();

            }
        }

        if (currentTimer < maxTimer)
        {
            currentTimer += Time.deltaTime;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if(reloadIcon.enabled) 
                { 
                    StopReload(); 
                    
                    if(equippedWeapon.currentMagazine < equippedWeapon.ammoPerShot)
                        return; 
                }

                if (fireMode == FireMode.semi)
                {
                    Fire();
                    currentTimer = 0;
                }
                else if (fireMode == FireMode.burst)
                {
                    StartCoroutine(BurstFire());
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (fireMode == FireMode.fullAuto)
                {
                    if (Input.GetMouseButton(0))
                    {
                        Fire();
                        currentTimer = 0;
                    }
                }
            }
        }

        if (Input.GetMouseButton(1) )
        {
            if (adsProgress < 0.1)
            {
                adsProgress = 0;
            }
            else
            {
                adsProgress -= adsSpeed * Time.deltaTime;
            }


        }
        else if (adsProgress >= 0)
        {

            if (adsProgress > 0.999)
            {
                adsProgress = 1;
            }
            else
            {
                adsProgress += adsSpeed * Time.deltaTime;
            }
        }

        if (equippedWeapon != null)
        {
            equippedWeapon.objectToRecoil.gameObject.transform.localPosition = Vector3.Lerp(adsPos, hipPos, adsProgress);

            cameraObj.fieldOfView = Mathf.Lerp(zoomedCameraFOV, baseCameraFOV, adsProgress);
            weaponCamera.fieldOfView = Mathf.Lerp(zoomedCameraFOV, baseCameraFOV, adsProgress);

            cameraMovement.Sensitivity = Mathf.Lerp(zoomedSensitivity, baseSensitivity, adsProgress);
            crosshair.alpha = adsProgress;
        }
    }

    public void Fire()
    {
        
        if (equippedWeapon == null || reloadIcon.isActiveAndEnabled) { return; }

        if (equippedWeapon.currentMagazine >= equippedWeapon.ammoPerShot)
        {
            recoilScript.CallFire();
            equippedWeapon.TriggerItem();
            equippedWeapon.TakeAmmo();
            ammoScript.UseAmmo();
            shootSystem.FireHomingRocket(false, equippedWeapon.barrel, 30, playerStats.fireBallChance);
        }
        else if (equippedWeapon.currentMagazine <= 0 && equippedWeapon.currentAmmo != 0)
        {
            Reload();
        }
        else
        {
            if (!noAmmo.isPlaying)
                noAmmo.Play();
        }

    }


    public void Reload()
    {
        if (equippedWeapon.currentAmmo == 0 && !equippedWeapon.infinteAmmo) { return; }
        if (equippedWeapon == null || reloadIcon.isActiveAndEnabled) { return; }

        if (!reload_SFX.isPlaying)
            reload_SFX.Play();
        StartCoroutine(Reloading());
    }

    public void SetAds(Vector3 ads, Vector3 hipFire, float fireRate, float AdsSpeed)
    {
        adsPos = ads;
        hipPos = hipFire;
        maxTimer = fireRate;
        adsSpeed = AdsSpeed;
        if (equippedWeapon.firingMode == BaseWeapon.FiringMode.fullAuto)
            fireMode = FireMode.fullAuto;
        else if (equippedWeapon.firingMode == BaseWeapon.FiringMode.semiAuto)
            fireMode = FireMode.semi;
        else if (equippedWeapon.firingMode == BaseWeapon.FiringMode.burst)
            fireMode = FireMode.burst;
    }

    private IEnumerator BurstFire()
    {
        currentTimer = 0;
        for (int i = 0; i < equippedWeapon.burstAmount; i++)
        {
            Fire();
            yield return new WaitForSeconds(equippedWeapon.burstDelay);
        }


        yield return null;
    }

    public void SetUpWeapon(GameObject incomingObj)
    {
        if (!wSwitch_SFX.isPlaying)
            wSwitch_SFX.Play();
        if(!equippedWeapon)
            SetFOVnSens();
        
        InitalizeItem(incomingObj);

        

        if (equippedWeapon != null)
            hipPos = equippedWeapon.hipPos;
    }

    public void InitalizeItem(GameObject incomingObj)
    {
        incomingObj.GetComponent<Recoil>().IUsed = true;
        equippedWeapon = incomingObj.GetComponentInChildren<BaseWeapon>();
        equippedWeapon.screenShake = screenShake;
        recoilScript = incomingObj.GetComponent<Recoil>();
        equippedWeapon.Initialize(cameraObj);

        UpdateAmmo();

    }

    public void UpdateAmmo()
    {
        ammoScript.maxMagazine = equippedWeapon.maxMagazine;
        ammoScript.currentMagazine = equippedWeapon.currentMagazine - 1;
        //ammoScript.ammoSprite = equippedWeapon.sprite;
        ammoScript.SetAmmoType(equippedWeapon.type);

        ammoScript.UpdateAmmoInfo();
    }

    public void StopReload()
    {
        StopCoroutine(Reloading());
        reloadGroup.alpha = 0;
        reloadIcon.fillAmount = 0f;
        reloadIcon.enabled = false;
    }

    public IEnumerator Reloading()
    {
        reloadIcon.fillAmount = 0f;
        reloadIcon.enabled = true;
        reloadGroup.alpha = 1;

        while (reloadIcon.fillAmount < 1 && reloadIcon.isActiveAndEnabled)
        {
            yield return new WaitForEndOfFrame();

            float totalReloadSpeed = 1 + playerStats.reloadSpeed;

            if (playerStats.hasAlternateFastReload > 0 && playerStats.alternateFastReload)
                totalReloadSpeed += 0.5f;

            reloadIcon.fillAmount += totalReloadSpeed * (1.0f / equippedWeapon.reloadTime * Time.deltaTime);
        }

        if (reloadIcon.fillAmount >= 1)
        {
            if (playerStats.hasAlternateFastReload > 0)
                playerStats.alternateFastReload = !playerStats.alternateFastReload;

            
            equippedWeapon.ReloadWeapon();


            if(equippedWeapon.HasFullMagazine() || equippedWeapon.OutOfAmmo())
                StartCoroutine(ReloadFinish());
            else
                StartCoroutine(PartialReloadFinish());

        }


        yield return null;
    }

    public void AmmoVisualRefillMagazine()
    {
        ammoScript.RefillMagazine();
    }

    public void AmmoVisualRefillMagazineAmount(int amount)
    {
        ammoScript.RefillMagazineAmount(amount);
    }

    public void AmmoVisualOneByOne()
    {
        ammoScript.FillAmmo();
    }

    public void AmmoVisualOneByOne(int ammoAmount)
    {
        ammoScript.FillAmmo(ammoAmount);
    }

    public void SetFOVnSens()
    {
        baseCameraFOV = cameraObj.fieldOfView;
        zoomedCameraFOV = baseCameraFOV * 0.7f;

        baseSensitivity = cameraMovement.Sensitivity;
        zoomedSensitivity = cameraMovement.Sensitivity * 0.7f;
    }

    public IEnumerator ReloadFinish()
    {
        reloadFinish.enabled = true;
        yield return new WaitForSeconds(0.1f);
        reloadGroup.alpha = 0;
        reloadFinish.enabled = false;
        reloadIcon.enabled = false;
    }

    public IEnumerator PartialReloadFinish()
    {
        reloadFinish.enabled = true;
        yield return new WaitForSeconds(0.1f);
        reloadFinish.enabled = false;
    }

}
