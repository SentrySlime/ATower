using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeper : MonoBehaviour, IInteractInterface
{
   

    GameObject canvas;
    PauseMenu pauseMenu;
    GameObject shopPanel;
    ShopPanel shopPanelScript;


    private void Awake()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        pauseMenu = canvas.GetComponent<PauseMenu>();
        shopPanel = pauseMenu.shopPanel;
        shopPanelScript = shopPanel.GetComponent<ShopPanel>();
    }

    private void Start()
    {
        //shopPanel.GetComponent<ShopPanel>().PopulateShop();
    }

    public void Interact()
    {
        if (pauseMenu.paused) return;

        shopPanel.SetActive(true);
        pauseMenu.PauseGame();
        shopPanelScript.SetFirstItemDisplay();
    }    
}