using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaisingPlatforms : MonoBehaviour
{
    public bool reverse = false;
    public bool goingUp = false;

    [Header("MoveSpeed")]
    public float raiseSpeed = 1;
    public float fallSpeed = 1;

    [Header("Xtra stuff")]
    public GameObject locationObj;


    #region Hidden stats
    Rigidbody playerRB;
    Vector3 startingPos;
    Vector3 endPos;
    Rigidbody rb;
    #endregion

    void Start()
    {
        startingPos = transform.position;

        if(locationObj)
        {
            endPos = locationObj.transform.position;
            locationObj.SetActive(false);
        }

        rb = GetComponent<Rigidbody>();

        if (reverse)
            goingUp = true;
    }

    
    void Update()
    {
        //if(goingUp)
        //{
        //    if (currentSpeed < raiseSpeed)
        //        currentSpeed += Time.deltaTime * acceleration;
            
        //}
        //else if(transform.position.y < 2 && transform.position.y > -2)
        //{
        //    transform.position = startingPos;
        //}
        

        
    }

    private void FixedUpdate()
    {
        if (goingUp)
        {
           
            if (transform.position.y <= endPos.y)
            {
                rb.velocity = Vector3.up * raiseSpeed;
                rb.isKinematic = false;
            }
            else
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }


        }
        else if (!goingUp)
        {

            if (transform.position.y >= startingPos.y)
            {
                rb.velocity = -Vector3.up * fallSpeed;
                rb.isKinematic = false;
            }
            else
            {
                rb.isKinematic = true;
                rb.velocity = Vector3.zero;
            }


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!playerRB)
            playerRB = other.GetComponent<Rigidbody>();
        
        //if(playerRB)    
        //    playerRB.velocity = rb.velocity = -Vector3.up * fallSpeed;

        if (other.CompareTag("Player"))
        {
            if(!reverse)
            {
                StopAllCoroutines();
                rb.velocity = Vector3.zero;
                goingUp = true;
            }
            else
            {
                StopAllCoroutines();
                rb.velocity = Vector3.zero;
                goingUp = false;

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(!reverse)
            {
                StartCoroutine(ReverseDirection());
            }
            else
            {
                StartCoroutine(ReverseDirection());
            }
        }

    }

    IEnumerator ReverseDirection()
    {
        if(!reverse)
        { 
            yield return new WaitForSeconds(0.5f);
            rb.isKinematic = false;
            goingUp = false;
        }
        else
        {
            yield return new WaitForSeconds(2f);
            rb.isKinematic = false;
            goingUp = true;
        }

    }

}
