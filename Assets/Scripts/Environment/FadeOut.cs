using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{

    public float fadeSpeed = 1;

    bool fadeOut = false;
    bool fadeIn = false;

    CanvasGroup canvasToFade;
    public GameObject endText;

    void Start()
    {
        canvasToFade = GameObject.Find("FadeImage").GetComponent<CanvasGroup>();
        endText = GameObject.Find("EndText");
        Invoke("StartFadeIn", 0.35f);
    }

    
    void Update()
    {
        if (fadeOut)
            canvasToFade.alpha += Time.deltaTime * fadeSpeed;

        if (fadeIn)
            canvasToFade.alpha -= Time.deltaTime * fadeSpeed;

        if (canvasToFade.alpha == 0)
        {
            fadeIn = false;
        }

        if (canvasToFade.alpha == 1 && fadeOut)
        {
            Invoke("EnableEndText", 0.5f);
            //SceneManager.LoadScene(1);
        }

    }

    private void StartFadeIn()
    {
        fadeIn = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            fadeOut = true;
        }
    }

    private void EnableEndText()
    {
        endText.GetComponent<TextMeshProUGUI>().enabled = true;
    }

}
