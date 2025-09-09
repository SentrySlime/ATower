using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    GameObject player;
    GameObject shootPoint;
    public Settings settings;
    public GameObject shopPanel;
    public GameObject pauseMenu;
    public GameObject inventoryMenu;
    public bool paused = false;
    public CameraMovement cmMovement;
    //bool panelOn;
    public GameObject panel;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public Image image;

    GameObject itemPanel;
    float dissapearTimer;

    [Header("Player objects")]
    public CanvasGroup PitemGroup;
    public CanvasGroup PWeaponGroup;
    public TextMeshProUGUI PitemName;
    public TextMeshProUGUI PitemDescription;
    public Image Pimage;

    [Header("ChildReferences")]
    public CanvasGroup hudGroup;
    public CanvasGroup damageVignette;
    public CanvasGroup shieldVignette;
    public ItemShowCase itemShowCase;

    [Header("Settings")]
    public Slider sensitivitySlider;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        shootPoint = GameObject.FindGameObjectWithTag("ShootPoint");
        cmMovement = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        
        SetPlayerObj();
        
    }

    void Start()
    {
        StartCoroutine(HideMenu());
        pauseMenu.SetActive(false);
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inventoryMenu.activeInHierarchy)
            {
                ResumeGame(inventoryMenu);

                return;
            }

            if (pauseMenu.activeInHierarchy)
            {
                ResumeGame(pauseMenu);
                return;
            }


            if(shopPanel.activeInHierarchy)
            {
                ResumeGame(shopPanel);
                return;

            }
            else
            {
                PauseGame();
                pauseMenu.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (pauseMenu.activeInHierarchy || shopPanel.activeInHierarchy)
                return;


            if (inventoryMenu.activeInHierarchy)
            {
                ResumeGame(inventoryMenu);
            }
            else
            {
                PauseGame();
                inventoryMenu.SetActive(true);

                itemShowCase.SetFirstItemDisplay();
                
            }
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            if (hudGroup.alpha == 1)
                hudGroup.alpha = 0;
            else if (hudGroup.alpha == 0)
                hudGroup.alpha = 1;
        }

        //if (Input.GetKeyDown(KeyCode.L))
        //{

        //    Scene scene = SceneManager.GetActiveScene();
        //    if (SceneManager.GetSceneByBuildIndex(0) == scene)
        //        FelixStage();
        //    if (SceneManager.GetSceneByBuildIndex(1) == scene)
        //        SebbeStage();

        //}
    }

    public void PauseGame()
    {
        paused = true;
        cmMovement.paused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame(GameObject obj)
    {
        settings.CloseSettings();

        paused = false;
        cmMovement.paused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        obj.SetActive(false);
    }

    public void ResumeGame()
    {
        settings.CloseSettings();

        paused = false;
        cmMovement.paused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        inventoryMenu.SetActive(false);
        pauseMenu.SetActive(false);
        shopPanel.SetActive(false);
    }

    public void OpenInventoryFromPause()
    {
        inventoryMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void ResetGame()
    {
        ResumeGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        //var currentScene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(currentScene.ToString());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator HideMenu()
    {
        yield return new WaitForSeconds(0.1f);
        shopPanel.GetComponent<ShopPanel>().PopulateShop();
        yield return new WaitForSeconds(0.1f);
        pauseMenu.SetActive(false);
        inventoryMenu.SetActive(false);
        shopPanel.SetActive(false);
    }

    private void SetPlayerObj()
    {
        FindAndEquipWeapons tempFind = player.GetComponent<FindAndEquipWeapons>();
        tempFind.itemGroup = PitemGroup;
        tempFind.weaponGroup = PWeaponGroup;
        tempFind.itemName = PitemName;
        tempFind.itemDescription = PitemDescription;
        tempFind.image = Pimage;
    }

    public void SebbeStage()
    {
        SceneManager.LoadScene(0);
    }

    public void FelixStage()
    {
        SceneManager.LoadScene(1);
    }

    public void GetVignettes(PlayerHealth playerHP)
    {
        playerHP.damageVignette = damageVignette;
        playerHP.shieldVignette = shieldVignette;
    }



    public void Settings()
    {
        
    }

}
