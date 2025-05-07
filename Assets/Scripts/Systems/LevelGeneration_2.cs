using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public class LevelGeneration_2 : MonoBehaviour
{
    [Header("Navmesh")]
    public NavMeshSurface surface_1;
    public NavMeshSurface surface_2;
    [SerializeField] NavMeshSurface[] navMeshSurfaces;


    List<GameObject> easyRooms = new List<GameObject>();
    List<GameObject> mediumRooms = new List<GameObject>();
    List<GameObject> hardRooms = new List<GameObject>();

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

    [HideInInspector] public bool spawnedTreasureRoom = false;
    [HideInInspector] public bool spawnedDevilRoom = false;
    [HideInInspector] public bool spawnedShopRoom = false;
    [HideInInspector] public List<GameObject> allSurfaces = new List<GameObject>();
    [HideInInspector] public GameObject spawnTransform;

    //roomCounters
    public int roomCount1 = 0;
    public int roomCount2 = 0;
    public int roomCount3 = 0;
    int easyRoomCount = 3;
    int mediumRoomCount = 2;
    int hardRoomCount = 2;

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
        roomAmount = easyRoomCount + mediumRoomCount + hardRoomCount;

        //StartLevelGeneration();
        GenerateStartingRoom();
    }

    
    void Update()
    {
        if(roomCount3 < 3)
        {
            if (spawnRoom)
            {
                GenerateRoom();
            }
            else
                GenerateCorridor();

            spawnRoom = !spawnRoom;

        }
        else if(spawnEndRoom)
        {
            spawnEndRoom = false;
            GenerateEndRoom();
            GenerateNavMesh();
        }
    }

    private void GenerateRoom()
    {


        if (roomCount1 <= easyRoomCount)
        {
            GenerateEasyRoom();
            roomCount1++;
        }
        else if (roomCount2 <= mediumRoomCount)
        {
            GenerateMediumRoom();
            roomCount2++;
        }
        else if (roomCount3 <= hardRoomCount)
        {
            GenerateHardRoom();
            roomCount3++;
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

    private void GenerateHardRoom()
    {
        int randomIndex = Random.Range(0, hardRooms.Count);

        if (previousRoom)
        {
            if (hardRooms[randomIndex] == previousRoom)
            {
                if (randomIndex + 1 < hardRooms.Count)
                    randomIndex++;
                else
                    randomIndex = 0;
            }
        }

        previousRoom = hardRooms[randomIndex];
        RoomScript newRoom = Instantiate(hardRooms[randomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
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
        easyRooms.Clear();
        mediumRooms.Clear();
        hardRooms.Clear();

        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i] == null) { return; }

            RoomScript roomScript = rooms[i].GetComponent<RoomScript>();

            if (roomScript.roomDifficultyLevel == 1)
                easyRooms.Add(rooms[i]);
            else if (roomScript.roomDifficultyLevel == 2)
                mediumRooms.Add(rooms[i]);
            else if (roomScript.roomDifficultyLevel == 3)
                hardRooms.Add(rooms[i]);
        }

        ShuffleList(easyRooms);
        ShuffleList(mediumRooms);
        ShuffleList(hardRooms);
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public GameObject GetRandomRoom()
    {
        if (spawnedDevilRoom && spawnedTreasureRoom)
        {

            return null;
        }

        List<GameObject> availableRooms = new List<GameObject>();

        if (!spawnedTreasureRoom && treasureRoom != null)
            availableRooms.Add(treasureRoom);

        if (!spawnedDevilRoom && devilRoom != null)
            availableRooms.Add(devilRoom);

        if (availableRooms.Count == 0)
            return null;

        GameObject chosenRoom = availableRooms[Random.Range(0, availableRooms.Count)];

        if (chosenRoom == treasureRoom)
        {
            spawnedTreasureRoom = true;
        }
        else if (chosenRoom == devilRoom)
        {
            spawnedDevilRoom = true;
        }

        return chosenRoom;
    }

    private void GenerateNavMesh()
    {

        StartCoroutine(some());
        

        //navMeshManager.NavMeshBuilder(allSurfaces[0].GetComponent<NavMeshSurface>());
    }

    IEnumerator some()
    {
        yield return new WaitForSeconds(0.1f);

        print("Build");

        surface_1.BuildNavMesh();
        surface_2.BuildNavMesh();

        //for (int i = 0; i < allSurfaces.Count; i++)
        //{
        //    // build navmesh here
        //    navMeshSurfaces[i].BuildNavMesh();
        //}
    }

}
