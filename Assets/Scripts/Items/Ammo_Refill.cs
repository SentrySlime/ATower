using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo_Refill : MonoBehaviour
{

    public GameObject FX;

    void Update()
    {
        transform.Rotate(Vector3.up, 100 * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Instantiate(FX, transform.position, Quaternion.identity);

            BaseWeapon tempWeapon = other.GetComponent<WeaponSocket>().equippedWeapon.GetComponentInChildren<BaseWeapon>();
            tempWeapon.AmmoRefill();

            Destroy(gameObject);
        }
    }

}
