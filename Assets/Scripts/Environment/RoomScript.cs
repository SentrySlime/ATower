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

    [Tooltip("Room difficulty decides if this room is gonna spawn early or late, smaller number means earlier")]
    [Range(1, 3)] public int roomDifficultyLevel = 1;

    [Header("Connected Room")]
    public GameObject blockade;
    public Transform secretPoint;

    public GameObject roomToSpawn;

    void Start()
    {
        if(roomToSpawn)
            SpawnRoom();
    }


    void Update()
    {

    }

    private void SpawnRoom()
    {
        blockade.SetActive(false);
        Instantiate(roomToSpawn, secretPoint.transform.position, secretPoint.transform.rotation);
    }

}
