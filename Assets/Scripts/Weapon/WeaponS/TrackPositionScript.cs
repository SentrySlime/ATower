using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackPositionScript : MonoBehaviour
{

    public GameObject objToTrack;

    void Start()
    {

    }


    void Update()
    {
        transform.position = objToTrack.transform.position;

    }
}
