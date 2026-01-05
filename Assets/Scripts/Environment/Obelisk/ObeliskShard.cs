using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskShard : MonoBehaviour
{

    public Vector3 defaultPosition;

    public float moveSpeed = 70f; 

    private bool move = false;

    public ParticleSystem vfx;

    float distance = 100;

    void Start()
    {
     
        
    }

 
    void Update()
    {
        if(move)
        {
            Distance();
            Move();
        }
    }


    public void StartMovement()
    {
        StartCoroutine(EnableMovement());
        
    }

    IEnumerator EnableMovement()
    { 
        yield return new WaitForSeconds(0.5f);
        move = true;
    }

    private void Move()
    {
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, defaultPosition, moveSpeed * Time.deltaTime);

        if (distance < 2)
        {
            move = false;
            transform.localPosition = defaultPosition;

            if(vfx)
                vfx.Play();
        }

    }

    private void Distance()
    {
        distance = Vector3.Distance(transform.localPosition, defaultPosition);
    }

}
