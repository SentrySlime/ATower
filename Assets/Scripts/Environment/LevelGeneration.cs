using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGeneration : MonoBehaviour
{
    
    [SerializeField]
    NavMeshSurface[] navMeshSurfaces;

    public List<GameObject> rooms = new List<GameObject>();
    GameObject spawnTransform;
    int allRoomCount;

    public int roomCount = 0;

    public float rate1 = 1.5f;
    public float timer1 = 0;

    public List<GameObject> allSurfaces = new List<GameObject>();
    //public List<> allSurfaces = new List<GameObject>();

    NavMeshManager navMeshManager;

    bool hasBuilt = false;

    void Start()
    {
        navMeshManager = GetComponent<NavMeshManager>();
        allRoomCount = rooms.Count;

        
    }


    void Update()
    {
        
        if (roomCount < allRoomCount)
        {
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
        //else if(!hasBuilt)
        //{

        //    //GenerateNavmesh here
        //    GenerateNavMesh();
        //    hasBuilt = true;    
        //}

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
