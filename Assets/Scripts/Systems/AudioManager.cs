using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    
    [SerializeField] AudioSource mainMusic;
    [SerializeField] AudioSource devilMusic;
    float fadeDuration = 1.5f;

    private Coroutine currentFade;

    float mainMusicVolume;
    float devilMusicVolume;

    void Start()
    {
        mainMusicVolume = mainMusic.volume;

        devilMusicVolume = devilMusic.volume;
        devilMusic.volume = 0f;

        mainMusic.Play();
        devilMusic.Play(); 

    }

    public void TriggerMainMusic()
    {
        StartFade(mainMusic, devilMusic);
    }

    public void TriggerDevilMusic()
    {
        StartFade(devilMusic, mainMusic);
    }

    private void StartFade(AudioSource fadeIn, AudioSource fadeOut)
    {
        
        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeMusic(fadeIn, fadeOut));
    }

    private IEnumerator FadeMusic(AudioSource fadeIn, AudioSource fadeOut)
    {
        float time = 0f;
        float fadeInStart = fadeIn.volume;
        float fadeOutStart = fadeOut.volume;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            fadeIn.volume = Mathf.Lerp(fadeInStart, 0.125f, t);
            fadeOut.volume = Mathf.Lerp(fadeOutStart, 0f, t);
            yield return null;
        }

        fadeIn.volume = 0.125f;
        fadeOut.volume = 0f;
    }

}
