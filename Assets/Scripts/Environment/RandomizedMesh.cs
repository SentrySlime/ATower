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
        int randomIndex = Random.Range(0, meshes.Count);

        SetRandomMesh(randomIndex);
        SetRandomTexture(randomIndex);
    }

    private void SetRandomMesh(int index)
    {
        if (meshes.Count == 0)
            return;

        meshFilter = GetComponent<MeshFilter>();

        if (meshFilter == null)
            return;

        meshFilter.mesh = meshes[index];
    }

    public void SetRandomTexture(int index)
    {
        if (textures.Count == 0)
            return;

        Renderer renderer = GetComponent<Renderer>();

        if (renderer == null)
            return;

        material = renderer.material;
        material.SetTexture("_MainTex", textures[index]);
    }

}
