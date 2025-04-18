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

    public enum FireMode { fullAuto, semi, burst };
    public FireMode fireMode;

    [SerializeField] public bool rotating;
    bool moving;

    Quaternion startRotation;
    Vector3 startPos;

    Quaternion targetRotation;
    public Vector3 targetPosition;

    float timeElapsed = 0;
    float timeElapsed2 = 0;

    public float reloadTimer = 0;
    private float maxTimer = 0.08f;
    private float currentTimer = 0;
    private float burstDelay = .1f;
    public bool isBurstFiring = false;

    WeaponSocket weaponSocket;

    bool reloading = false;

    private void Awake()
    {
        //obj = GetComponentInChildren<GameObject>();
    }

    private void Start()
    {
        weaponSocket = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponSocket>();
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
            //if (fireMode == FireMode.fullAuto)
            //{
            //        Fire();


            //}
            //else if (fireMode == FireMode.semi)
            //{
            //        currentTimer = 0;



            //}
            //else if (fireMode == FireMode.burst)
            //{
            //        StartCoroutine(BurstFire());

            //}
        }
    }

    public void Fire()
    {
        if (weaponSocket.equippedWeapon.currentMagazine > 0 && !weaponSocket.reloadIcon.isActiveAndEnabled)
        {
            timeElapsed = 0;
            timeElapsed2 = 0;
            rotating = true;
        }


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

    public void InitializeRecoil(float move, int angle, float fireRate, int firingMode)
    {
        recoilAmount = angle;
        moveAmount = move;
        maxTimer = fireRate;

        startRotation = obj.transform.localRotation;
        startPos = transform.localPosition;

        if (firingMode == 0)
            fireMode = FireMode.fullAuto;
        else if (firingMode == 1)
            fireMode = FireMode.semi;
        if (firingMode == 2)
            fireMode = FireMode.burst;

    }

    private IEnumerator BurstFire()
    {
        isBurstFiring = true;
        currentTimer = 0;
        for (int i = 0; i < weaponSocket.equippedWeapon.burstAmount; i++)
        {
            Fire();
            yield return new WaitForSeconds(weaponSocket.equippedWeapon.burstDelay);
        }

        isBurstFiring = false;
        yield return null;
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
        if (reloadTimer < baseWeapon.reloadTime)
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

