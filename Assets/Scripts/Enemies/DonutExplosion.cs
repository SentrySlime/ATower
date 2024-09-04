using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DonutExplosion : MonoBehaviour
{
    public float scaleSpeed = 1;
    public float currentRadius = 1;
    public float explosionScale = 1;

    public float maxRadius = 50;

    public PlayerHealth playerhp;
    public ParticleSystem ps;
    public GameObject collidingObj;

    Vector3 currentScale = new Vector3(6.5f, 0, 6.5f);


    void Start()
    {
        transform.root.transform.localScale = new Vector3(explosionScale, 1, explosionScale);

        ps.startLifetime = maxRadius * 0.05f;

        playerhp = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            float tempRadius = (currentRadius - 1) * 0.5f;

            if (distance >= tempRadius)
            {
                playerhp.Damage(30);
            }
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{

    //    //print("Triggered");
        
    //    if (other.CompareTag("Player"))
    //    {

    //        float distance = Vector3.Distance(transform.position, other.transform.position);


    //        float tempRadius = (minimumRadius -1) * 0.5f;
    //        print(tempRadius + " + " + minimumRadius);
            
    //        //print(distance + "+" + tempRadius);

    //        if (distance >= tempRadius)
    //        {
    //            playerhp.Damage(30, false);
    //            print(distance + "+" + tempRadius);
    //        }
    //    }
    //}

    void Update()
    {

        if (currentRadius < maxRadius -2)
        {
            collidingObj.transform.localScale += currentScale * Time.deltaTime * scaleSpeed;
            currentRadius = collidingObj.transform.localScale.z;
        }
        else if(currentRadius > maxRadius - 2)
        {
            collidingObj.SetActive(false);

        }
        else if (currentRadius > maxRadius + 7)
        {
            Destroy(gameObject);
        }

    }
}
