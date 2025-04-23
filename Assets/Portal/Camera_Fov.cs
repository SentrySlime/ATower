using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Fov : MonoBehaviour
{
    Camera thisCamera;
    Camera mainCamera;

    void Start()
    {
        thisCamera = GetComponent<Camera>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        thisCamera.fieldOfView = mainCamera.fieldOfView;
    }
}
