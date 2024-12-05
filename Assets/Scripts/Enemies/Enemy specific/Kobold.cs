using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Kobold : MonoBehaviour
{
    public Animator animator;
    GameObject player;

    public float firingRate = 2;
    public float timer;

    [Header("Ranged")]
    public bool rangedAttack;
    public GameObject rangedProjectile;
    public float rangedDistance_ = 75;
    public Transform shootPoint;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        firingRate = UnityEngine.Random.Range(0.3f, 4f);

    }

    
    void Update()
    {
        float playerDistance = Vector3.Distance(transform.position, player.transform.position);

        if (timer < firingRate)
        {
            timer += Time.deltaTime;
        }
        else if (playerDistance > 15 && rangedAttack)
        {
            timer = 0;
            animator.SetBool("CloseToAttack", true);
            //RangedAttack();
        }
        else
            animator.SetBool("CloseToAttack", false);

    }

    public void RangedAttack()
    {
        Console.WriteLine("We attacked");

        float length = Vector3.Distance(player.transform.position, transform.position);
        if (length < rangedDistance_)
        {

            Vector3 direction = player.transform.position - shootPoint.position;

            // Create a rotation that points towards the player
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // Instantiate the projectile with the correct rotation
            Instantiate(rangedProjectile, shootPoint.position, lookRotation);

            //rb.AddForce(direction.normalized * projectileSpeed, ForceMode.Impulse);

        }
    }
}
