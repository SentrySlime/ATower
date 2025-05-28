using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitmarkerLogic : MonoBehaviour
{
    private GameObject hitMarker;
    private GameObject critHitMarker;

    private float hitMarkerTimer = 0f;
    private float critHitMarkerTimer = 0f;

    private const float markerDuration = 0.2f;

    void Start()
    {
        hitMarker = transform.GetChild(0).gameObject;
        critHitMarker = transform.GetChild(1).gameObject;
    }

    void Update()
    {
        if (hitMarker.activeSelf)
        {
            hitMarkerTimer -= Time.deltaTime;
            if (hitMarkerTimer <= 0f)
                hitMarker.SetActive(false);
        }

        if (critHitMarker.activeSelf)
        {
            critHitMarkerTimer -= Time.deltaTime;
            if (critHitMarkerTimer <= 0f)
                critHitMarker.SetActive(false);
        }
    }

    public void EnableHitMarker()
    {
        if (!hitMarker.activeSelf)
            hitMarker.SetActive(true);

        hitMarkerTimer = markerDuration;
    }


    public void EnableCritHitMarker()
    {
        critHitMarker.SetActive(true);
        critHitMarkerTimer = markerDuration; // Extend timer every crit
    }
}
