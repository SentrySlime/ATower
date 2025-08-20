using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRoom : MonoBehaviour
{
    public GameObject roomLock;
    public ParticleSystem VFX;
    public AudioSource SFX;
    public RoomManager roomManager;
    public RoomScript roomScript;

    public GameObject gate;

    public GameObject specialRoomMesh;
    public GameObject flashMesh;
    Material material;
    Material flashMaterial;

    float fadeDuration = 2f;

    float moveSpeed = 5f;

    bool unlocked = false;

    private void Start()
    {
        material = specialRoomMesh.GetComponent<Renderer>().material;
        flashMaterial = flashMesh.GetComponent<Renderer>().material;
        flashMesh.SetActive(false);
        //StartCoroutine(StartReleaseLock());
    }

    IEnumerator StartReleaseLock()
    {
        yield return new WaitForSeconds(2f);
        IniateLockRelease();
    }

    public void IniateLockRelease()
    {
        if (unlocked) return;
        if (SFX)
            SFX.Play();
        unlocked = true;
        StartCoroutine(ReleaseLock());
    }

    IEnumerator ReleaseLock()
    {

        //StartCoroutine(FadeEmission());

        StartCoroutine(MoveGate());
        

        yield return new WaitForSeconds(fadeDuration);

        if (VFX)
            VFX.Play();

        //StartCoroutine(FadeFlash());
        //StartCoroutine(FadeAlpha());
    }

    private IEnumerator MoveGate()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {

            gate.transform.position += transform.up * moveSpeed * Time.deltaTime;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }

    private IEnumerator FadeEmission()
    {
        float elapsedTime = 0f;
        Color startColor = Color.red;
        Color endColor = Color.white;

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            Color currentColor = Color.Lerp(startColor, endColor, t);
            material.SetColor("_Emission", currentColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        material.SetColor("_Emission", endColor);
    }


    private IEnumerator FadeFlash()
    {
        

        flashMesh.SetActive(true);

        float elapsedTime = 0f;

        Color startColor = flashMaterial.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            flashMaterial.color = Color.Lerp(startColor, targetColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        flashMaterial.color = targetColor;
        roomLock.SetActive(false);
    }

    private IEnumerator FadeAlpha()
    {
        float elapsedTime = 0f;

        Color startColor = material.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            material.color = Color.Lerp(startColor, targetColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        material.color = targetColor;
        roomLock.SetActive(false);
    }

}