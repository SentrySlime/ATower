using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public class LevelGeneration_2 : MonoBehaviour
{
    public List<GameObject> easyRooms = new List<GameObject>();
    public List<GameObject> mediumRooms = new List<GameObject>();
    public List<GameObject> hardRooms = new List<GameObject>();

    [Header("Rooms & corridors")]
    public List<GameObject> rooms = new List<GameObject>();
    public List<GameObject> corridors = new List<GameObject>();

    [Header("Special Rooms")]
    public GameObject startingRoom;
    public GameObject endRoom;
    public GameObject treasureRoom;
    public GameObject devilRoom;

    [Header("Room generation stats")]
    public int roomAmount = 12;

    [HideInInspector] public bool spawnedDevilRoom = false;
    [HideInInspector] public bool spawnedShopRoom = false;
    [HideInInspector] public List<GameObject> allSurfaces = new List<GameObject>();
    [HideInInspector] public GameObject spawnTransform;

    //roomCounters
    public int roomCount = 0;
    int easyRoomCount = 7;
    int mediumRoomCount = 14;
    int hardRoomCount = 9;

    bool spawnRoom = false;
    bool spawnEndRoom = true;

    GameObject previousRoom;
    GameObject previousCorridor;

    NavMeshManager navMeshManager;
    RoomManager roomManager;

    private void Awake()
    {
        navMeshManager = GetComponent<NavMeshManager>();
        roomManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<RoomManager>();
        AssignRoomsToList();
    }

    void Start()
    {
        //StartLevelGeneration();
        GenerateStartingRoom();
    }

    
    void Update()
    {
        if(roomCount < roomAmount)
        {

            if (spawnRoom)
            {
                GenerateRoom();
                roomCount++;
            }
            else
                GenerateCorridor();

            spawnRoom = !spawnRoom;
        }
        else if(spawnEndRoom)
        {
            spawnEndRoom = false;
            GenerateEndRoom();
        }
    }



    private void StartLevelGeneration()
    {
        GenerateStartingRoom();

        for (int i = 0; i < roomAmount; i++)
        {
            if (roomCount <= easyRoomCount)
            {
                GenerateEasyRoom();
                roomCount++;
            }
            if(roomCount > easyRoomCount &&  roomCount < mediumRoomCount)
            {
                GenerateMediumRoom();
                roomCount++;
            }
        }
    }

    private void GenerateRoom()
    {
        if (roomCount <= easyRoomCount)
        {
            GenerateEasyRoom();
        }
        else if (roomCount > easyRoomCount && roomCount < mediumRoomCount)
        {
            GenerateMediumRoom();
        }
        else if (spawnEndRoom)
        {
            spawnEndRoom = false;
            GenerateEndRoom();
        }

    }

    private void GenerateStartingRoom()
    {
        RoomScript newRoom = Instantiate(startingRoom, transform.position, Quaternion.identity).GetComponent<RoomScript>();
        spawnTransform = newRoom.point2.gameObject;
        if (roomManager)
            roomManager.AddRoom(newRoom);
    }

    private void GenerateEndRoom()
    {
        print("End room");

        RoomScript newRoom = Instantiate(endRoom, spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
        spawnTransform = newRoom.point2.gameObject;
        if (roomManager)
            roomManager.AddRoom(newRoom);
    }

    private void GenerateEasyRoom()
    {
        int randomIndex = Random.Range(0, easyRooms.Count);

        if(previousRoom)
        {
            if (easyRooms[randomIndex] == previousRoom)
            {
                if (randomIndex + 1 < easyRooms.Count)
                    randomIndex++;
                else
                    randomIndex = 0;
            }   
        }

        previousRoom = easyRooms[randomIndex];
        RoomScript newRoom = Instantiate(easyRooms[randomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
        newRoom.levelGeneration = this;
        spawnTransform = newRoom.point2.gameObject;
        if (roomManager)
            roomManager.AddRoom(newRoom);
    }

    private void GenerateMediumRoom()
    {
        int randomIndex = Random.Range(0, mediumRooms.Count);

        if (previousRoom)
        {
            if (mediumRooms[randomIndex] == previousRoom)
            {
                if (randomIndex + 1 < mediumRooms.Count)
                    randomIndex++;
                else
                    randomIndex = 0;
            }
        }

        previousRoom = mediumRooms[randomIndex];
        RoomScript newRoom = Instantiate(mediumRooms[randomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
        newRoom.levelGeneration = this;
        spawnTransform = newRoom.point2.gameObject;
        if (roomManager)
            roomManager.AddRoom(newRoom);
    }

    private void GenerateCorridor()
    {
        int randomIndex = Random.Range(0, corridors.Count);

        if (previousCorridor)
        {
            if (corridors[randomIndex] == previousCorridor)
            {
                if (randomIndex + 1 < corridors.Count)
                    randomIndex++;
                else
                    randomIndex = 0;
            }
        }

        previousCorridor = corridors[randomIndex];

        RoomScript newRoom = Instantiate(corridors[randomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
        newRoom.levelGeneration = this;
        spawnTransform = newRoom.point2.gameObject;
        if (roomManager)
            roomManager.AddRoom(newRoom);

    }

    private void AssignRoomsToList()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i] == null) { return; }

            RoomScript roomScript = rooms[i].GetComponent<RoomScript>();

            if (roomScript.roomDifficultyLevel == 1)
                easyRooms.Add(rooms[i]);
            if (roomScript.roomDifficultyLevel == 2)
                mediumRooms.Add(rooms[i]);
            if (roomScript.roomDifficultyLevel == 3)
                hardRooms.Add(rooms[i]);
        }
    }
}
