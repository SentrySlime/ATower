using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedLook : MonoBehaviour
{
    public List<Texture> textures = new List<Texture>();
    private Material material;

    void Start()
    {
        SetRandomTexture();
    }

    public void SetRandomTexture()
    {
        if (textures.Count == 0)
        {
            return;
        }

        Renderer renderer = GetComponent<Renderer>();

        if (renderer == null)
        {
            return;
        }

        material = renderer.material;

        int randomIndex = Random.Range(0, textures.Count);
        material.SetTexture("_MainTex", textures[randomIndex]);
    }

    public void SetRandomTexture(int index)
    {
        if (textures.Count == 0)
        {
            return;
        }

        Renderer renderer = GetComponent<Renderer>();

        if (renderer == null)
        {
            return;
        }

        material = renderer.material;

        material.SetTexture("_MainTex", textures[index]);
    }
}