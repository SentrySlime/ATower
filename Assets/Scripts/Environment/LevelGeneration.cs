using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGeneration : MonoBehaviour
{
    
    [SerializeField]
    NavMeshSurface[] navMeshSurfaces;

    public NavMeshSurface surfaCE;

    public List<GameObject> rooms = new List<GameObject>();
    public List<GameObject> corridors = new List<GameObject>();
    GameObject spawnTransform;
    int allRoomCount;
    bool currentRoom = false;

    [Header("RoomLogic")]
    public bool removeRooms = true;
    public int roomCount = 0;


    [Header("Timer")]
    public bool hasTimer = false;
    public float rate1 = 1.5f;
    float timer1 = 0;

    public List<GameObject> allSurfaces = new List<GameObject>();
    //public List<> allSurfaces = new List<GameObject>();

    NavMeshManager navMeshManager;

    bool hasBuilt = false;

    void Start()
    {
        navMeshManager = GetComponent<NavMeshManager>();
        allRoomCount = rooms.Count + corridors.Count;
    }


    void Update()
    {
        
        if (roomCount < allRoomCount)
        {
            if(!hasTimer)
            {
                GenerateLevel();
                return;
            }

            if (timer1 < rate1)
            {
                timer1 += Time.deltaTime;
            }
            else
            {
                GenerateLevel();
                timer1 = 0;
            }
        }
        else if(!hasBuilt)
        {
            surfaCE.BuildNavMesh();
            hasBuilt = true;    

        }

    }

    private int GetRandomRoom()
    {
        return Random.Range(0, rooms.Count);
    }

    private int GetRandomCorridor()
    {
        return Random.Range(0, corridors.Count);
    }

    private void GenerateLevel()
    {
        if(currentRoom)
        {
            GenerateRoom();
            currentRoom = false;
        }
        else
        {
            GenerateCorridor();
            currentRoom = true;
        }

        
    }

    private void GenerateRoom()
    {
        int roomIndex = GetRandomRoom();

        //GetChildren(newNumber);
        if (roomCount == 0)
        {
            RoomScript newRoom = Instantiate(rooms[roomIndex], transform.position, Quaternion.identity).GetComponent<RoomScript>();
            spawnTransform = newRoom.point2.gameObject;
        }
        else
        {

            RoomScript newRoom = Instantiate(rooms[roomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
            spawnTransform = newRoom.point2.gameObject;
        }

        if (removeRooms)
            rooms.RemoveAt(roomIndex);

        roomCount++;
    }

    private void GenerateCorridor()
    {
        int roomIndex = GetRandomRoom();

        //GetChildren(newNumber);
        if (roomCount == 0)
        {
            RoomScript newRoom = Instantiate(corridors[roomIndex], transform.position, Quaternion.identity).GetComponent<RoomScript>();
            spawnTransform = newRoom.point2.gameObject;
        }
        else
        {

            RoomScript newRoom = Instantiate(corridors[roomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
            spawnTransform = newRoom.point2.gameObject;
        }

        if (removeRooms)
            corridors.RemoveAt(roomIndex);

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
        //allSurfaces[0].GetComponent<Material>().color = Color.red;
        navMeshManager.NavMeshBuilder(allSurfaces[0].GetComponent<NavMeshSurface>());
        //Generate navmesh here
        //for (int i = 0; i < allSurfaces.Count; i++)
        //{
        //    // build navmesh here
        //    navMeshManager.NavMeshBuilder(allSurfaces[i].GetComponent<NavMeshSurface>());
        //}

    }

}
