using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIcon : MonoBehaviour
{
    public Sprite selectedImageBorder;
    public Sprite imageBorder;

    public Image borderImage;
    public Image iconImage;
    public TextMeshProUGUI hotkeyIndex;
    public Color color;

    public CanvasGroup canvasGroup;
    public Image fillImage;

    public float reloadTime = 0;
    public float reloadTimeTimer = 0;

    [Header("AmmoText")]
    public TextMeshProUGUI currentAmmoText;
    public TextMeshProUGUI ammoDivider;
    public TextMeshProUGUI maxAmmoText;

    private void Update()
    {
        if (canvasGroup != null)
            DecreaseAlpha();
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
        borderImage.sprite = selectedImageBorder;
        //borderImage.color = color;
        //SetHeirarchy();
    }

    public void SetInactive()
    {
        borderImage.sprite = imageBorder;
        //borderImage.color = Color.white;
    }

    public void SetHotKeyIndex(int index)
    {
        hotkeyIndex.text = index.ToString();
    }

    public void DisplayFinishedReload()
    {
        canvasGroup.alpha = 0.75f;
        fillImage.fillAmount = 1;
    }

    private void DecreaseAlpha()
    {

        if (fillImage.fillAmount > 0)
        {
            fillImage.fillAmount -= (1f / reloadTime) * Time.deltaTime;
        }
        else
        {
            canvasGroup.alpha = 0;
        }

    }

    public void DisableReloadIcon()
    {
        fillImage.fillAmount = 0;
        canvasGroup.alpha = 0;
    }
}