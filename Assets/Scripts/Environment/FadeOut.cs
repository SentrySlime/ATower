using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{

    float fadeSpeed = 0.25f;
    float fadeAlpha = 1f;

    [HideInInspector] public GameObject endScreenButtons;
    CanvasGroup canvasToFadeIn;
    GameObject endText;
    Collider triggerCollider;

    private void Awake()
    {
        endScreenButtons = GameObject.Find("EndScreenButtons");
        canvasToFadeIn = GameObject.Find("FadeImageIn").GetComponent<CanvasGroup>();
        triggerCollider = GetComponent<Collider>();
        endText = GameObject.Find("EndText");
    }

    void Start()
    {
        DisableObjects();
        StartCoroutine(FadeIn());
    }

    private void DisableObjects()
    {
        endScreenButtons.SetActive(false);
        endText.GetComponent<TextMeshProUGUI>().enabled = false;
    }

    IEnumerator FadeIn()
    {
        fadeAlpha = 1;

        while (fadeAlpha > 0)
        {
            fadeAlpha -= Time.deltaTime * fadeSpeed;
            fadeAlpha = Mathf.Clamp01(fadeAlpha); 
            canvasToFadeIn.alpha = fadeAlpha;

            yield return null; 
        }

        yield return new WaitForSeconds(0.5f);
    }

}
