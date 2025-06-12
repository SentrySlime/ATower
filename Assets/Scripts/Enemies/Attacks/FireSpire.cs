using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpire : MonoBehaviour
{
    float fadeDuration = 1.8f;
    Material material;
    public Collider dmgCollider;
    public ParticleSystem one;
    public ParticleSystem two;
    public ParticleSystem three;
    public AudioSource fireSFX;

    private void Awake()
    {
        material = GetComponentInChildren<Renderer>().material;

        StartCoroutine(FadeAlpha());
    }

    private IEnumerator FadeAlpha()
    {
        yield return new WaitForSeconds(8);

        dmgCollider.enabled = false;
        fireSFX.enabled = false;
        TurnOfEmission();

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
    }

    private void TurnOfEmission()
    {
        ParticleSystem ps1 = one; 
        ParticleSystem.EmissionModule emission1 = ps1.emission;
        emission1.rateOverTime = 0f;

        ParticleSystem ps2 = two; 
        ParticleSystem.EmissionModule emission2 = ps2.emission;
        emission2.rateOverTime = 0f;

        ParticleSystem ps3 = three;
        ParticleSystem.EmissionModule emission3 = ps3.emission;
        emission3.rateOverTime = 0f;
    }
}