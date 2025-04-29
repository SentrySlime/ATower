using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissivePulse : MonoBehaviour
{
    Material pulseMaterial;

    public float pulseSpeed = 0.4f;
    public float emissionIntensity = 1.2f; 
    public Color emissionColor = Color.white;

    [Range(0f, 1f)] public float minPulse = 0.2f;
    [Range(0f, 1f)] public float maxPulse = 1f;

    void Start()
    {
        pulseMaterial = GetComponent<Renderer>().material;
        pulseMaterial.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        float rawPulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
        float scaledPulse = Mathf.Lerp(minPulse, maxPulse, rawPulse);

        Color currentColor = emissionColor * scaledPulse * emissionIntensity;
        pulseMaterial.SetColor("_Emission", currentColor);
    }
}
