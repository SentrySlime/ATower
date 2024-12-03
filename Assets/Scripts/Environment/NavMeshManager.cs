using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshManager : MonoBehaviour
{
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void NavMeshBuilder(NavMeshSurface navMeshSurface)
    {
        print("Started building" + navMeshSurface);
        navMeshSurface.BuildNavMesh();
        print("Finished building");
    }
}
