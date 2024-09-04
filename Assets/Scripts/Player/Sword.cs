using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [Header("RaycastStuff")]
    int damage = 5;

    [Header("RaycastStuff")]
    public float raycastLength = 1;
    public LayerMask layerMask;
    public float rayCastDuration = 0.1f;
    public float rayCastTimer = 0;


    [Header("MoveAmount")]
    public int recoilAmount;
    public float moveAmount;

    [Header("Rotation durations")]
    public float lerpDuration = 0.05f;
    public float lerpBackDuration = 0.5f;

    Quaternion startRotation;
    Vector3 startPos;

    Quaternion targetRotation;
    public Vector3 targetPosition;

    bool rotating;
    bool rotatingThere;

    float timeElapsed = 0;
    float timeElapsed2 = 0;

    void Start()
    {
        InitializeRecoil();   
    }

    
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !rotating)
            Fire();

        if (rotating)
        {
            RecoilFunc();
            
        }

        if(rotatingThere)
        {
            if (rayCastTimer < rayCastDuration)
            {
                rayCastTimer += Time.deltaTime;
            }
            else
            {
                DamageCalculation();
                rayCastTimer = 0;
            }
        }
        

    }

    public void Fire()
    {
        timeElapsed = 0;
        timeElapsed2 = 0;
        rotating = true;
        rotatingThere = true;
        //if (weaponSocket.equippedWeapon.currentMagazine > 0 && !weaponSocket.reloadIcon.isActiveAndEnabled)
        //{
        //}


    }

    public void RecoilFunc()
    {
        targetRotation = startRotation * Quaternion.Euler(0, -recoilAmount, 0);

        //targetPosition = new Vector3(startPos.x, startPos.y, startPos.z + moveAmount);
        targetPosition = new Vector3(startPos.x + moveAmount, startPos.y , startPos.z );

        if (timeElapsed < lerpDuration)
        {
            //Rotating back because of the recoil
            transform.localRotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);

            //Moving because of the recoil
            transform.localPosition = Vector3.Slerp(startPos, targetPosition, timeElapsed / lerpDuration);

            timeElapsed += Time.deltaTime;

        }
        else
        {
            rotatingThere = false;
            ReturnFunc();
        }
    }

    public void ReturnFunc()
    {


        if (timeElapsed2 < lerpBackDuration)
        {

            //Rotatin back after the recoil
            transform.localRotation = Quaternion.Lerp(targetRotation, startRotation, timeElapsed2 / lerpBackDuration);

            //Moving back after the recoil
            transform.localPosition = Vector3.Lerp(targetPosition, startPos, timeElapsed2 / lerpDuration);

            timeElapsed2 += Time.deltaTime;

        }
        else
        {
            rotating = false;

        }
    }

    public void InitializeRecoil()
    {
     
        startRotation = transform.localRotation;
        startPos = transform.localPosition;

    }

    private void DamageCalculation()
    {
        RaycastHit hit;
        Debug.DrawLine(transform.position, transform.position +  transform.forward * raycastLength, Color.red, 1);
        if (Physics.Raycast(transform.position, transform.forward * raycastLength,  out hit, raycastLength, ~layerMask))
        {
            if (hit.transform.CompareTag("Enemy"))
                hit.transform.gameObject.GetComponent<Health>().TakeDamage(damage);
                //Destroy(hit.transform.gameObject);
        }
    }

}
