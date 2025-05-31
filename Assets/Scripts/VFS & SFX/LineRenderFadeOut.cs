using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRenderFadeOut : MonoBehaviour
{

    LineRenderer lineRenderer;    

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        StartCoroutine(FadeLine(0.5f));
    }

    IEnumerator FadeLine(float duration)
    {
        float elapsed = 0f;

        Color startColor = lineRenderer.startColor;
        Color endColor = lineRenderer.endColor;

        while (elapsed < duration)
        {
            float t = 1f - (elapsed / duration);
            Color newStartColor = new Color(startColor.r, startColor.g, startColor.b, t);
            Color newEndColor = new Color(endColor.r, endColor.g, endColor.b, t);

            lineRenderer.startColor = newStartColor;
            lineRenderer.endColor = newEndColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Optional: disable or destroy line after fade
        lineRenderer.enabled = false;
    }
}
