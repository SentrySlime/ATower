using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    
    void Start()
    {

    }

    void Update()
    {

        if(Input.GetKey(KeyCode.Space))
            Application.targetFrameRate = 300;

        
    }
}
