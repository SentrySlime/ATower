using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoScript : MonoBehaviour
{
    public Sprite ammoSprite;
    public int maxMagazine;
    public int currentMagazine;
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

    public void UpdateAmmoInfo()
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

    public void FillAmmo()
    {
        currentMagazine++; 
        ammoList[currentMagazine].GetComponent<Image>().enabled = true;
    }

    public void FillAmmo(int ammoAmount)
    {
        for(int i = 0;i < ammoAmount;i++)
        {
            currentMagazine++;
            ammoList[currentMagazine].GetComponent<Image>().enabled = true;
        }
    }

    public void UseAmmo()
    {
        ammoList[currentMagazine].GetComponent<Image>().enabled = false;
        currentMagazine--;
    }

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
}