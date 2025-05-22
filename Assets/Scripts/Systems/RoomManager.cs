using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{

    public List<RoomScript> rooms = new List<RoomScript>();

    public int roomIndex = -1;

    public float rate = 0.5f;
    public float timer = 0;

    public int roomOffset = 2;

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
        rooms[index].DisableRoom();
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
    }

    public void SetEnabledRooms(int index)
    {
        roomIndex = index;

        for (int i = 0; i < rooms.Count; i++)
        {
            if (i >= index - roomOffset - 1 && i <= index + roomOffset)
            {
                rooms[i].gameObject.SetActive(true);
                rooms[i].EnableEnemies();
            }
            else
            {
                rooms[i].DisableRoom();
            }
        }
    }

    public void SetEnabledRooms(RoomScript roomScript)
    {
        print("Disabled all other rooms");

        for (int i = 0; i < rooms.Count; i++)
        {
            if(roomScript == rooms[i])
            {
                if (!rooms[i].isActiveAndEnabled)
                {
                    rooms[i].gameObject.SetActive(true);
                    rooms[i].EnableEnemies();
                }
            }
            else
            {
                rooms[i].DisableRoom();
            }
        }
    }

    public void DisableRooms()
    {
        StartCoroutine(DisableRoomsRoutine());
    }

    IEnumerator DisableRoomsRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 4; i < rooms.Count;i++)
        {
            rooms[i].DisableRoom();
        }
        
    }
}