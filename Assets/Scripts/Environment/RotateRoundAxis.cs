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

        transform.Rotate(Vector3.forward, rotateX * Time.deltaTime);
        transform.Rotate(Vector3.up, rotateY * Time.deltaTime);
        transform.Rotate(Vector3.right, rotateZ * Time.deltaTime);

    }
}
