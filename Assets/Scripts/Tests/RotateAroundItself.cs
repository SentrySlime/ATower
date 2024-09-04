using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundItself : MonoBehaviour
{

    public float XangleRotation = 1;
    public float YangleRotation = 1;
    public float ZangleRotation = 1;

    void Start()
    {
        
    }

    
    void Update()
    {
        transform.Rotate(XangleRotation, YangleRotation, ZangleRotation);
        
    }
}
