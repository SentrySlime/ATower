using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{

    public Image borderImage;
    public Image iconImage;
    public TextMeshProUGUI hotkeyIndex;
    public Color color;

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
        borderImage.color = color;
        //SetHeirarchy();
    }

    public void SetInactive()
    {
        borderImage.color = Color.white;
    }

    public void SetHotKeyIndex(int index)
    {
        hotkeyIndex.text = index.ToString();
    }
}