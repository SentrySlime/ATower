using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    public enum WeaponRarity
    {
        S,
        A,
        B,
        C,
        D
    }



    public WeaponRarity weaponRarity;

    [SerializeField] GameObject weaponPrefab;
    private GameObject currentWeapon;

    void Start()
    {

        //rotatingMesh = Instantiate(weaponMesh, transform.position, Quaternion.identity, transform);
        //rotatingMesh.transform.localScale = size;
    }


    void Update()
    {
        transform.Rotate(Vector3.up, 100 * Time.deltaTime);
    }

    public GameObject returnWeapon()
    {
        currentWeapon = Instantiate(weaponPrefab);
        return currentWeapon;

    }

}
