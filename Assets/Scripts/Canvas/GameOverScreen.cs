using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverScreen : MonoBehaviour
{
    GameObject player;
    WeaponSocket weaponSocket;
    FadeOut fadeOutScript;
    GameObject endScreenButtons;
    GameObject endText;
    TextMeshProUGUI endTextMeshPro;
    GameObject hudPanel;
    public CanvasGroup fadeOut;

    private void Awake()
    {
        
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        fadeOutScript = GameObject.Find("EndScreen").GetComponent<FadeOut>();
        endText = GameObject.Find("EndText");
        endTextMeshPro = endText.GetComponent<TextMeshProUGUI>();
        hudPanel = transform.Find("HUD_Panel").gameObject;
        endScreenButtons = fadeOutScript.endScreenButtons;
        weaponSocket = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponSocket>();
    }

    
    void Update()
    {
        
    }

    public void GameOver()
    {
        DisplayMouse();
        EnableEndText();
    }

    private void EnableEndText()
    {
        hudPanel.SetActive(false);
        endTextMeshPro.text = "Game Over";
        fadeOut.alpha = 0.5f;
        endTextMeshPro.enabled = true;
        if (endScreenButtons)
            endScreenButtons.SetActive(true);
    }

    private void DisplayMouse()
    {
        weaponSocket.interupptedBool = true;
        weaponSocket.equippedWeapon.HideWeapon();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

}
