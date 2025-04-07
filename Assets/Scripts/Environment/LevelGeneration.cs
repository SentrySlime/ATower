using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField] NavMeshSurface[] navMeshSurfaces;

    public NavMeshSurface surfaCE;
    
    [Header("Rooms")]
    public List<GameObject> rooms = new List<GameObject>();
    public List<GameObject> corridors = new List<GameObject>();
    [Header("Special Rooms")]
    public GameObject startingRoom;
    public GameObject endRoom;
    public GameObject treasureRoom;

    GameObject spawnTransform;
    int allRoomCount;
    bool currentRoom = true;

    [Header("RoomLogic")]
    public int amountOfRoomsToSpawn = 28;
    public bool removeRooms = true;
    public bool removeCorridor = true;
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
        allRoomCount = rooms.Count + corridors.Count + 1;
        if (!removeRooms)
            allRoomCount = amountOfRoomsToSpawn;
    }


    void Update()
    {
        
        if (roomCount < allRoomCount)
        {
            if(rooms.Count == 0) { return; }

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
            GenerateEndRoom();
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

    private void GenerateTreasureRoom()
    {
        RoomScript newRoom = Instantiate(treasureRoom, spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
        spawnTransform = newRoom.point2.gameObject;
    }

    private void GenerateEndRoom()
    {
        RoomScript newRoom = Instantiate(endRoom, spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
        spawnTransform = newRoom.point2.gameObject;
    }

    private void GenerateStartingRoom()
    {
        RoomScript newRoom = Instantiate(startingRoom, transform.position, Quaternion.identity).GetComponent<RoomScript>();
        spawnTransform = newRoom.point2.gameObject;
    }

    private void GenerateRoom()
    {
        int roomIndex = GetRandomRoom();

        //GetChildren(newNumber);
        if (roomCount == 0)
        {
            GenerateStartingRoom();
            //RoomScript newRoom = Instantiate(rooms[roomIndex], transform.position, Quaternion.identity).GetComponent<RoomScript>();
            //spawnTransform = newRoom.point2.gameObject;
        }
        else if(roomCount == 6)
        {
            GenerateTreasureRoom();
        }
        else if (roomCount == 12)
        {
            
            GenerateTreasureRoom();
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

        if (removeCorridor)
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
