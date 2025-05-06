using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTeleporterScript : MonoBehaviour
{

    Transform player;
    AudioManager audioManager;


    public Transform reciever;
    private bool playerIsOverlapping = false;

    public bool isDevilRoom = false;

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AudioManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
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
                    DevilRoomLogic();
                else
                    MainLogic();

                
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


}
