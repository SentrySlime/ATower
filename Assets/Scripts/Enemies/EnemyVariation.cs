using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVariation : MonoBehaviour
{
    public List<GameObject> variationParts = new List<GameObject>();
    
    void Start()
    {
        for (int i = 0; i < variationParts.Count; i++)
        {
            variationParts[i].SetActive(false);
        }

        RandomizeParts();
    }

    
    void Update()
    {
        
    }

    private void RandomizeParts()
    {
        for (int i = 0; i < variationParts.Count; i++)
        {
            if(randomBoolean())
                variationParts[i].SetActive(true);
        }
    }

    private bool randomBoolean()
    {
        if (Random.value >= 0.5)
        {
            return true;
        }
        return false;
    }
}
