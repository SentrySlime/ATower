using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dragon : MonoBehaviour
{
    GameObject player;
    NavMeshAgent agent;


    [Header("Player Detection")]
    bool foundPlayer = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    
    void Update()
    {
        agent.SetDestination(player.transform.position);
        
    }
}