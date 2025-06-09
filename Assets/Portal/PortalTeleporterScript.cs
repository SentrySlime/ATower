using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporterScript : MonoBehaviour
{

    Transform player;
    AudioManager audioManager;
    RoomManager roomManager;

    public SpecialRoom specialRoom;
    
    public Transform reciever;
    private bool playerIsOverlapping = false;

    public bool isDevilRoom = false;
    public bool isShopRoom = false;
    public bool isTreasureRoom = false;
    public bool isBlackSmith = false;
    public bool isAmbiens = false;


    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        roomManager = specialRoom.roomManager;
    }

    
    void Update()
    {
        

        if (playerIsOverlapping)
        {
            Vector3 portalToPlayer = player.position - transform.position;
            float dotProduct = Vector3.Dot(transform.up, portalToPlayer);


            
            if (dotProduct < 0f)
            {

                if (isDevilRoom)
                {
                    DevilRoomLogic();
                    if (roomManager)
                        roomManager.SetEnabledRooms(specialRoom.roomScript);
                }
                else if(isShopRoom)
                {
                    ShopRoomLogic();
                    if (roomManager)
                        roomManager.SetEnabledRooms(specialRoom.roomScript);

                }
                else if (isBlackSmith)
                {
                    BlackSmithRoomLogic();
                    if (roomManager)
                        roomManager.SetEnabledRooms(specialRoom.roomScript);
                }
                else if (isAmbiens)
                {
                    AmbiensLogic();
                    if (roomManager)
                        roomManager.SetEnabledRooms(specialRoom.roomScript);
                }
                else if (isTreasureRoom)
                {
                    if (roomManager)
                        roomManager.SetEnabledRooms(specialRoom.roomScript);
                }
                else
                {
                    if (roomManager)
                        roomManager.SetEnabledRooms(roomManager.roomIndex);
                    MainLogic();
                }

                
                float rotationDiff = -Quaternion.Angle(transform.rotation, reciever.rotation);
                rotationDiff += 180;
                player.Rotate(Vector3.up, rotationDiff);

                Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToPlayer;
                player.position = reciever.position + positionOffset;

                playerIsOverlapping = false;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsOverlapping = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIsOverlapping = false;
        }
    }

    private void MainLogic()
    {
        audioManager.TriggerMainMusic();
    }

    private void DevilRoomLogic()
    {
        audioManager.TriggerDevilMusic();
    }

    private void ShopRoomLogic()
    {
        audioManager.TriggerShopMusic();
    }

    private void BlackSmithRoomLogic()
    {
        audioManager.TriggerBlacksmithMusic();
    }

    private void AmbiensLogic()
    {
        audioManager.TriggerAmbiens();
    }

}
