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
    public TextMeshProUGUI PitemName;
    public TextMeshProUGUI PitemDescription;
    public Image Pimage;


    [Header("ChildReferences")]
    public CanvasGroup damageVignette;
    public CanvasGroup shieldVignette;

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
                ResumeGame(true);

                return;
            }

            if (pauseMenu.activeInHierarchy)
            {
                ResumeGame(false);
            }
            else
            {
                paused = true;
                cmMovement.paused = true;
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                pauseMenu.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (pauseMenu.activeInHierarchy)
                return;

            if (inventoryMenu.activeInHierarchy)
            {
                ResumeGame(true);
            }
            else
            {
                paused = true;
                cmMovement.paused = true;
                Time.timeScale = 0;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                inventoryMenu.SetActive(true);
            }
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

    public void ResumeGame(bool inventory)
    {
        paused = false;
        cmMovement.paused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        if(inventory)
            inventoryMenu.SetActive(false);
        else
            pauseMenu.SetActive(false);
    }

    public void ResumeGame()
    {
        paused = false;
        cmMovement.paused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        inventoryMenu.SetActive(false);
        pauseMenu.SetActive(false);
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
        pauseMenu.SetActive(false);
        inventoryMenu.SetActive(false);
    }

    private void SetPlayerObj()
    {
        FindAndEquipWeapons tempFind = player.GetComponent<FindAndEquipWeapons>();
        tempFind.itemGroup = PitemGroup;
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
