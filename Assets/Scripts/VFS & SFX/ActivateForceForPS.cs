using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateForceForPS : MonoBehaviour
{
    public ParticleSystem ps;

    private void Start()
    {
        StartCoroutine(ActivateForce());
    }

    IEnumerator ActivateForce()
    {
        yield return new WaitForSeconds(1);
        var mainModule = ps.externalForces;
        mainModule.multiplier = 65;
        mainModule.enabled = true;
    }
}
