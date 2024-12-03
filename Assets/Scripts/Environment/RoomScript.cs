using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    //[Header("Room attributes")]
    public enum Direction { forward, left, right, down }

    public Direction direction;

    [Header("Room points")]
    public Transform point1;
    public Transform point2;

    void Start()
    {

    }


    void Update()
    {

    }
}
