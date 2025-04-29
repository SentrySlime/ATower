using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedMesh : MonoBehaviour
{
    [Header("Meshes")]
    public List<Mesh> meshes = new List<Mesh>();
    MeshFilter meshFilter;

    [Header("Textures")]
    public List<Texture> textures = new List<Texture>();
    Material material;

    void Start()
    {
        

        SetRandomMesh();
        SetRandomTexture();
    }

    private void SetRandomMesh()
    {

        if (meshes.Count == 0)
            return;


        meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null)
            return;
        
        int randomIndex = Random.Range(0, meshes.Count);

        meshFilter.mesh = meshes[randomIndex];
    }

    public void SetRandomTexture()
    {

        if (textures.Count == 0)
            return;

        int randomIndex = Random.Range(0, textures.Count);

        Renderer renderer = GetComponent<Renderer>();

        if (renderer == null)
            return;

        material = renderer.material;
        material.SetTexture("_MainTex", textures[randomIndex]);
    }

}
