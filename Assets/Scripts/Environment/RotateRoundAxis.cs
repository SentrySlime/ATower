using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRoundAxis : MonoBehaviour
{
    public float rotateX = 0;
    public float rotateY = 0;
    public float rotateZ = 0;


    void Start()
    {
        
    }

    
    void Update()
    {

        transform.Rotate(Vector3.forward, rotateX);
        transform.Rotate(Vector3.up, rotateY);
        transform.Rotate(Vector3.right, rotateZ);

    }
}
