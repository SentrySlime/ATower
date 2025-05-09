using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{

    float scale = 0;
    Collider barrierCollider;

    private void Awake()
    {
        barrierCollider = GetComponent<Collider>();
    }

    void Start()
    {
        transform.localScale = new Vector3(scale, scale, scale);
        StartCoroutine(Grow());
    }

    IEnumerator Grow()
    {

        while(scale < 1)
        {
            scale += Time.deltaTime;
            transform.localScale = new Vector3 (scale, scale, scale);

            yield return null;
        }

        barrierCollider.enabled = true;

        yield return null;
    }

    
}
