using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float destroyTimer = 0;
    float destroyTime = 0;

    void Start()
    {

        
    }

    
    void Update()
    {
        if (destroyTime < destroyTimer)
            destroyTime += Time.deltaTime;
        else
            Destroy(this);
        
    }

    

}