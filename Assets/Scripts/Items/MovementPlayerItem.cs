using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementPlayerItem : ItemBase
{
    public PlayerStats playerMovement;


    [Header("Player movement Stats")]
    [Tooltip("One point is equal to one extra movement speed")]
    [Range(-200, 200f)] public int moveSpeed;


    [Tooltip("1 point is equal to one extra jump")]
    [Range(-5f, 5f)] public int extraJumps;

    private void Awake()
    {

        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();
    }

    private void Start()
    {

    }

    public override void EquipItem()
    {
        playerMovement.moveSpeed += moveSpeed;
        playerMovement.extraJumps += extraJumps;
        playerMovement.StartLocomotion();
    }

    public override void UnEquipItem()
    {
        playerMovement.moveSpeed -= moveSpeed;
        playerMovement.extraJumps -= extraJumps;
        playerMovement.StartLocomotion();
    }
}
