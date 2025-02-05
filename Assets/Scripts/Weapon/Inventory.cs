using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Money")]
    public int Money;

    int weaponIndex = 0;
    private int previousIndex = 0;

    FindAndEquipWeapons findAndEquipWeapons;
    WeaponSocket weaponSocket;
    PauseMenu pauseMenu;
    //public GameObject currentlyHeldGun;

    [Header("Weapons")]
    public List<GameObject> heldWeapons = new List<GameObject>();
    [HideInInspector] public BaseWeapon baseWeapon;
    [HideInInspector] public Recoil recoil;
    [HideInInspector] public Image reloadIcon;
    

    [Header("Items")]
    public List<GameObject> heldItems = new List<GameObject>();


    // weaponIndex++
    // current weapon index = 0
    // weaponIndex--

    // Increase or decrease depending on if scrolling up or down

    //PreviousWeaponIndex = weaponIndex
    //Update weapon index

    void Start()
    {
        findAndEquipWeapons = GetComponent<FindAndEquipWeapons>();
        pauseMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PauseMenu>();
        weaponSocket = GetComponent<WeaponSocket>();
        reloadIcon = GameObject.FindGameObjectWithTag("ReloadImage").GetComponent<Image>();
        
    }


    void Update()
    {
        if (pauseMenu.paused)
            return;


        if (heldWeapons.Count == 0) { return; }
        if (recoil.isBurstFiring) { return; }


        weaponIndex += Mathf.RoundToInt(Input.mouseScrollDelta.y);
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            weaponIndex = 0;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            weaponIndex = 1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            weaponIndex = 2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            weaponIndex = 3;
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            weaponIndex = 4;
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            weaponIndex = 5;
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            weaponIndex = 6;
        }

        if (weaponIndex < 0)
            weaponIndex = heldWeapons.Count - 1;
        else if (weaponIndex >= heldWeapons.Count)
            weaponIndex = 0;

        if (previousIndex != weaponIndex)
        {
            weaponSocket.adsProgress = 1;
            weaponSocket.StopReload();
            weaponSocket.currentTimer = 0;
            reloadIcon.enabled = false;
            recoil.rotating = false;
            baseWeapon.gameObject.transform.localRotation = Quaternion.identity;

            heldWeapons[previousIndex].GetComponent<Recoil>().DisableWeapon();
            heldWeapons[weaponIndex].GetComponent<Recoil>().EnableWeapon();
            

            findAndEquipWeapons.SetWeapon(heldWeapons[weaponIndex]);
            previousIndex = weaponIndex;
        }

    }

    public void IncreaseMoney(int incomingAmount)
    {
        Money += incomingAmount;
        //Money++;
    }

    

    //Do something about longer reloads here

}

