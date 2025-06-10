using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlessingText : MonoBehaviour
{
    CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(FadeBlessing());
    }

    IEnumerator FadeBlessing ()
    {
        yield return new WaitForSeconds(4.5f);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= 1 * Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

}
