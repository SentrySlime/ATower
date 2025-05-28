using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//for Better recoil
//https://www.youtube.com/watch?v=geieixA4Mqc&ab_channel=Gilbert

public class Recoil : MonoBehaviour
{
    public bool IUsed = false;
    public bool holstered = false;
    public GameObject obj;
    public BaseWeapon baseWeapon;

    public int recoilAmount;
    public float moveAmount;

    [Header("Rotation durations")]
    public float lerpDuration = 0.05f;
    public float lerpBackDuration = 0.5f;

    [SerializeField] public bool rotating;
    bool moving;
    bool hasSetRecoil = false;

    Quaternion startRotation;
    Vector3 startPos;

    Quaternion targetRotation;
    public Vector3 targetPosition;

    float timeElapsed = 0;
    float timeElapsed2 = 0;

    public float reloadTimer = 0;
    private float maxTimer = 0.08f;
    private float currentTimer = 0;
    public bool isBurstFiring = false;

    WeaponSocket weaponSocket;
    PlayerStats playerStats;

    bool reloading = false;

    private void Awake()
    {
        //obj = GetComponentInChildren<GameObject>();
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        weaponSocket = player.GetComponent<WeaponSocket>();
        playerStats = player.GetComponent<PlayerStats>();
        baseWeapon = obj.GetComponent<BaseWeapon>();
    }

    void Update()
    {


        if (holstered && !baseWeapon.HasFullMagazine())
            HolsteredReload();
        else if(reloadTimer != 0)
            reloadTimer = 0;

        if (currentTimer < maxTimer)
        {
            currentTimer += Time.deltaTime;
        }


        if (rotating)
            RecoilFunc();
    }

    public void CallFire()
    {
        if (IUsed && currentTimer >= maxTimer)
        {
            Fire();
            currentTimer = 0;
        }
    }

    public void Fire()
    {
        timeElapsed = 0;
        timeElapsed2 = 0;
        rotating = true;
        //if (weaponSocket.equippedWeapon.currentMagazine > 0 || playerStats.heartboundRounds > 0)
        //{
        //    if(!weaponSocket.reloadIcon.isActiveAndEnabled)
        //    {
        //    }
        //}
    }

    public void RecoilFunc()
    {
        targetRotation = startRotation * Quaternion.Euler(recoilAmount, 0, 0);

        targetPosition = new Vector3(startPos.x, startPos.y, startPos.z + moveAmount);

        if (timeElapsed < lerpDuration)
        {
            //Rotating back because of the recoil
            obj.transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);

            //Moving because of the recoil
            transform.localPosition = Vector3.Slerp(startPos, targetPosition, timeElapsed / lerpDuration);

            timeElapsed += Time.deltaTime;

        }
        else
        {
            ReturnFunc();
        }
    }

    public void ReturnFunc()
    {

        if (timeElapsed2 < lerpBackDuration)
        {

            //Rotatin back after the recoil
            obj.transform.localRotation = Quaternion.Lerp(targetRotation, startRotation, timeElapsed2 / lerpBackDuration);

            //Moving back after the recoil
            transform.localPosition = Vector3.Lerp(targetPosition, startPos, timeElapsed2 / lerpDuration);

            timeElapsed2 += Time.deltaTime;

        }
        else
        {
            rotating = false;

        }
    }

    public void InitializeRecoil(float move, int angle, float fireRate)
    {
        if (hasSetRecoil) return;

        hasSetRecoil = true;

        recoilAmount = angle;
        moveAmount = move;
        maxTimer = fireRate;

        startRotation = obj.transform.localRotation;
        startPos = transform.localPosition;
    }

    public void HolsteredReload()
    {
        if (baseWeapon.currentAmmo == 0 && !baseWeapon.infinteAmmo) { return; }

        if (baseWeapon.reloadType == BaseWeapon.ReloadType.Magazine)
        {

            if(!reloading)
                baseWeapon.iconPrefab.GetComponent<WeaponIcon>().DisplayFinishedReload();

            reloading = true;
            MagazineBasedReload();
        }
        else if (baseWeapon.reloadType == BaseWeapon.ReloadType.ShotByShot)
        {
            if (!reloading)
                baseWeapon.iconPrefab.GetComponent<WeaponIcon>().DisplayFinishedReload();

            reloading = true;
            SingleBasedReload();
        }
    }
    private void MagazineBasedReload()
    {
        if (reloadTimer < baseWeapon.reloadTime * 1.5)
        {
            reloadTimer += Time.deltaTime;
        }
        else
        {
            reloadTimer = 0;
            baseWeapon.ReloadWeapon();
            reloading = false;
            //baseWeapon.currentMagazine = baseWeapon.maxMagazine;
        }

    }

    private void SingleBasedReload()
    {
        if (reloadTimer < baseWeapon.reloadTime * 1.5)
        {
            reloadTimer += Time.deltaTime;
        }
        else
        {
            reloadTimer = 0;
            baseWeapon.ReloadWeapon();
            reloading = false;
            //baseWeapon.currentMagazine += baseWeapon.reloadAmount;
        }

    }


    public void DisableWeapon()
    {
        holstered = true;
        obj.SetActive(false);
    }

    public void EnableWeapon()
    {
        reloading = false;
        holstered = false;
        baseWeapon.iconPrefab.GetComponent<WeaponIcon>().DisableReloadIcon();
        obj.SetActive(true);
    }
}

