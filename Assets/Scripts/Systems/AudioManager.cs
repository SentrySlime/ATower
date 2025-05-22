using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public bool IsMainMusicPlaying = true;
    [SerializeField] AudioSource mainMusic;

    public bool IsDevilMusicPlaying = false;
    [SerializeField] AudioSource devilMusic;

    public bool IsShopMusicPlaying = false;
    [SerializeField] AudioSource shopMusic;
    float fadeDuration = 1.5f;

    private Coroutine currentFade;

    void Start()
    {
 
        devilMusic.volume = 0f;
        shopMusic.volume = 0f;

        mainMusic.Play();
        devilMusic.Play();
        shopMusic.Play();
    }

    public void TriggerMainMusic()
    {
        IsMainMusicPlaying = true;
        if(IsDevilMusicPlaying)
        {
            StartFade(mainMusic, devilMusic);
            IsDevilMusicPlaying = false;
        }
        else if(IsShopMusicPlaying)
        {
            IsShopMusicPlaying = false;
            StartFade(mainMusic, shopMusic);
        }

    }

    public void TriggerDevilMusic()
    {
        IsDevilMusicPlaying = true;
        StartFade(devilMusic, mainMusic);
    }

    public void TriggerShopMusic()
    {
        IsShopMusicPlaying = true;
        StartFade(shopMusic, mainMusic);
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
