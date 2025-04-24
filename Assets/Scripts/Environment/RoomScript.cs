using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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

    public List<GameObject> enemyList = new List<GameObject>();

    Transform player;

    [HideInInspector] public int roomIndex;
    [HideInInspector] public RoomManager roomManager;
    [HideInInspector] public LevelGeneration_2 levelGeneration;

    public bool invertedRoomDirection = false;

    void Start()
    {
        if(roomToSpawn)
            SpawnRoom();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }


    void Update()
    {

    }

    public void DisableEnemies()
    {
        if (enemyList.Count == 0)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i])
                    enemyList[i].SetActive(false);
            }
        }


        gameObject.SetActive(false);
    }

    public void EnableEnemies()
    {
        if (enemyList.Count == 0) { return; }

        for (int i = 0; i < enemyList.Count; i++)
        {
            if(enemyList[i])
                enemyList[i].SetActive(true);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        roomManager.SetEnabledRooms(roomIndex);
    }


    private void SpawnRoom()
    {
        if(levelGeneration.spawnedDevilRoom) { return; }

        blockade.SetActive(false);
        Instantiate(roomToSpawn, secretPoint.transform.position, secretPoint.transform.rotation);
        levelGeneration.spawnedDevilRoom = true;
    }
    public void AddEnemy(GameObject enemy)
    {
        enemyList.Add(enemy);
    }

}
