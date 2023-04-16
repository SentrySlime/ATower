using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public bool paused;
    public GameObject pausePanel;

    public Toggle psxToggle;
    public Toggle fishEyeToggle;

    public Slider regularFov;
    public Slider fishEyeFov;

    Camera mainCamera;
    GameObject player;
    PSXEffects psxEffects;
    WideCameraProjector fishEyeEffect;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        psxEffects = mainCamera.GetComponent<PSXEffects>();
        fishEyeEffect = mainCamera.GetComponent<WideCameraProjector>();

        SetSliders();

        pausePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseFunction();
        }


        if(paused)
            UpdateSettings();
    }

    private void UpdateSettings()
    {
        //-----  Major Effects -----\\
        psxEffects.enabled = psxToggle.isOn;
        fishEyeEffect.enabled = fishEyeToggle.isOn;

        //-----  Field of View -----\\
        mainCamera.fieldOfView = regularFov.value;
        fishEyeEffect.fieldOfView = fishEyeFov.value;

    }

    private void PauseFunction()
    {
        if (paused)
        {
            paused = false;
            pausePanel.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            paused = true;
            pausePanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void SetSliders()
    {
        regularFov.maxValue = 120;
        regularFov.minValue = 60;
        regularFov.value = mainCamera.fieldOfView;

        fishEyeFov.maxValue = 180;
        fishEyeFov.minValue = 90;
        fishEyeFov.value = fishEyeEffect.fieldOfView;

    }
}
