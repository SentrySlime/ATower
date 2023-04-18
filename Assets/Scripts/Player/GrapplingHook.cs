using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrapplingHook : MonoBehaviour
{

    public LayerMask layermask;

    public float grappleDistance = 100;
    public float grappleSpeed = 1;
    public Transform barrel;
    public LineRenderer lineRender;
    public bool grappling = false;

    [Header("Info Tab")]
    public Image crosshair;
    public TextMeshProUGUI distanceText;

    [Header("SFX")]
    public AudioSource reelingIn;

    Rigidbody rb;
    Vector3 grappleDirection;
    Vector3 grapplePoint;
    PlayerState playerState;
    Locomotion locomotion;
    BetterJump betterJump;

    void Start()
    {
        layermask = ~layermask;
        rb = GetComponent<Rigidbody>();
        locomotion = GetComponent<Locomotion>();
        betterJump = GetComponent<BetterJump>();
        playerState = GetComponent<PlayerState>();
    }

    
    void Update()
    {
        GrappleInfo();
        
        if (Input.GetMouseButtonDown(1))
            StartGrapple();

        if (Input.GetMouseButtonUp(1))
            StopGrapple();

        if (grappling)
            Grapple();
    }

    private void StartGrapple()
    {

        RaycastHit hit;
        if(Physics.Raycast(barrel.transform.position, barrel.transform.forward, out hit, grappleDistance, layermask))
        {
            Debug.DrawLine(barrel.transform.position, barrel.transform.forward * 999, Color.red, 1);
            grapplePoint = hit.point;
            grappleDirection = grapplePoint - transform.position;

            playerState.moveState = PlayerState.StateMachine.grapple;

            //locomotion.enabled = false;
            betterJump.enabled = false;

            rb.velocity = Vector3.zero;

            rb.useGravity = false;
            grappling = true;

            lineRender.enabled = true;
            lineRender.SetPosition(1, grapplePoint);

            if (!reelingIn.isPlaying)
                reelingIn.Play();
        }
    }

    private void Grapple()
    {
        lineRender.SetPosition(0, lineRender.transform.position);
        float distance = Vector3.Distance(barrel.transform.position, grapplePoint);
        //print(distance);
        if (distance > 6)
        {
            
            grappleDirection = (grapplePoint - transform.position).normalized;
            Vector3 movePos = transform.position + grappleDirection * grappleSpeed * Time.deltaTime;
            rb.velocity = grappleDirection * grappleSpeed;
        }
        else
        {
            reelingIn.Stop();
            playerState.moveState = PlayerState.StateMachine.freeze;
            rb.velocity = Vector3.zero;
            rb.useGravity = false;
        }
    }

    public void BreakGrapple()
    {
        if (!grappling) { return; }

        reelingIn.Stop();

        grappling = false;
        betterJump.enabled = true;

        rb.useGravity = true;
        
        locomotion.StartFlying();
        lineRender.enabled = false;
    }

    public void StopGrapple()
    {
        if(!grappling) { return; }

        reelingIn.Stop();

        grappling = false;
        betterJump.enabled = true;
        rb.useGravity = true;

        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / 2, rb.velocity.z);
        
        playerState.moveState = PlayerState.StateMachine.defaultMove;
        lineRender.enabled = false;
    }

    public void GrappleInfo()
    {
        RaycastHit hit;
        if(Physics.Raycast(barrel.transform.position, barrel.transform.forward, out hit, 999, layermask))
        {

            float distance = Vector3.Distance(barrel.transform.position, hit.point);
            if(distance <= 999)
            {
                distanceText.text = distance.ToString("0.00");
                crosshair.color = Color.white;
            }
            else
            {
                distanceText.text = "\u221E";
            }

            if(distance > grappleDistance)
                crosshair.color = Color.red;
        }
        else
        {
            crosshair.color = Color.red;
            distanceText.text =  "\u221E";
        }
    }

    public void GrappleCooldown()
    {

    }

}
