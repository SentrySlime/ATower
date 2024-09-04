using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitmarkerLogic : MonoBehaviour
{
    private GameObject hitMarker;

    void Start()
    {
        hitMarker = transform.GetChild(0).gameObject;
    }


    void Update()
    {

    }

    public void EnableHitMarker()
    {
        StopAllCoroutines();

        hitMarker.SetActive(true);

        StartCoroutine(DisableHitmarker());
    }

    IEnumerator DisableHitmarker()
    {

        yield return new WaitForSeconds(.1f);

        hitMarker.SetActive(false);
    }


}

