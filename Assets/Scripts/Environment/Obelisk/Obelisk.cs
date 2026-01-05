using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obelisk : MonoBehaviour, IInteractInterface
{
    GameObject gameManager;
    CurseManager curseManager;
    InteractInfo interactInfo;

    public GameObject obelisk;
    public GameObject shards;

    bool rotateBack;
    bool canRotate;
    [Header("CurseShards")]
    public InteractableObeliskShard[] obeliskShardsInteractable;

    public GameObject reward;

    [Header("Aura particles")]
    public GameObject cursedAura;
    public ParticleSystem cursedAuraVFX;
    public GameObject blessedAura;
    public ParticleSystem blessedAuraVFX;

    public ParticleSystem holyAura2;

    [Header("Ring particles")]
    public GameObject ring3;
    public ParticleSystem ring3PS;

    [Header("Obelisk")]
    float currentRotation = 0;
    int rotationSpeedForward = 100;
    float rotationSpeedBackwards = 1000;

    [Header("HolyMove")]
    public bool holyRotate = false;
    public int holyRotationSpeed = 1;
    private Vector3 startPosition;

    [Header("Shard")]
    float currentShardRotation = -720;
    bool hasCalledShards = false;
    public ObeliskShard[] obeliskShards;

    [Header("Emissive pulses")]
    public EmissivePulse[] emissivePulses;

    [Header("SFX")]
    public AudioSource cursedSFX;
    public AudioSource rewindSFX;
    public AudioSource slamSFX;
    public AudioSource holyStartSFX;
    public AudioSource blessingSFX;

    void Start()
    {
        startPosition = transform.position;
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        curseManager = gameManager.GetComponent<CurseManager>();
        interactInfo = GetComponent<InteractInfo>();
        SetObeliskShardsReward();
    }

    void Update()
    {

        float newY = startPosition.y + Mathf.Sin(Time.time * 1f) * 0.5f;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        if (canRotate)
        {
            // Float up and down
            

            if (holyRotate)
                HolyRotate();
            else if (rotateBack)
                RotateBack();
            else
                RotateForward();
        }
        

    }


    public void Interact()
    {
        for (int i = 0; i < obeliskShardsInteractable.Length; i++)
            obeliskShardsInteractable[i].gameObject.SetActive(false);

        gameObject.transform.tag = "Untagged";
        SetBack();
    }


    private void SetBack()
    {
        canRotate = false;
        StartCoroutine(SetBackCo());
    }

    IEnumerator SetBackCo()
    {
        StartCoroutine(RewindSFX());
        yield return new WaitForSeconds(0.3f);
        canRotate = true;
        rotateBack = true;
    }

    private void RotateBack()
    {
        StartCoroutine(CursedSFX());
        

        //rotationSpeedBackwards = Mathf.Max(currentRotation, 5f) * 8f;

        if (currentRotation > 0)
        {
            currentRotation += Time.deltaTime * -rotationSpeedBackwards;
            obelisk.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
            ring3.transform.rotation = Quaternion.Euler(90, currentRotation, currentRotation);
        }

        if (currentShardRotation < 0)
        {
            currentShardRotation += Time.deltaTime * rotationSpeedBackwards;
            shards.transform.rotation = Quaternion.Euler(0, currentShardRotation, 0);
        }
        else if(!hasCalledShards)
        {
            StartCoroutine(SlamSFX());

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
        ParticleSystem.MainModule ringps3 = ring3PS.main;

        Color startColor = main.startColor.color;

        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsed / duration);
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            main.startColor = newColor;
            ringps3.startColor = newColor;


            yield return null;
        }

        StartCoroutine(HolyStartSFX());
        main.startColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        ringps3.startColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        
        StartCoroutine(FadePulseColor());
        StartCoroutine(FadeInBlessedAura());
    }

    IEnumerator FadePulseColor()
    {
        float duration = 1f;
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

        StartCoroutine(BlessedSFX());
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

        
        holyRotate = true;
        //curseManager.SpawnCurse(reward);
    }


    private void RotateForward()
    {   
        currentRotation += Time.deltaTime * rotationSpeedForward;

        obelisk.transform.rotation = Quaternion.Euler(0, currentRotation, 0);
        ring3.transform.rotation = Quaternion.Euler(90, currentRotation, currentRotation);

        currentShardRotation += Time.deltaTime * -rotationSpeedForward;
        shards.transform.rotation = Quaternion.Euler(0, currentShardRotation, 0);

    }

    private void HolyRotate()
    {
        // Rotate
        currentRotation += Time.deltaTime * holyRotationSpeed;
        transform.rotation = Quaternion.Euler(0, currentRotation, 0);
    }

    IEnumerator CursedSFX()
    {
        yield return new WaitForSeconds(0.5f);

        cursedSFX.Stop();
    }

    IEnumerator HolyStartSFX()
    {
        yield return null;

        holyStartSFX.Play();
    }

    IEnumerator BlessedSFX()
    {
        yield return new WaitForSeconds(1f);

        float fadeInDuration = 1;

        blessingSFX.volume = 0f;
        blessingSFX.Play();

        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            blessingSFX.volume = Mathf.Lerp(0f, 0.5f, timer / fadeInDuration);
            yield return null;
        }

        blessingSFX.volume = 0.5f; // Ensure it's exactly 1 at the end
    }

    IEnumerator SlamSFX()
    {
        yield return null;

        slamSFX.Play();
    }

    IEnumerator RewindSFX()
    {
        yield return null;

        rewindSFX.Play();
    }

    private void SetObeliskShardsReward()
    {
        GameObject[] reward = curseManager.GetCurseRewards();

        for (int i = 0; i < obeliskShardsInteractable.Length; i++)
        {
            obeliskShardsInteractable[i].SetReward(reward[i], this);
        }
    }
}