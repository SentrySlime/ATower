using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndOfDemo : MonoBehaviour
{
    float fadeSpeed = 1;
    float fadeAlpha = 0f;
    bool triggered = false;

    FadeOut fadeOutScript;

    GameObject endScreenButtons;
    CanvasGroup canvasToFadeOut;
    GameObject endText;
    WeaponSocket weaponSocket;

    void Start()
    {
        endText = GameObject.Find("EndText");
        fadeOutScript = GameObject.Find("EndScreen").GetComponent<FadeOut>();
        canvasToFadeOut = GameObject.Find("FadeImageOut").GetComponent<CanvasGroup>();
        weaponSocket = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponSocket>();
        endScreenButtons = fadeOutScript.endScreenButtons;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) { return; }

        if (other.CompareTag("Player"))
        {
            triggered = true;
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        fadeAlpha = 0;

        while (fadeAlpha < 1)
        {
            fadeAlpha += Time.deltaTime * fadeSpeed;
            fadeAlpha = Mathf.Clamp01(fadeAlpha);
            canvasToFadeOut.alpha = fadeAlpha;

            yield return null;
        }

        yield return new WaitForSeconds(0.2f);
        DisplayMouse();
        EnableEndText();
    }

    private void EnableEndText()
    {
        endText.GetComponent<TextMeshProUGUI>().enabled = true;
        if (endScreenButtons)
            endScreenButtons.SetActive(true);
    }

    private void DisplayMouse()
    {
        weaponSocket.interupptedBool = triggered;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
