using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponManager : MonoBehaviour
{
    GameObject player;

    [SerializeField] private List<GameObject> weapons = new List<GameObject>();
    [SerializeField] private List<WeaponPickUp> pickUps = new List<WeaponPickUp>();

    private List<GameObject> shuffledWeapons = new List<GameObject>();
    private int currentIndex = 0;

    public List<GameObject> s_Tier = new List<GameObject>();
    public List<GameObject> a_Tier = new List<GameObject>();
    public List<GameObject> b_Tier = new List<GameObject>();
    public List<GameObject> c_Tier = new List<GameObject>();
    public List<GameObject> d_Tier = new List<GameObject>();

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //SortRarity();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.U))
        {
            if (weapons == null || weapons.Count == 0)
            {
                return;
            }
            DropWeapon(player.transform.position);
        }
    }

    public void DropWeapon(Vector3 spawnPos)
    {

        //GameObject weaponToDrop = GetRandomWeapon();
        GameObject weaponToDrop = GetShuffledWeapon();
        Instantiate(weaponToDrop, spawnPos, Quaternion.identity);
    }

    private GameObject GetRandomWeapon()
    {
        int weaponIndex = Random.Range(0, weapons.Count);
        GameObject weapon = weapons[weaponIndex];
        weapons.RemoveAt(weaponIndex);
        return weapon;
    }

    public GameObject GetWeaponForShop()
    {
        GameObject weaponToDrop = GetShuffledWeapon();
        return weaponToDrop;
    }

    //public GameObject GetRandomWeapon(bool S, bool A, bool B, bool C, bool D)
    //{

    //    List<GameObject> weaponList = new List<GameObject>();

    //    if (S)
    //        weaponList.AddRange(s_Tier);

    //    if (A)
    //        weaponList.AddRange(a_Tier);

    //    if (B)
    //        weaponList.AddRange(b_Tier);

    //    if (C)
    //        weaponList.AddRange(c_Tier);

    //    if (D)
    //        weaponList.AddRange(d_Tier);



    //    if (weaponList.Count != 0)
    //    {
    //        int randomIndex = Random.Range(0, weaponList.Count - 1);

    //        GameObject tempObj = weaponList[randomIndex];

    //        weapons.Remove(tempObj);

    //        if (S)
    //            s_Tier.Remove(tempObj);

    //        if (A)
    //            a_Tier.Remove(tempObj);

    //        if (B)
    //            b_Tier.Remove(tempObj);

    //        if (C)
    //            c_Tier.Remove(tempObj);

    //        if (D)
    //            d_Tier.Remove(tempObj);

    //        return tempObj;

    //    }


    //    return null;
    //}

    //------------------------------------------------------------

    //public GameObject ChoseRandomWeapon()
    //{
    //    List<WeaponPickUp> newWeaponList = new List<WeaponPickUp>();
    //    for (int i = 0; i < pickUps.Count - 1; i++)
    //    {
    //        if(pickUps[i].weaponRarity == WeaponPickUp.WeaponRarity.S && )

    //    }
    //}

    //public GameObject GetRandomWeapon(bool S, bool A, bool B, bool C, bool D)
    //{

    //    List<GameObject> weaponList = new List<GameObject>();

    //    if(S)
    //        weaponList.AddRange(s_Tier);

    //    if(A)
    //        weaponList.AddRange(a_Tier);

    //    if (B)
    //        weaponList.AddRange(b_Tier);

    //    if (C)
    //        weaponList.AddRange(c_Tier);

    //    if (D)
    //        weaponList.AddRange(d_Tier);



    //    if (weaponList.Count != 0)
    //    {
    //        int randomIndex = Random.Range(0, weaponList.Count - 1);

    //        GameObject tempObj = weaponList[randomIndex];

    //        weapons.Remove(tempObj);

    //        RemoveLists();
    //        SortRarity();

    //        return tempObj;

    //    }

    //    return null;
    //}

    private void SortRarity()
    {
        for (int i = 0; i < weapons.Count; i++)
        {
            if (weapons[i].GetComponent<WeaponPickUp>().weaponRarity == WeaponPickUp.WeaponRarity.S)
                s_Tier.Add(weapons[i]);
            else if (weapons[i].GetComponent<WeaponPickUp>().weaponRarity == WeaponPickUp.WeaponRarity.A)
                a_Tier.Add(weapons[i]);
            else if (weapons[i].GetComponent<WeaponPickUp>().weaponRarity == WeaponPickUp.WeaponRarity.B)
                b_Tier.Add(weapons[i]);
            else if (weapons[i].GetComponent<WeaponPickUp>().weaponRarity == WeaponPickUp.WeaponRarity.C)
                c_Tier.Add(weapons[i]);
            else if (weapons[i].GetComponent<WeaponPickUp>().weaponRarity == WeaponPickUp.WeaponRarity.D)
                d_Tier.Add(weapons[i]);
        }
    }

    private void RemoveLists()
    {

        s_Tier.Clear();
        a_Tier.Clear();
        b_Tier.Clear();
        c_Tier.Clear();
        d_Tier.Clear();

    }

    private void ShuffleWeapons()
    {
        shuffledWeapons = new List<GameObject>(weapons);
        for (int i = 0; i < shuffledWeapons.Count; i++)
        {
            int rnd = Random.Range(i, shuffledWeapons.Count);
            var temp = shuffledWeapons[i];
            shuffledWeapons[i] = shuffledWeapons[rnd];
            shuffledWeapons[rnd] = temp;
        }
    }

    private GameObject GetShuffledWeapon()
    {
        if (currentIndex >= shuffledWeapons.Count)
        {
            ShuffleWeapons();
            currentIndex = 0;
        }

        GameObject weapon = shuffledWeapons[currentIndex];
        currentIndex++;
        weapons.Remove(weapon);
        RemoveLists();
        //SortRarity();

        return weapon;
    }
}
