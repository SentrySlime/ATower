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

    public bool IsBlacksmithMusicPlaying = false;
    [SerializeField] AudioSource blackSmithMusic;

    public bool IsAmbiensPlaying = false;
    [SerializeField] AudioSource ambiens;

    float fadeDuration = 1.5f;
    private Coroutine currentFade;

    private float musicVolume = 1;

    void Start()
    {
 
        devilMusic.volume = musicVolume * 0f;
        
        shopMusic.volume = musicVolume * 0f;

        if(blackSmithMusic)
            blackSmithMusic.volume = musicVolume * 0f;

        mainMusic.Play();
        devilMusic.Play();
        shopMusic.Play();
        ambiens.Play();

        if (blackSmithMusic)
            blackSmithMusic.Play();

        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMusic();
        }
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
        else if (IsBlacksmithMusicPlaying)
        {
            IsBlacksmithMusicPlaying = false;
            StartFade(mainMusic, blackSmithMusic);
        }
        else if (IsAmbiensPlaying)
        {
            IsAmbiensPlaying = false;
            StartFade(mainMusic, ambiens);
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

    public void TriggerBlacksmithMusic()
    {
        IsBlacksmithMusicPlaying = true;
        StartFade(blackSmithMusic, mainMusic);
    }

    public void TriggerAmbiens()
    {
        IsAmbiensPlaying = true;
        StartFade(ambiens, mainMusic);
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
            fadeIn.volume = Mathf.Lerp(fadeInStart, musicVolume * 0.125f, t);
            fadeOut.volume = Mathf.Lerp(fadeOutStart, musicVolume * 0f, t);
            yield return null;
        }

        fadeIn.volume = musicVolume * 0.125f;
        fadeOut.volume = musicVolume * 0f;
    }

    private void ToggleMusic()
    {
        if (musicVolume == 1)
            musicVolume = 0;
        else
            musicVolume = 1;

        if(IsMainMusicPlaying)
            mainMusic.volume = musicVolume * 0.125f;
        else if (IsDevilMusicPlaying)
            devilMusic.volume = musicVolume * 0.125f;
        else if (IsShopMusicPlaying)
            shopMusic.volume = musicVolume * 0.125f;
        else if (IsBlacksmithMusicPlaying)
            blackSmithMusic.volume = musicVolume * 0.125f;
        else if (IsAmbiensPlaying)
            ambiens.volume = musicVolume * 0.125f;

    }

}
