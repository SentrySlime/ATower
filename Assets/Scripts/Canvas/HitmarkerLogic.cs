using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitmarkerLogic : MonoBehaviour
{
    private GameObject hitMarker;
    private GameObject critHitMarker;

    void Start()
    {
        hitMarker = transform.GetChild(0).gameObject;
        critHitMarker = transform.GetChild(1).gameObject;
    }

    public void EnableCritHitMarker()
    {
        StopCoroutine(DisableCritHitmarker());

        critHitMarker.SetActive(true);

        StartCoroutine(DisableCritHitmarker());
    }

    IEnumerator DisableCritHitmarker()
    {
        yield return new WaitForSeconds(.2f);

        critHitMarker.SetActive(false);
    }

    public void EnableHitMarker()
    {
        StopCoroutine(DisableHitmarker());

        hitMarker.SetActive(true);

        StartCoroutine(DisableHitmarker());
    }

    IEnumerator DisableHitmarker()
    {
        yield return new WaitForSeconds(.2f);

        hitMarker.SetActive(false);
    }

}