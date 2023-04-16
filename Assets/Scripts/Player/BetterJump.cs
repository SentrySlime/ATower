using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float flyingDownMultiplier = 2f;

    Rigidbody rb;
    PlayerState playerState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerState = GetComponent<PlayerState>();
    }


    private void Update()
    {

        if(playerState.moveState == PlayerState.StateMachine.freeze) { return; }

        if (rb.velocity.y < 0 && playerState.moveState != PlayerState.StateMachine.flying)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        else if(playerState.moveState == PlayerState.StateMachine.flying)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (flyingDownMultiplier - 1) * Time.deltaTime;
        }

    }

}