using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Locomotion2 : MonoBehaviour
{
    public float velocityZ;
    public float velocityX;


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
    bool readyToJump;

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
    public bool pushed;
    public AudioSource landSFX;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;

    public Transform orientation;
    public GameObject spherecastStart;
    public float length = 1.74f;


    public float horizontalInput;
    public float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    PlayerStats playerStats;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerStats = GetComponent<PlayerStats>();
        readyToJump = true;
    }



    private void Update()
    {

        velocityX = rb.velocity.x;
        velocityZ = rb.velocity.z;
        velocity = rb.velocity.magnitude;
        //Debug.LogError("RB.X" + rb.velocity.x);
        //Debug.LogError("RB.Z" + rb.velocity.z);
        CoyoteTime();
        // ground check
        GroundCheck();
        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
        {
            rb.drag = 0;

        }

        rb.useGravity = !OnSlope();


        if(pushed) { return; }

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
        if (rb.velocity.y > -900)
        {
            rb.AddForce(-transform.up * gravityMultiplier);
        }
        else
        {
            Vector3 tempVel = rb.velocity;
            rb.AddForce(new Vector3(rb.velocity.x, rb.velocity.y * -2, rb.velocity.z));
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
            if (grounded || canCoyoteJump)
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

    private void Jump()
    {


        canCoyoteJump = false;
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }

    private void GroundCheck()
    {
        RaycastHit hit;

        if (Physics.SphereCast(spherecastStart.transform.position, 0.5f, -spherecastStart.transform.up, out hit, length, notGround))
        {
            if (!grounded)
            {
                grounded = true;
                extraJumps = maxJumps;
                if (!landSFX.isPlaying)
                    landSFX.Play();
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
        if (Physics.SphereCast(spherecastStart.transform.position, 0.5f, -spherecastStart.transform.up, out slopeHit, length + 0.3f, notGround))
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
            if (SFXList.Count >= 10)
            {
                SFXList[soundCount].PlayOneShot(SFXList[soundCount].clip);
                if (soundCount >= 8)
                    soundCount = 0;
                else
                    soundCount++;
            }
            else
            {
                //AudioSource source = Instantiate(audioSource.GetComponent<AudioSource>().clip = Sfx);
                //audioSource.GetComponent<AudioSource>().clip = Sfx;

                SFXList.Add(Instantiate(movementSFX.GetComponent<AudioSource>()));

            }
            movementSFXTimer = 0;

        }
    }

    public void UpdateJumps(float incomingJumps)
    {
        maxJumps = incomingJumps;
        
        if(grounded)
        {
            extraJumps = maxJumps;
        }
        else
        {
            extraJumps += incomingJumps;
        }

    }

    public void Push()
    {
        pushed = true;
        StartCoroutine(RemovePush());
    }

    IEnumerator RemovePush()
    {
        yield return new WaitForSeconds(0.15f);
        pushed = false; 
    }

}
