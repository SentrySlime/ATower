using UnityEngine;
using System.Collections;

public class ECdestroyMe : MonoBehaviour
{
    public float damage = 10;
    float timer;
    public float deathtimer = 1;
    float currentTimer = 0;
    GameObject player;
    AMainSystem mainSystem;
    public float minimumRadius;
    // Use this for initialization
    void Start () 
    {
        player = GameObject.FindGameObjectWithTag("Player");
	    mainSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<AMainSystem>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance >= minimumRadius)
            {
                mainSystem.DealDamage(player, 30, false);
                //player.GetComponent<IDamageInterface>().Damage(30);

            }

        }
    }


    private void OnTriggerEnter(Collider other)
    {
        
    }

    void Update ()
    {
        if (currentTimer < deathtimer)
        {
            currentTimer += Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }

        minimumRadius = (transform.localScale.z / 2) -2;
        //minimumRadius -= 2f;
	}
}
