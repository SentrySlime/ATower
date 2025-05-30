using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BaseWeapon;

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
    PlayerHealth playerHealth;
    AmmoScript ammoScript;
    GameObject gameManager;
    Settings settings;

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

    Coroutine burstFireCoroutine;
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
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        shootSystem = gameManager.GetComponent<ShootSystem>();
        settings = GameObject.Find("SettingsMenu").GetComponent<Settings>();
        ammoScript = GameObject.Find("AmmoBar").GetComponent<AmmoScript>();
        playerStats = GetComponent<PlayerStats>();
        playerHealth = GetComponent<PlayerHealth>();
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
        if (pauseMenu.paused || equippedWeapon == null || interupptedBool) { return; }

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
            BasShootLogic1();
        }

        if (!equippedWeapon.baseShootingLogic2)
            ADSLogic();


        if (equippedWeapon != null)
        {
            equippedWeapon.objectToRecoil.gameObject.transform.localPosition = Vector3.Lerp(adsPos, hipPos, adsProgress);

            cameraObj.fieldOfView = Mathf.Lerp(zoomedCameraFOV, baseCameraFOV, adsProgress);
            weaponCamera.fieldOfView = Mathf.Lerp(zoomedCameraFOV, baseCameraFOV, adsProgress);

            cameraMovement.Sensitivity = Mathf.Lerp(zoomedSensitivity, baseSensitivity, adsProgress);
            crosshair.alpha = adsProgress;
        }
    }

    private void BasShootLogic1()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (reloadIcon.enabled)
            {
                if (!equippedWeapon.HasEmptyMagazine())
                    StopReload();

                if (equippedWeapon.currentMagazine < equippedWeapon.ammoPerShot1)
                    return;
            }

            if(equippedWeapon.baseShootingLogic1.firingMode == BaseShootingLogic.FiringMode.semiAuto)
            {
                CheckForFire(false);
                currentTimer = 0;
            }
            else if (equippedWeapon.baseShootingLogic1.firingMode == BaseShootingLogic.FiringMode.burst)
            {
                burstFireCoroutine = StartCoroutine(BurstFire(false));
            }
        }
        else if (Input.GetMouseButton(0))
        {
            if (equippedWeapon.baseShootingLogic1.firingMode == BaseShootingLogic.FiringMode.fullAuto)
            {

                CheckForFire(false);
                currentTimer = 0;

            }
        }
        else if(equippedWeapon.baseShootingLogic2)
        {
            BasShootLogic2();
        }
    }

    private void BasShootLogic2()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (reloadIcon.enabled)
            {
                if (!equippedWeapon.HasEmptyMagazine())
                    StopReload();

                if (equippedWeapon.currentMagazine < equippedWeapon.ammoPerShot2)
                    return;
            }

            if (equippedWeapon.baseShootingLogic2.firingMode == BaseShootingLogic.FiringMode.semiAuto)
            {
                CheckForFire(true);
                currentTimer = 0;
            }
            else if (equippedWeapon.baseShootingLogic2.firingMode == BaseShootingLogic.FiringMode.burst)
            {
                burstFireCoroutine = StartCoroutine(BurstFire(true));
            }
        }
        else if (Input.GetMouseButton(1))
        {
            if (equippedWeapon.baseShootingLogic2.firingMode == BaseShootingLogic.FiringMode.fullAuto)
            {

                CheckForFire(true);
                currentTimer = 0;

            }
        }
    }

    private void ADSLogic()
    {
        if (Input.GetMouseButton(1))
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
    }

    public void CheckForFire(bool isSecondary)
    {
        
        if (equippedWeapon == null || reloadIcon.isActiveAndEnabled) { return; }

        int percentageOfPlayerHp = playerHealth.CalculatePercentage(5);

        if (equippedWeapon.currentMagazine >= equippedWeapon.ammoPerShot1 && !isSecondary)
        {
            equippedWeapon.TakeAmmo1();
            ammoScript.UseAmountOfAmmo(equippedWeapon.ammoPerShot1);
            Fire(isSecondary);
        }
        else if(equippedWeapon.currentMagazine >= equippedWeapon.ammoPerShot2 && isSecondary)
        {
            equippedWeapon.TakeAmmo2();
            ammoScript.UseAmountOfAmmo(equippedWeapon.ammoPerShot2);
            Fire(isSecondary);
        }
        else if (playerStats.heartboundRounds > 0 && playerHealth.currentHP - percentageOfPlayerHp > 0)
        {
            playerHealth.RemoveHealth(percentageOfPlayerHp);
            Fire(isSecondary);
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

    public void Fire(bool isSecondary)
    {
        recoilScript.CallFire();
        if (isSecondary)
            equippedWeapon.TriggerItem2();
        else
            equippedWeapon.TriggerItem1();

        shootSystem.FireHomingRocket(false, equippedWeapon.barrel, 30, playerStats.fireBallChance);
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
    }

    private IEnumerator BurstFire(bool isSecondary)
    {
        currentTimer = 0;
        
        if(!isSecondary)
        {
            for (int i = 0; i < equippedWeapon.baseShootingLogic1.burstAmount; i++)
            {
                CheckForFire(isSecondary);
                yield return new WaitForSeconds(equippedWeapon.baseShootingLogic1.burstDelay);
            }
        }
        else
        {
            for (int i = 0; i < equippedWeapon.baseShootingLogic2.burstAmount; i++)
            {
                CheckForFire(isSecondary);
                yield return new WaitForSeconds(equippedWeapon.baseShootingLogic2.burstDelay);
            }
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

        reloadIcon.fillAmount = 0f;
        reloadGroup.alpha = 0;
        reloadIcon.enabled = false;
        
        StartCoroutine(CancelReload());
    }

    public IEnumerator Reloading()
    {
        if(burstFireCoroutine != null)
            StopCoroutine(burstFireCoroutine);

        reloadIcon.fillAmount = 0f;
        reloadIcon.enabled = true;
        reloadGroup.alpha = 1;
        equippedWeapon.finishedReload = false;

        while (reloadIcon.fillAmount < 1 && reloadIcon.isActiveAndEnabled)
        {
            yield return new WaitForEndOfFrame();
            float totalReloadSpeed = 1 + playerStats.reloadSpeed;
            if (playerStats.hasAlternateFastReload > 0 && playerStats.alternateFastReload)
                totalReloadSpeed += 0.75f;

            reloadIcon.fillAmount += totalReloadSpeed * (1.0f / equippedWeapon.reloadTime * Time.deltaTime);
            float fillAmountFor035 = (totalReloadSpeed / equippedWeapon.reloadTime) * 0.25f;
            float finalReloadBuffer = 1 - fillAmountFor035; 
            if (reloadIcon.fillAmount >= finalReloadBuffer)
            {
                if(!equippedWeapon.finishedReload)
                    equippedWeapon.ReloadWeapon();
            }
        }

        if(reloadIcon.fillAmount >= 1)
        {
            if (playerStats.hasAlternateFastReload > 0)
                playerStats.alternateFastReload = !playerStats.alternateFastReload;

            equippedWeapon.finishedReload = false;

            if (equippedWeapon.HasFullMagazine() || equippedWeapon.OutOfAmmo())
            {
                StartCoroutine(ReloadFinish());
            }
            else
            {
                StartCoroutine(PartialReloadFinish());
            }
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

        baseSensitivity = settings.sensitivity;
        zoomedSensitivity = settings.sensitivity * 0.7f;

        cameraMovement.sensitivity = baseSensitivity;
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
        RepeatReload();
    }

    public IEnumerator CancelReload()
    {
        reloadFinish.enabled = true;
        yield return new WaitForSeconds(0.1f);
        reloadFinish.enabled = false;
    }

    private void RepeatReload()
    {
        if (equippedWeapon.recoil.holstered && equippedWeapon.OutOfAmmo())
            return;

        if (equippedWeapon.currentMagazine < equippedWeapon.maxMagazine && !equippedWeapon.interuptReload)
        {
            StartCoroutine(Reloading());
        }
        else if (equippedWeapon.reloadType == ReloadType.ShotByShot)
        {
            StartCoroutine(Reloading());
        }
    }
}