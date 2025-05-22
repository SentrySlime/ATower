using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Hexademon_C : Enemy_Movement
{
    public Transform shootPoint;

    //List<Action>  

    public bool canRadialAttack = true;
    public bool canMeleeAttack = true;
    public bool canBeamAttack = true;

    [Header("// --- RadialAttack --- \\")]
    public GameObject radialTelegraphVFX;
    public GameObject radialProjectile;
    public Transform radialAttackPoint;


    [Header("// --- Melee --- \\")]
    public GameObject meleeTelegraph;
    public GameObject meleeVFX;
    public GameObject expandingAttackPosition;
    public AudioSource meleeTelegraphSFX;
    public LayerMask meleeLayerMask;
    
    
    float meleeDamage = 30;
    float telegraphScaleSpeed = 9;
    float meleeDistance = 25;
    float meleeForce = 300;
    float meleeForceY = 30;


    [Header("// --- Beam --- \\")]
    public GameObject beamTelegraph;
    public Transform beamTelegraphPosition;
    public LineRenderer beamLineRender;
    public AudioSource beamAttackSFX;

    

    float beamDamage = 7;
    float beamDamageRate = 0.1f;

    float beamDistance = 80;
    float beamDamageRateTimer = 0f;
    float beamFollowSpeed = 0.125f;
    float maxBeamFollowSpeed = 0.125f;
    float yawSpeed = 0.5f;
    float pitchSpeed = 6f;
    float rotationSpeed = 80;

    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();
    }

    public override void AttackLogic()
    {
        if (isAttacking) return;

        if (attackRateTimer < attackRate)
            attackRateTimer += Time.deltaTime;
        else
        {

            if (playerDistance < meleeDistance)
            {

                if(canMeleeAttack)
                    MeleeAttack();
                else if(canRadialAttack)
                    RadialAttack();
                else if(canBeamAttack)
                    BeamAttack();

            }
            else if (playerDistance < beamDistance)
            {
                if(canBeamAttack)
                {
                    if (IsPlayerInfront() && HasLineOfSight())
                    {
                        BeamAttack();
                    }
                }
                else if(canRadialAttack)
                {
                    RadialAttack();
                }
                else
                {
                    MeleeAttack();
                }


                
            }
        }
    }

    public override void StartAttack(float speed)
    {
        base.StartAttack(speed);
    }

    public override void EndAttack()
    {
        agent.speed = movementSpeed;
        base.EndAttack();
    }

    #region Radial

    private void RadialAttack()
    {

        StartAttack(1);
        canRadialAttack = false;
        Instantiate(radialTelegraphVFX, visionPoint.position, Quaternion.identity, visionPoint);
        StartCoroutine(DoRadialAttack());
    }

    IEnumerator DoRadialAttack()
    {
        yield return new WaitForSeconds(1);

        int repeatCount = 3;
        float fireRate = 0.6f;
        int projectileCount = 18;
        float stepAngle = 360f / projectileCount;

        for (int round = 0; round < repeatCount; round++)
        {
 
            float angleOffset = (round % 2 == 0) ? 0f : stepAngle / 2f;

            for (int i = 0; i < projectileCount; i++)
            {
                float angle = i * stepAngle + angleOffset;

                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                Vector3 direction = rotation * Vector3.forward;
                Quaternion lookRotation = Quaternion.LookRotation(direction);

                Instantiate(radialProjectile, radialAttackPoint.position, lookRotation);
            }

            yield return new WaitForSeconds(fireRate); 
        }

        StartCoroutine(RadialAttackCooldown());
        EndAttack();
    }

    IEnumerator RadialAttackCooldown()
    {
        yield return new WaitForSeconds(5);
        canRadialAttack = true;

    }

    IEnumerator DoRadialAttack2()
    {
        yield return new WaitForSeconds(1.4f); 

        int repeatCount = 3;
        float fireRate = 0.2f;
        int projectileCount = 30;

        float thetaOffsetMultiplier = Mathf.PI * 1.3f;

        for (int round = 0; round < repeatCount; round++)
        {
            Quaternion roundOffset = Quaternion.Euler(0, round * 10f, 0);
            float thetaOffset = round * thetaOffsetMultiplier;

            for (int i = 0; i < projectileCount; i++)
            {
                float t = (i + 0.5f) / projectileCount;

               
                float biasedT = Mathf.Pow(t, 0.15f); 
                float phi = biasedT * (Mathf.PI / 2f); 

                float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * i + thetaOffset;

                float x = Mathf.Sin(phi) * Mathf.Cos(theta);
                float z = Mathf.Sin(phi) * Mathf.Sin(theta);
                float y = Mathf.Cos(phi);

                Vector3 localDirection = new Vector3(x, y, z).normalized;
                Vector3 finalDirection = roundOffset * localDirection;

                Quaternion lookRotation = Quaternion.LookRotation(finalDirection);
                Instantiate(radialProjectile, radialAttackPoint.position, lookRotation);
            }

            yield return new WaitForSeconds(fireRate);
        }

        EndAttack();
    }



    #endregion

    #region Melee

    private void MeleeAttack()
    {
        StartAttack(0);
        canMeleeAttack = false;
        meleeTelegraph.transform.localScale = Vector3.one;
        meleeTelegraphSFX.Play();
        meleeTelegraph.SetActive(true);
        StartCoroutine(MeleeTelegraph());
    }

    IEnumerator MeleeTelegraph()
    {
        Vector3 targetScale = new Vector3(9f, 9f, 9f);
        while (meleeTelegraph.transform.localScale.y < 9f)
        {
            meleeTelegraph.transform.localScale = Vector3.MoveTowards(meleeTelegraph.transform.localScale, targetScale, telegraphScaleSpeed * Time.deltaTime);
            yield return null;
        }

        DoMeleeAttack();
    }

        
    private void DoMeleeAttack()
    {
        MeleeDealDamage();
        Instantiate(meleeVFX, expandingAttackPosition.transform.position, expandingAttackPosition.transform.rotation, transform);
        meleeTelegraph.SetActive(false);
    }

    private void MeleeDealDamage()
    {
        if (playerDistance >= 25) { EndAttack(); StartCoroutine(MeleeAttackCooldown()); return; }

        RaycastHit hit;
        if (Physics.Raycast(shootPoint.transform.position, directionToPlayer, out hit, playerDistance + 1, meleeLayerMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                aMainSystem.DealDamage(hit.transform.gameObject, meleeDamage, false);

                //Push the player
                hit.transform.GetComponent<Locomotion2>().Push();
                Vector3 tempDirection = directionToPlayer.normalized;
                Vector3 force = tempDirection * meleeForce;
                force.y = meleeForceY;
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
            }
        }

        StartCoroutine(MeleeAttackCooldown());
        EndAttack();
    }

    IEnumerator MeleeAttackCooldown()
    {
        yield return new WaitForSeconds(5);
        canMeleeAttack = true;
    }


    #endregion

    #region Beam

    private void BeamAttack()
    {
        StartAttack(0);
        canBeamAttack = false;
        StartCoroutine(BeamTelegraph());
    }

    IEnumerator BeamTelegraph()
    {

        Instantiate(beamTelegraph, beamTelegraphPosition.position, beamTelegraphPosition.rotation, beamTelegraphPosition);
        yield return new WaitForSeconds(1.5f);
        StartCoroutine(BeamAttackCoroutine());
    }

    IEnumerator fade()
    {
        yield return new WaitForSeconds(.1f);
        
    }

    private IEnumerator BeamAttackCoroutine()
    {
        float duration = 4f;
        float timer = 0f;
        beamLineRender.enabled = true;
        beamAttackSFX.Play();


        beamLineRender.enabled = true;
        beamLineRender.SetPosition(0, shootPoint.position);
        beamLineRender.SetPosition(1, shootPoint.position + shootPoint.forward * 100f);

        while (timer < duration)
        {
            if (!HasLineOfSight()) yield return null;

            RotateTowardsPlayer();

            // Update damage timer
            if (beamDamageRateTimer < beamDamageRate)
                beamDamageRateTimer += Time.deltaTime;

            // Smooth aim adjustment
            Vector3 playerDirection = (player.transform.position - shootPoint.position).normalized;

            // Check dot product limit
            float alignment = Vector3.Dot(shootPoint.forward, playerDirection);
            
            // Smooth aim adjustment within allowed angle
            Vector3 horizontalAim = Vector3.RotateTowards(shootPoint.forward, playerDirection, yawSpeed * Time.deltaTime, maxBeamFollowSpeed);
            Vector3 verticalAim = Vector3.RotateTowards(shootPoint.forward, playerDirection, pitchSpeed * Time.deltaTime, maxBeamFollowSpeed);

            Vector3 combinedDirection = new Vector3(horizontalAim.x, verticalAim.y, horizontalAim.z);
            shootPoint.rotation = Quaternion.LookRotation(combinedDirection);
            

            // Default hit position far ahead
            Vector3 hitPosition = shootPoint.position + shootPoint.forward * 100f;

            // Check for actual hit
            if (Physics.Raycast(shootPoint.position, shootPoint.forward, out RaycastHit hit, 100f))
            {
                hitPosition = hit.point;

                if (hit.transform.CompareTag("Player") && beamDamageRateTimer >= beamDamageRate)
                {
                    aMainSystem.DealDamage(hit.transform.gameObject, beamDamage, false);
                    beamDamageRateTimer = 0f;
                }
            }

            // Update laser line
            beamLineRender.SetPosition(0, shootPoint.position);
            beamLineRender.SetPosition(1, hitPosition);

            // Wait for next frame
            timer += Time.deltaTime;
            yield return null;
        }

        // Turn off beam after duration ends
        beamLineRender.enabled = false;
        StartCoroutine(BeamAttackCooldown());
        EndAttack();
    }

    IEnumerator BeamAttackCooldown()
    {
        yield return new WaitForSeconds(5);
        canBeamAttack = true;
    }

    #endregion

    private void RotateTowardsPlayer()
    {
        //if (!HasLineOfSight()) { return; }

        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}