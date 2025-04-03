using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Pause_Button_Hover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject image;

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.SetActive(false);
    }
}
