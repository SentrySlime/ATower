using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSFX : MonoBehaviour
{

    public AudioSource audioSource;

    public AudioClip[] soundEffects;


    public bool shouldDestroy = false;

    [SerializeField]
    float MinPitch = 1.1f;

    [SerializeField]
    float MaxPitch = 1.25f;

    private void Awake()
    {
        //if(audioSource == null) { return; }

        if (soundEffects.Length > 0)
        {

            int tempNmb = Random.Range(0, soundEffects.Length);
            audioSource.clip = soundEffects[tempNmb];
            audioSource.pitch = Random.Range(MinPitch, MaxPitch);

        }



        audioSource.Play();

        if(shouldDestroy)
            StartCoroutine(DestroySound());
        
    }

    IEnumerator DestroySound()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        Destroy(gameObject);
    }

    

}
