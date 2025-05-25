using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Pause_Button_Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject image;
    public AudioSource hoverSFX;
    public AudioSource clickSFX;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(image) 
            image.SetActive(true);

        if(hoverSFX)
            hoverSFX.Play();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (image)
            image.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSFX)
            clickSFX.Play();
    }
}
