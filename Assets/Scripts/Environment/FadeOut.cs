using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeOut : MonoBehaviour
{

    public float fadeSpeed = 1;

    bool startFade = false;

    CanvasGroup canvasToFade;

    void Start()
    {
        canvasToFade = GameObject.Find("FadeImage").GetComponent<CanvasGroup>();
    }

    
    void Update()
    {
        if (startFade)
            canvasToFade.alpha += Time.deltaTime * fadeSpeed;

        if(canvasToFade.alpha == 1)
        {
            SceneManager.LoadScene(1);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            startFade = true;
        }
    }

}
