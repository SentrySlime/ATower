using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleAIMovement : MonoBehaviour
{

    NavMeshAgent agent;

    public float range = 10;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        
    }

    
    void Update()
    {
        if(agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if(RandomPoint(transform.position, range, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.red, 1);
                agent.SetDestination(point);
            }
        }

        
        
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if(NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
