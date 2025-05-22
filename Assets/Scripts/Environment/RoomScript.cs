using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

public class RoomScript : MonoBehaviour
{
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

    public List<GameObject> enemyList = new List<GameObject>();

    Transform player;
    GameObject specialRoomObj;
    SpecialRoom specialRoom;

    [HideInInspector] public int roomIndex;
    [HideInInspector] public RoomManager roomManager;
    [HideInInspector] public LevelGeneration_2 levelGeneration;

    public bool invertedRoomDirection = false;

    void Start()
    {
        SpawnRoom();
        GameObject tempPlayer = GameObject.FindGameObjectWithTag("Player");
        if(tempPlayer != null)
            player = tempPlayer.GetComponent<Transform>();
    }

    public void DisableRoom()
    {
        if(specialRoomObj)
            specialRoomObj.SetActive(false);

        if (enemyList.Count != 0)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                if (enemyList[i])
                {
                    if(enemyList[i].TryGetComponent<Spawner>(out var spawner))
                    {
                        spawner.DisableSpawnlings();
                    }
                    enemyList[i].SetActive(false);
                }
            }
        }

        gameObject.SetActive(false);
    }

    public void EnableEnemies()
    {
        if (specialRoomObj)
            specialRoomObj.SetActive(true);

        if (enemyList.Count == 0) { return; }

        for (int i = 0; i < enemyList.Count; i++)
        {
            if(enemyList[i])
            {
                enemyList[i].SetActive(true);
                if (enemyList[i].TryGetComponent<Spawner>(out var spawner))
                {
                    spawner.EnableSpawnlings();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(roomManager)
            roomManager.SetEnabledRooms(roomIndex);
    }
    private void SpawnRoom()
    {

        if(!levelGeneration) { return; }
        if (!blockade) { return; }
        
        GameObject roomToSpawn = levelGeneration.GetRandomRoom();

        if(roomToSpawn == null) { return; }

        blockade.SetActive(false);
        specialRoomObj = Instantiate(roomToSpawn, secretPoint.transform.position, secretPoint.transform.rotation);

        specialRoom = specialRoomObj.GetComponent<SpecialRoom>();
        specialRoom.roomManager = roomManager;
        specialRoom.roomScript = this;
    }

    public void AddEnemy(GameObject enemy)
    {
        enemyList.Add(enemy);

        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
        if (enemyBase)
            enemyBase.OnEnemyDied += HandleEnemyDeath;

    }

    private void HandleEnemyDeath(EnemyBase deadEnemy)
    {
        enemyList.Remove(deadEnemy.gameObject);

        if (specialRoom == null) return;

        if(enemyList.Count == 0)
        {
            specialRoom.IniateLockRelease();
        }    
    }

}
