using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGeneration : MonoBehaviour
{
    [SerializeField] NavMeshSurface[] navMeshSurfaces;

    public NavMeshSurface surfaCE;

    public enum Direction {forward, left, right}

    public Direction direction = Direction.forward;

    [Header("Rooms")]
    public List<GameObject> rooms = new List<GameObject>();
    public List<GameObject> fordwardRooms = new List<GameObject>();
    public List<GameObject> leftRooms = new List<GameObject>();
    public List<GameObject> rightRooms = new List<GameObject>();

    [Header("Corridors")]
    public List<GameObject> corridors = new List<GameObject>();
    public List<GameObject> fordwardCorridors = new List<GameObject>();
    public List<GameObject> leftCorridors = new List<GameObject>();
    public List<GameObject> rightCorridors = new List<GameObject>();

    [Header("Special Rooms")]
    public GameObject startingRoom;
    public GameObject endRoom;
    public GameObject treasureRoom;

    GameObject spawnTransform;
    public int allRoomCount;
    bool currentRoom = true;

    [HideInInspector] public bool spawnedDevilRoom = false;

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
    RoomManager roomManager;

    bool hasBuilt = false;

    void Start()
    {
        navMeshManager = GetComponent<NavMeshManager>();
        roomManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<RoomManager>();
        
        allRoomCount = rooms.Count + 2;
        
        if (!removeRooms)
            allRoomCount = amountOfRoomsToSpawn;

        //SpawnRoomSequence(12, transform);
    }


    void Update()
    {

        if (roomCount < allRoomCount)
        {
            if (rooms.Count == 0) { return; }

            if (!hasTimer)
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
        else if (!hasBuilt)
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

    private int GetRandomRightRoom()
    {
        return Random.Range(0, rightRooms.Count);
    }

    private int GetRandomLeftRoom()
    {
        return Random.Range(0, leftRooms.Count);
    }

    private int GetRandomCorridor()
    {
        return Random.Range(0, corridors.Count);
    }

    private int GetRandomRightCorridor()
    {
        return Random.Range(0, rightCorridors.Count);
    }

    private int GetRandomLeftCorridor()
    {
        return Random.Range(0, leftCorridors.Count);
    }

    private void GenerateLevel()
    {
        if (currentRoom)
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
        if (roomManager)
            roomManager.AddRoom(newRoom);
        spawnTransform = newRoom.point2.gameObject;
    }

    private void GenerateEndRoom()
    {
        RoomScript newRoom = Instantiate(endRoom, spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
        spawnTransform = newRoom.point2.gameObject;
        if (roomManager)
            roomManager.AddRoom(newRoom);
    }

    private void GenerateStartingRoom()
    {
        RoomScript newRoom = Instantiate(startingRoom, transform.position, Quaternion.identity).GetComponent<RoomScript>();
        spawnTransform = newRoom.point2.gameObject;
        if (roomManager)
            roomManager.AddRoom(newRoom);
    }

    private void GenerateRoom()
    {
        if (roomCount == 0)
        {
            roomCount++;
            GenerateStartingRoom();
        }
        else if (roomCount == 3)
        {
            roomCount++;
            GenerateTreasureRoom();
        }
        else if (direction == Direction.forward)
        {
            SpawnRandomRoom();
        }
        else if (direction == Direction.left)
        {
            SpawnRightRoom();
        }
        else if (direction == Direction.right)
        {
            SpawnLeftRoom();
        }
    }

    private void SpawnRandomRoom()
    {
        print("Spawned random room");

        int roomIndex = GetRandomRoom();

        roomCount++;
        RoomScript newRoom = Instantiate(rooms[roomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();

        if (newRoom.direction == RoomScript.Direction.left)
            direction = Direction.left;
        else if (newRoom.direction == RoomScript.Direction.right)
            direction = Direction.right;
        else if (newRoom.direction == RoomScript.Direction.forward)
            direction = Direction.forward;


        if (roomManager)
            roomManager.AddRoom(newRoom);
        spawnTransform = newRoom.point2.gameObject;
        //newRoom.levelGeneration = this;

        if (removeRooms)
            rooms.RemoveAt(roomIndex);
    }


    private void SpawnRightRoom()
    {
        print("Spawned right room");

        int roomIndex = GetRandomRightRoom();

        if (roomIndex < 0 || roomIndex >= rightRooms.Count)
        {
            Debug.LogWarning($"Invalid roomIndex: {roomIndex}, rightRooms.Count: {rightRooms.Count}");
            return; // Don't try to instantiate or remove if index is bad
        }

        RoomScript newRoom = Instantiate(
            rightRooms[roomIndex],
            spawnTransform.transform.position,
            spawnTransform.transform.rotation
        ).GetComponent<RoomScript>();

        roomCount++;
        direction = Direction.right;

        if (roomManager)
            roomManager.AddRoom(newRoom);

        spawnTransform = newRoom.point2.gameObject;
        //newRoom.levelGeneration = this;

        if (removeRooms)
        {
            rightRooms.RemoveAt(roomIndex);
        }
    }



    private void SpawnLeftRoom()
    {
        print("Spawned left room");

        int roomIndex = GetRandomLeftRoom();

        roomCount++;
        RoomScript newRoom = Instantiate(leftRooms[roomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();

        direction = Direction.left;

        if (roomManager)
            roomManager.AddRoom(newRoom);
        spawnTransform = newRoom.point2.gameObject;
        //newRoom.levelGeneration = this;

        if (removeRooms && roomIndex >= 0 && roomIndex < leftRooms.Count)
        {
            leftRooms.RemoveAt(roomIndex);
        }   
    }



    private void GenerateCorridor()
    {
        

        if (direction == Direction.forward)
        {
            SpawnRandomCorridor();
        }
        else if (direction == Direction.left)
        {
            SpawnRightCorridor();
        }
        else if (direction == Direction.right)
        {
            SpawnLeftCorridor();
        }

        //int roomIndex = GetRandomCorridor();

        ////GetChildren(newNumber);
        //if (roomCount == 0)
        //{
        //    RoomScript newRoom = Instantiate(corridors[roomIndex], transform.position, Quaternion.identity).GetComponent<RoomScript>();
        //    spawnTransform = newRoom.point2.gameObject;
        //    if (roomManager)
        //        roomManager.AddRoom(newRoom);
        //}
        //else
        //{
        //    RoomScript newRoom = Instantiate(corridors[roomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();
        //    spawnTransform = newRoom.point2.gameObject;
        //    if (roomManager)
        //        roomManager.AddRoom(newRoom);
        //}



        //if (removeCorridor)
        //    corridors.RemoveAt(roomIndex);

        //roomCount++;
    }

    private void SpawnRandomCorridor()
    {
        print("Special room");

        int roomIndex = GetRandomCorridor();

        roomCount++;
        RoomScript newRoom = Instantiate(corridors[roomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();

        if (newRoom.direction == RoomScript.Direction.left)
            direction = Direction.left;
        else if (newRoom.direction == RoomScript.Direction.right)
            direction = Direction.right;
        else if (newRoom.direction == RoomScript.Direction.forward)
            direction = Direction.forward;


        if (roomManager)
            roomManager.AddRoom(newRoom);
        spawnTransform = newRoom.point2.gameObject;
        //newRoom.levelGeneration = this;

        //----

        //if (removeCorridor)
        //    corridors.RemoveAt(roomIndex);
    }


    private void SpawnRightCorridor()
    {
        print("Spawned right room");

        int roomIndex = GetRandomRightCorridor();

        RoomScript newRoom = Instantiate(rightCorridors[roomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();

        direction = Direction.left;

        if (roomManager)
            roomManager.AddRoom(newRoom);
        spawnTransform = newRoom.point2.gameObject;
        //newRoom.levelGeneration = this;

        //if (removeRooms && roomIndex >= 0 && roomIndex < rightCorridors.Count)
        //{
        //    rightCorridors.RemoveAt(roomIndex);
        //}
    }


    private void SpawnLeftCorridor()
    {
        print("Spawned left room");

        int roomIndex = GetRandomLeftCorridor();

        RoomScript newRoom = Instantiate(leftCorridors[roomIndex], spawnTransform.transform.position, spawnTransform.transform.rotation).GetComponent<RoomScript>();

        direction = Direction.left;

        if (roomManager)
            roomManager.AddRoom(newRoom);
        spawnTransform = newRoom.point2.gameObject;
        //newRoom.levelGeneration = this;

        //if (removeRooms && roomIndex >= 0 && roomIndex < leftCorridors.Count)
        //{
        //    leftCorridors.RemoveAt(roomIndex);
        //}
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

    public void SpawnRoomSequence(int numberOfRooms, Transform startTransform)
    {
        Transform currentSpawnTransform = startTransform;

        List<GameObject> availableRooms = new List<GameObject>(rooms);

        // Sort rooms by difficulty ascending
        availableRooms.Sort((a, b) =>
        {
            return a.GetComponent<RoomScript>().roomDifficultyLevel
                .CompareTo(b.GetComponent<RoomScript>().roomDifficultyLevel);
        });

        RoomScript.Direction[] directionPattern = new RoomScript.Direction[]
        {
        RoomScript.Direction.right,
        RoomScript.Direction.forward,
        RoomScript.Direction.left
        };

        int directionIndex = 0;

        for (int i = 0; i < numberOfRooms; i++)
        {
            RoomScript.Direction desiredDirection = directionPattern[directionIndex];

            // Try to find a room that matches the direction
            GameObject chosenRoom = null;

            for (int j = 0; j < availableRooms.Count; j++)
            {
                var roomScript = availableRooms[j].GetComponent<RoomScript>();
                if (roomScript.direction == desiredDirection)
                {
                    chosenRoom = availableRooms[j];
                    availableRooms.RemoveAt(j);
                    break;
                }
            }

            // If no match found, pick any available room
            if (chosenRoom == null && availableRooms.Count > 0)
            {
                chosenRoom = availableRooms[0];
                availableRooms.RemoveAt(0);
            }

            if (chosenRoom == null) break; // No more rooms

            Debug.Log("Spawned random room");

            RoomScript newRoom = Instantiate(chosenRoom, currentSpawnTransform.position, currentSpawnTransform.rotation)
                .GetComponent<RoomScript>();

            // Assign systems
            //newRoom.levelGeneration = this;
            if (roomManager)
            {
                roomManager.AddRoom(newRoom);
            }

            // Set up for next iteration
            currentSpawnTransform = newRoom.point2;
            directionIndex = (directionIndex + 1) % directionPattern.Length;
        }
    }


}
