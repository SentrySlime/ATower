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


    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        shootPoint = GameObject.FindGameObjectWithTag("ShootPoint");
        cmMovement = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>();
        SetPlayerObj();
        StartCoroutine(HideMenu());
    }

    void Update()
    {
        //if (dissapearTimer < .25f)
        //{
        //    dissapearTimer += Time.deltaTime;
        //}
        //else
        //{
        //    panel.SetActive(false);
        //}


        //RaycastHit hit;
        //if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out hit, 5))
        //{
        //    if (hit.transform.CompareTag("Item"))
        //    {
        //        dissapearTimer = 0;
        //        var tempItem = hit.transform.GetComponent<ItemPickUp>();
        //        if (tempItem)
        //        {
        //            if (!panel.activeInHierarchy)
        //                panel.SetActive(true);

        //            itemName.text = tempItem.itemName;
        //            itemDescription.text = tempItem.itemDescription;
        //            image.sprite = tempItem.itemIcon;

        //        }


        //    }
        //    else
        //    {

        //    }

        //}


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.activeInHierarchy)
            {
                ResumeGame();
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

        if (Input.GetKeyDown(KeyCode.L))
        {

            Scene scene = SceneManager.GetActiveScene();
            if (SceneManager.GetSceneByBuildIndex(0) == scene)
                FelixStage();
            if (SceneManager.GetSceneByBuildIndex(1) == scene)
                SebbeStage();

        }
    }

    public void ResumeGame()
    {
        paused = false;
        cmMovement.paused = false;
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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

}
