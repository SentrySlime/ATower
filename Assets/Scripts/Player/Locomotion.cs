using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Locomotion : MonoBehaviour
{
    public float velocity;


    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    public float maxJumps = 0;
    public float extraJumps = 0;


    public float gravityMultiplier = 25;

    [Header("FlyingMovement")]
    public float appliedForce = 0;
    public float flyingMaxSpeed = 100;
    public float speedLoss = 1;

    [Header("LocomotionSFX")]
    public GameObject movementSFX;
    float movementSFXTimer;
    List<AudioSource> SFXList = new List<AudioSource>();
    int soundCount;

    [Header("CoyoteTime")]
    public float currentCoyoteTimer = 0;
    public float maxCoyoteTime = 0;
    public bool canCoyoteJump = false;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask notGround;
    bool grounded;
    bool readyToJump;
    public AudioSource landSFX;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    public Transform orientation;
    public GameObject spherecastStart;


    public float horizontalInput;
    public float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    PlayerState playerState;
    BetterJump betterJump;
    GrapplingHook grapplingHoook;

    private void Start()
    {
        playerState = GetComponent<PlayerState>();
        betterJump = GetComponent<BetterJump>();
        grapplingHoook = GetComponent<GrapplingHook>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
    }



    private void Update()
    {
        velocity = rb.velocity.y;
        CoyoteTime();
        GroundCheck();
        MyInput();

        if (playerState.moveState == PlayerState.StateMachine.defaultMove)
        {
            SpeedControl();
        }
        else if (playerState.moveState == PlayerState.StateMachine.flying)
        {
            FlyingSpeedControl();
            Velocity();

        }

        // handle drag
        if (grounded)
        {
            rb.drag = groundDrag;
            playerState.moveState = PlayerState.StateMachine.defaultMove;
        }
        else
        {
            rb.drag = 0;


        }

        rb.useGravity = !OnSlope();

        if (grounded)
        {
            if (horizontalInput == 0 && verticalInput == 0)
            {

                rb.velocity = new Vector3(0, rb.velocity.y, 0);

            }
            else
                MovementTimer();
        }

    }

    private void FixedUpdate()
    { 
        if(playerState.moveState == PlayerState.StateMachine.grapple || playerState.moveState == PlayerState.StateMachine.freeze) { return; }


        if (rb.velocity.y > -40)
        {
            rb.AddForce(-transform.up * gravityMultiplier);
        }

        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKeyDown(jumpKey) && readyToJump)
        {
            if (grounded || canCoyoteJump || grapplingHoook.grappling)
            {
                canCoyoteJump = false;
                readyToJump = false;
                Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
            }
            else if (extraJumps > 0)
            {
                extraJumps--;
                readyToJump = false;
                Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
            }


        }
    }

    private void MovePlayer()
    {
        if(playerState.moveState == PlayerState.StateMachine.freeze) { return; }
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z);

        // on ground
        if (grounded)
        {
            if (OnSlope())
            {

                rb.AddForce(GetSlopeMoveDirection(moveDirection.normalized) * moveSpeed * 10f, ForceMode.Force);

            }
            else
            {
                rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            }

        }

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void FlyingSpeedControl()
    {

        Vector3 flatVel = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > appliedForce)
        {

            Vector3 limitedVel = flatVel.normalized * appliedForce;
            rb.velocity = new Vector3(limitedVel.x, flatVel.y, limitedVel.z);

        }

    }

    private void Jump()
    {
        if(playerState.moveState == PlayerState.StateMachine.freeze)
        {
            grapplingHoook.StopGrapple();
            canCoyoteJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }
        else if (grapplingHoook.grappling)
        {
            grapplingHoook.BreakGrapple();
        }
        else
        {
            canCoyoteJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }


    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    private void GroundCheck()
    {
        RaycastHit hit;

        if (Physics.SphereCast(spherecastStart.transform.position, 0.5f, -spherecastStart.transform.up, out hit, 1.74f, notGround))
        {
            if (!grounded)
            {
                grounded = true;
                extraJumps = maxJumps;
                //if (landSFX && !landSFX.isPlaying)
                //    landSFX.Play();
            }

        }
        else
            grounded = false;
    }

    private void CoyoteTime()
    {
        if (grounded)
        {
            currentCoyoteTimer = 0;
        }

        if (currentCoyoteTimer < maxCoyoteTime && !grounded)
        {
            currentCoyoteTimer += Time.deltaTime;
            canCoyoteJump = true;
        }
        else if (currentCoyoteTimer > maxCoyoteTime)
        {
            canCoyoteJump = false;
        }

    }

    private bool OnSlope()
    {
        if (Physics.SphereCast(spherecastStart.transform.position, 0.5f, -spherecastStart.transform.up, out slopeHit, 1.74f + 0.3f, notGround))
        {

            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection(Vector3 incomingDir)
    {
        return Vector3.ProjectOnPlane(incomingDir, slopeHit.normal);
    }

    public Vector3 GetPlayerOnNavMesh()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up * -1, out hit, 8, notGround))
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.up * -8);
            return hit.point;
        }
        else
            return transform.position;
    }

    public void MovementTimer()
    {
        if (movementSFXTimer < 0.25f)
        {
            movementSFXTimer += Time.deltaTime;
        }
        else
        {
            if (SFXList.Count >= 20)
            {
                SFXList[soundCount].PlayOneShot(SFXList[soundCount].clip);
                if (soundCount >= 18)
                    soundCount = 0;
                else
                    soundCount++;
            }
            else
            {

                //SFXList.Add(Instantiate(movementSFX.GetComponent<AudioSource>()));

            }
            movementSFXTimer = 0;

        }
    }

    public void StartFlying()
    {
        playerState.moveState = PlayerState.StateMachine.flying;
        appliedForce = rb.velocity.magnitude * 1.5f;
        appliedForce = Mathf.Clamp(appliedForce, 0, flyingMaxSpeed);
        speedLoss = appliedForce / 4;
        print(appliedForce);
    }

    private void Velocity()
    {
        if(appliedForce > 30)
            appliedForce -= Time.deltaTime * speedLoss;
        else
        {
            playerState.moveState = PlayerState.StateMachine.defaultMove;
        }
    }
}
