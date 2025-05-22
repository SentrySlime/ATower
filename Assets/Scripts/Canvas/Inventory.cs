using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Money")]
    public int money;
    public TextMeshProUGUI moneyText;
    public GameObject[] moneyText2;

    [Header("Weapon")]
    public int weaponIndex = 0;
    public int previousIndex = 0;

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

    void Start()
    {
        findAndEquipWeapons = GetComponent<FindAndEquipWeapons>();
        pauseMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PauseMenu>();
        weaponSocket = GetComponent<WeaponSocket>();
        reloadIcon = GameObject.FindGameObjectWithTag("ReloadImage").GetComponent<Image>();
        moneyText = GameObject.FindGameObjectWithTag("MoneyText").GetComponent<TextMeshProUGUI>();
        moneyText2 = GameObject.FindGameObjectsWithTag("MoneyText");
        UpdateMoneyText();

    }

    void Update()
    {
        if (pauseMenu.paused)
            return;


        if (heldWeapons.Count == 0) { return; }
        if (recoil.isBurstFiring) { return; }


        weaponIndex -= Mathf.RoundToInt(Input.mouseScrollDelta.y);

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

        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            weaponIndex = 7;
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            weaponIndex = 8;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            weaponIndex = 9;
        }

        if (weaponIndex < 0)
            weaponIndex = heldWeapons.Count - 1;
        else if (weaponIndex >= heldWeapons.Count)
            weaponIndex = 0;

        if (previousIndex != weaponIndex)
        {
            SwitchWeapon();
        }

    }

    public void IncreaseMoney(int incomingAmount)
    {
        money += incomingAmount;
        UpdateMoneyText();
    }

    public void DecreaseMoney(int incomingAmount)
    {
        money -= incomingAmount;
        money = Mathf.Clamp(money, 0, 99999999);
        UpdateMoneyText();
    }

    private void UpdateMoneyText()
    {
        for (int i = 0; i < moneyText2.Length; i++)
        {
            if (moneyText2[i])
                moneyText2[i].GetComponent<TextMeshProUGUI>().text = money.ToString();
        }

        //moneyText.text = money.ToString();
    }

    public void SwitchWeapon()
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



    //Do something about longer reloads here

}

