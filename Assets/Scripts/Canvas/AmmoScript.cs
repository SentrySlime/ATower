using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoScript : MonoBehaviour
{
    //    Handgun,
    //    SMG,
    //    AssaultRifle,
    //    LMG,
    //    Shotgun,
    //    Sniper,
    //    Arrow,

    [Header("Pistol")]
    public Sprite pistolSprite;
    Vector2 pistolCellSize = new Vector2(50, 25);
    Vector2 pistolSpacing = new Vector2(0, -10);

    [Header("Rifle")]
    public Sprite rifleSprite;
    Vector2 rifleCellSize = new Vector2(37.5f, 18.75f);
    Vector2 rifleSpacing = new Vector2(0, -7);

    [Header("Shotgun")]
    public Sprite shotgunSprite;
    Vector2 shotgunCellSize = new Vector2(70f, 35f);
    Vector2 shotgunSpacing = new Vector2(0, -13.5f);

    [Header("Arrow")]
    public Sprite arrowSprite;
    Vector2 arrowCellSize = new Vector2(70f, 35f);
    Vector2 arrowSpacing = new Vector2(0, -10);

    [Header("Explosive")]
    public Sprite explosiveSprite;
    Vector2 explosiveCellSize = new Vector2(70f, 35f);
    Vector2 explosiveSpacing = new Vector2(0, -10);

    [Header("Energy")]
    public Sprite energySprite;
    Vector2 energyCellSize = new Vector2(90f, 45f);
    Vector2 energySpacing = new Vector2(0, -19);

    [Header("Reference")]
    public Sprite ammoSprite;
    Vector2 cellSize;
    float cellSize_x = 100;
    float cellSize_y = 100;
    float spacing = 0;
    
    public GridLayoutGroup gridLayoutGroup;
    [HideInInspector] public int maxMagazine;
    [HideInInspector] public int currentMagazine;
    public List<GameObject> ammoList;



    public int currentAmmo;

    void Start()
    {
        //SetMagazineSize(10);
        //UpdateAmmoInfo(10);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            UseAmmo();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            FillAmmo();
        }
    }


    //Triggers to set based on current ammo in magazine
    public void SetcurrentMagAmount()
    {
        
        for (int i = 0; i < ammoList.Count; i++)
        {
            if (i == ammoList.Count - 1)
            {
                return;
            }

            if (i <= currentMagazine)
            {
                //ammoList[i].GetComponent<Image>().sprite = ammoSprite;
                ammoList[i].GetComponent<Image>().enabled = true;
            }
            else
            {
                ammoList[i].GetComponent<Image>().enabled = false;
                //ammoList[i].gameObject.SetActive(false);
            }
        }
        
        
    }
    
    //Triggers to fill with just a single round
    public void FillAmmo()
    {
        currentMagazine++; 
        ammoList[currentMagazine].GetComponent<Image>().enabled = true;
    }

    //Triggers to fill with a certain amount, like shot by shot reload
    public void FillAmmo(int ammoAmount)
    {
        for(int i = 0;i < ammoAmount;i++)
        {
            currentMagazine++;
            ammoList[currentMagazine].GetComponent<Image>().enabled = true;
        }
    }

    //Fires when you shoot ammo
    public void UseAmmo()
    {
        ammoList[currentMagazine].GetComponent<Image>().enabled = false;
        currentMagazine--;
    }

    //Trigggers to refill the entier magazine
    public void RefillMagazine()
    {
        for (int i = 0; i < ammoList.Count; i++)
        {
            if (i == ammoList.Count - 1)
            {
                currentMagazine = maxMagazine - 1;
                return;
            }
            ammoList[i].GetComponent<Image>().enabled = true;
        }
    }

    //Trigggers to the magazine based on amount
    public void RefillMagazineAmount(int amount)
    {
        for (int i = 0; i < currentMagazine + amount; i++)
        {
            if (i == maxMagazine - 1)
            {
                currentMagazine += amount;
                return;
            }
            ammoList[i].GetComponent<Image>().enabled = true;
        }

        currentMagazine += amount;
    }

    //Set the max size of the magazine
    public void SetMagazineSize()
    {
        for (int i = 0; i < ammoList.Count; i++)
        {
            if (i == ammoList.Count - 1)
            {
                return;
            }

            if (i >= maxMagazine)
            {
                ammoList[i].SetActive(false);
            }
            else
            {
                ammoList[i].SetActive(true);
            }
        }
    }

    //Set the max size of the magazine
    public void UpdateAmmoInfo()
    {
        

        for (int i = 0; i < ammoList.Count; i++)
        {
            if (i == ammoList.Count - 1)
            {
                return;
            }

            
            if (i >= maxMagazine)
            {
                ammoList[i].SetActive(false);
            }
            else
            {
                if (i <= currentMagazine)
                {
                    ammoList[i].GetComponent<Image>().enabled = true;
                }
                else
                {
                    ammoList[i].GetComponent<Image>().enabled = false;
                }

                ammoList[i].SetActive(true);
                ammoList[i].GetComponent<Image>().sprite = ammoSprite;
            }
        }
    }

    public void SetAmmoType(BaseWeapon.WeaponType weaponType)
    {
        if (weaponType == BaseWeapon.WeaponType.handgun)
            SetCellsAndSpacing(pistolSprite, pistolCellSize, pistolSpacing);
        if (weaponType == BaseWeapon.WeaponType.AssaultRifle)
            SetCellsAndSpacing(rifleSprite, rifleCellSize, rifleSpacing);
        if (weaponType == BaseWeapon.WeaponType.Shotgun)
            SetCellsAndSpacing(shotgunSprite, shotgunCellSize, shotgunSpacing);
        if (weaponType == BaseWeapon.WeaponType.Arrow)
            SetCellsAndSpacing(arrowSprite, arrowCellSize, arrowSpacing);
        if (weaponType == BaseWeapon.WeaponType.Explosive)
            SetCellsAndSpacing(explosiveSprite, explosiveCellSize, explosiveSpacing);
        if (weaponType == BaseWeapon.WeaponType.Energy)
            SetCellsAndSpacing(energySprite, energyCellSize, energySpacing);
    }

    private void SetCellsAndSpacing(Sprite sprite, Vector2 cellsize, Vector2 spacing)
    {
        ammoSprite = sprite;
        gridLayoutGroup.cellSize = cellsize;
        gridLayoutGroup.spacing = spacing;
    }
}