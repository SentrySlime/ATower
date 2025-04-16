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
    bool triggered = false;

    CanvasGroup canvasToFadeIn;
    CanvasGroup canvasToFadeOut;
    //CanvasGroup canvasToFade;
    public GameObject endText;
    public GameObject endScreenButtons;

    public Collider triggerCollider;

    void Start()
    {
        canvasToFadeIn = GameObject.Find("FadeImageIn").GetComponent<CanvasGroup>();
        canvasToFadeOut = GameObject.Find("FadeImageOut").GetComponent<CanvasGroup>();
        //canvasToFade = GameObject.Find("FadeImage").GetComponent<CanvasGroup>();
        
        endScreenButtons = GameObject.Find("EndScreenButtons");
        endScreenButtons.SetActive(false);

        endText = GameObject.Find("EndText");
        Invoke("StartFadeIn", 0.35f);
    }

    
    void Update()
    {
        if (fadeIn)
            canvasToFadeIn.alpha -= Time.deltaTime * fadeSpeed;
        
        if (fadeOut)
            canvasToFadeOut.alpha += Time.deltaTime * fadeSpeed;

        if (canvasToFadeIn.alpha == 0)
        {
            fadeIn = false;
        }

        if (canvasToFadeOut.alpha == 1 && fadeOut)
        {
            Invoke("EnableEndText", 0.5f);
            Invoke("PauseGame", 0.5f);
        }

    }

    private void StartFadeIn()
    {
        fadeIn = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) { return; }

        triggerCollider.isTrigger = false;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            fadeOut = true;
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void EnableEndText()
    {
        endText.GetComponent<TextMeshProUGUI>().enabled = true;
        endScreenButtons.SetActive(true);
    }

}
