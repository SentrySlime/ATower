using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelGenTest : MonoBehaviour
{
    public NavMeshSurface surface;

    void Start()
    {
        

        surface.BuildNavMesh(); // Rebuilds the NavMesh to match new geometry
    }
}