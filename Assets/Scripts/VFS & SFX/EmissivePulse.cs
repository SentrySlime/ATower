using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissivePulse : MonoBehaviour
{
    Material pulseMaterial;

    public float pulseSpeed = 0.4f;
    public float emissionIntensity = 1.2f; 
    Color emissionColor = Color.white;

    void Start()
    {
        pulseMaterial = GetComponent<Renderer>().material;
        pulseMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        Color currentColor = emissionColor * pulse * emissionIntensity;
        pulseMaterial.SetColor("_EmissionColor", currentColor);
    }
}
