using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ghoulSpeed : MonoBehaviour
{
    public Animation animation;
    void Start()
    {
        animation["Walk"].speed = 1.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
