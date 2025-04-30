using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    public List<RoomScript> rooms = new List<RoomScript>();

    public int roomIndex = 3;

    public float rate = 0.5f;
    public float timer = 0;

    public int roomOffset = 2;

    void Start()
    {

    }


    void Update()
    {
        if (timer < rate)
        {
            timer += Time.deltaTime;
            return;
        }
    }

    public void GoBack(int index)
    {
        if (index <= 1 || index >= rooms.Count - 1) { return; }

        //print("Going back");

        DisableRoom(index + 2);
        EnableRoom(index - 2);
    }

    public void GoForward(int index)
    {
        if (index <= 1 || index >= rooms.Count - 1) { return; }

        //print("Going Forward" + index);

        DisableRoom(index - 2);
        EnableRoom(index + 2);
    }

    private void DisableRoom(int index)
    {
        if (index >= rooms.Count) { return; }
        if (index < 0) { return; }
        rooms[index].DisableEnemies();
    }

    private void EnableRoom(int index)
    {
        if (index  >= rooms.Count) { return; }
        if (index < 0) { return; }

        rooms[index].gameObject.SetActive(true);
        rooms[index].EnableEnemies();
    }

    public void AddRoom(RoomScript room)
    {
        rooms.Add(room);
        room.roomManager = this;
        room.roomIndex = rooms.Count - 1;

        if (rooms.Count > 4)
            StartCoroutine(WaitBeforeDisablingRoom(room));

    }

    public void SetEnabledRooms(int index)
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (i >= index - roomOffset - 1 && i <= index + roomOffset)
            {
                rooms[i].gameObject.SetActive(true);
                rooms[i].EnableEnemies();
            }
            else
            {
                rooms[i].DisableEnemies();
                //rooms[i].gameObject.SetActive(false);
            }
        }
    }

    IEnumerator WaitBeforeDisablingRoom(RoomScript room)
    {
            yield return new WaitForSeconds(1);
        room.DisableEnemies();
        
        //if(room.gameObject.name == "End_Room(Clone)")
        //{

        //    room.gameObject.SetActive(false);
        //}
        //else
        //{
        //    room.gameObject.SetActive(false);
        //}
    }

}
