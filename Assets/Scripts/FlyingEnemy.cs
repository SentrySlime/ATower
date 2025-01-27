using UnityEngine;
using UnityEngine.AI;

public class FlyingEnemy : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target; // The target the enemy will follow

    private NavMeshAgent agent;

    void Start()
    {
        // Get the NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
        print(agent.agentTypeID);
    }

    void Update()
    {
        agent.SetDestination(target.transform.position);
    }

}