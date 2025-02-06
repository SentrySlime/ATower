using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{

    public Image borderImage;
    public Image iconImage;

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void SetHeirarchy()
    {
        transform.SetAsFirstSibling();
    }

    public void SetIcon(Sprite icon)
    {
        iconImage.sprite = icon;
    }

    public void Activate()
    {
        borderImage.color = Color.red;
        SetHeirarchy();
    }

    public void SetInactive()
    {
        borderImage.color = Color.white;
    }
}
