using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    public Vector3 screenShakeAmount;
    public GameObject leftDoor;
    public GameObject rightDoor;

    public AudioSource audioSource;

    private bool openDoors = false;

    private bool canInvoke = true;

    ScreenShake screenShake;

    bool triggered = false;

    void Start()
    {
        screenShake = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<ScreenShake>();
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if(triggered) { return; }

        triggered = true;

        if(!openDoors)
            openDoors = true;
            audioSource.Play();


    }

    void Update()
    {
        if(Time.timeScale == 0 || !openDoors) { return; }

        if(canInvoke)
        {
            InvokeRepeating("ScreenShake", 0.1f, 0.2f); 
            Invoke("StopDoor", 7);
            canInvoke = false;
        }

        if (leftDoor)
            leftDoor.transform.Rotate(Vector3.up, 0.1f);

        if (rightDoor)
            rightDoor.transform.Rotate(Vector3.up, -0.1f);
    }

    private void StopDoor()
    {
        openDoors = false; 
    }

    private void ScreenShake()
    {


        if(openDoors == true) 
            screenShake.Screenshake(screenShakeAmount.x, screenShakeAmount.y, screenShakeAmount.z);
    }
}
