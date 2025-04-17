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

    void Update()
    {

    }

    public void EnableCritHitMarker()
    {
        StopCoroutine(DisableCritHitmarker());
        //StopAllCoroutines();

        critHitMarker.SetActive(true);

        StartCoroutine(DisableCritHitmarker());
    }

    IEnumerator DisableCritHitmarker()
    {
        yield return new WaitForSeconds(.1f);

        critHitMarker.SetActive(false);
    }

    public void EnableHitMarker()
    {
        StopCoroutine(DisableHitmarker());
        //StopAllCoroutines();

        hitMarker.SetActive(true);

        StartCoroutine(DisableHitmarker());
    }

    IEnumerator DisableHitmarker()
    {
        yield return new WaitForSeconds(.1f);

        hitMarker.SetActive(false);
    }

}