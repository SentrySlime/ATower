using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obelisk : MonoBehaviour
{

    public GameObject obelisk;
    public RotateRoundAxis rotateRoundAxis1;
    public GameObject shards;
    public RotateRoundAxis rotateRoundAxis2;

    public bool rotateBack;
    public bool canRotate;

    [Header("Aura particles")]
    public GameObject cursedAura;
    public ParticleSystem cursedAuraVFX;
    public GameObject blessedAura;
    public ParticleSystem blessedAuraVFX;

    public ParticleSystem holyAura2;

    [Header("Ring particles")]
    public GameObject ring1;
    public ParticleSystem ring1PS;
    public GameObject ring2;
    public ParticleSystem ring2PS;
    public GameObject ring3;
    public ParticleSystem ring3PS;

    

    [Header("Obelisk")]
    public float currentRotation = 0;
    public int rotationSpeedForward;
    public float rotationSpeedBackwards;


    [Header("Shard")]
    public float currentShardRotation = 0;
    bool hasCalledShards = false;
    public ObeliskShard[] obeliskShards;

    [Header("Emissive pulses")]
    public EmissivePulse[] emissivePulses;

    void Start()
    {
        Invoke("SetBack", 8);
    }

    void Update()
    {

        if(canRotate)
        {
            if (rotateBack)
                RotateBack();
            else
                RotateForward();
        }

    }

    private void SetBack()
    {
        canRotate = false;
        StartCoroutine(SetBackCo());
    }

    IEnumerator SetBackCo()
    { 
        yield return new WaitForSeconds(0.3f);
        canRotate = true;
        rotateBack = true;
    }

    private void RotateBack()
    {
        rotationSpeedBackwards = Mathf.Max(currentRotation, 5f) * 8f;

        if (currentRotation > 0)
        {
            currentRotation += Time.deltaTime * -rotationSpeedBackwards;
            obelisk.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

            ring1.transform.rotation = Quaternion.Euler(45, currentRotation, currentRotation);
            ring2.transform.rotation = Quaternion.Euler(-45, currentRotation, currentRotation);
            ring3.transform.rotation = Quaternion.Euler(90, currentRotation, currentRotation);
        }

        if (currentShardRotation < 0)
        {
            currentShardRotation += Time.deltaTime * rotationSpeedBackwards;
            shards.transform.rotation = Quaternion.Euler(0, currentShardRotation, 0);
        }
        else if(!hasCalledShards)
        {
            shards.transform.rotation = Quaternion.identity;
            obelisk.transform.rotation = Quaternion.identity;
            StartCoroutine(FadeOutCursedAura());

            for (int i = 0; i < obeliskShards.Length; i++)
            {
                obeliskShards[i].StartMovement();
            }

            hasCalledShards = true;
        }
    }

    IEnumerator FadeOutCursedAura()
    {
        float duration = 1f; 
        float elapsed = 0f;

        ParticleSystem.MainModule main = cursedAuraVFX.main;
        ParticleSystem.MainModule ringps1 = ring1PS.main;
        ParticleSystem.MainModule ringps2 = ring2PS.main;
        ParticleSystem.MainModule ringps3 = ring3PS.main;

        Color startColor = main.startColor.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / duration);
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            main.startColor = newColor;
            ringps1.startColor = newColor;
            ringps2.startColor = newColor;
            ringps3.startColor = newColor;


            yield return null;
        }

        main.startColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        ringps1.startColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        ringps2.startColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        ringps3.startColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        StartCoroutine(FadeInBlessedAura());
        StartCoroutine(FadePulseColor());

        

    }

    IEnumerator FadePulseColor()
    {
        

        float duration = 0.1f;
        float elapsed = 0f;

        Color targetColor = new Color(1f, 1f, 1f, 1f);
        Color startColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);

        // Initialize all emissive pulses to transparent
        for (int i = 0; i < emissivePulses.Length; i++)
        {
            emissivePulses[i].emissionColor = startColor;
        }

        // Fade in all emissive pulses over duration
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Color newColor = Color.Lerp(startColor, targetColor, t);

            for (int i = 0; i < emissivePulses.Length; i++)
            {
                emissivePulses[i].emissionColor = newColor;
            }

            yield return null;
        }

        // Ensure final color is exactly set
        for (int i = 0; i < emissivePulses.Length; i++)
        {
            emissivePulses[i].emissionColor = targetColor;
        }
    }


    IEnumerator FadeInBlessedAura()
    {

        blessedAura.SetActive(true);
        blessedAuraVFX.Play();


        float duration = 1f;
        float elapsed = 0f;

        
        ParticleSystem.MainModule main = blessedAuraVFX.main;
        ParticleSystem.MainModule main2 = holyAura2.main;

        

        // Define the final target color you want to fade in to
        Color targetColor = new Color(1f, 1f, 1f, 1f); // Example: white, fully opaque
        Color startColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f); // Same color, 0 alpha

        // Set the initial transparent color
        main.startColor = startColor;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Color newColor = Color.Lerp(startColor, targetColor, t);
            main.startColor = newColor;
            main2.startColor = newColor;

            yield return null;
        }

        main.startColor = targetColor; 
        main2.startColor = targetColor; 
    }





    private void RotateForward()
    {   
        currentRotation += Time.deltaTime * rotationSpeedForward;

        obelisk.transform.rotation = Quaternion.Euler(0, currentRotation, 0);

        ring1.transform.rotation = Quaternion.Euler(45, currentRotation, currentRotation);
        ring2.transform.rotation = Quaternion.Euler(-45, currentRotation, currentRotation);
        ring3.transform.rotation = Quaternion.Euler(90, currentRotation, currentRotation);

        currentShardRotation += Time.deltaTime * -rotationSpeedForward;
        shards.transform.rotation = Quaternion.Euler(0, currentShardRotation, 0);

    }

}