using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGeneration : MonoBehaviour
{

    public List<GameObject> rooms = new List<GameObject>();
    GameObject spawnTransform;
    int allRoomCount;

    public int roomCount = 0;

    public float rate1 = 1.5f;
    public float timer1 = 0;

    public List<GameObject> allSurfaces = new List<GameObject>();
    //public List<> allSurfaces = new List<GameObject>();

    void Start()
    {
        allRoomCount = rooms.Count;
    }


    void Update()
    {
        if (timer1 < rate1)
        {
            timer1 += Time.deltaTime;
        }
        else
        {
            if (roomCount < allRoomCount)
            {
                GenerateLevel();
                timer1 = 0;
            }
            else
            {

            }


        }

    }

    private void GenerateLevel()
    {
        int newNumber = Random.Range(0, rooms.Count);
        GetChildren(newNumber);
        if (roomCount == 0)
        {

            RoomScript newRoom = Instantiate(rooms[newNumber], transform.position, Quaternion.identity).GetComponent<RoomScript>();
            spawnTransform = newRoom.point2.gameObject;
        }
        else
        {

            RoomScript newRoom = Instantiate(rooms[newNumber], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
            spawnTransform = newRoom.point2.gameObject;
        }

        rooms.RemoveAt(newNumber);
        roomCount++;
    }

    private void GetChildren(int newNumber)
    {
        int children = rooms[newNumber].transform.childCount;
        for (int i = 0; i < children; i++)
        {
            allSurfaces.Add(rooms[newNumber].transform.GetChild(i).gameObject);
        }
    }

    private void GenerateNavMesh()
    {
        //Generate navmesh here

    }

}
