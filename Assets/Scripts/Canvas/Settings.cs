using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public GameObject settingsPanel;

    public Toggle psxToggle;
    public Slider regularFov;

    [Header("Sensitivity")]
    public Slider sensslider;
    public TextMeshProUGUI sensValue;
    public float sensitivity = 1f;

    [Header("Volume")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeValue;
    float masterVolume = 1;

    GameObject player;
    CameraMovement cameraMovement;
    WeaponSocket weaponSocket;


    Camera mainCamera;
    PSXEffects psxEffects;

    public PSXEffects mainMenuPsxEffects;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if(player)
        {
            cameraMovement = player.GetComponent<CameraMovement>();
            weaponSocket = player.GetComponent<WeaponSocket>();
        
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            psxEffects = mainCamera.GetComponentInChildren<PSXEffects>();

        }

        sensitivity = PlayerPrefs.GetFloat("Sens", sensitivity);
        sensslider.value = sensitivity;

        if(weaponSocket)
            weaponSocket.SetFOVnSens();


        masterVolume = PlayerPrefs.GetFloat("Volume", masterVolume);
        volumeSlider.value = masterVolume;

        settingsPanel.SetActive(false);
    }

    public void EnableSetting()
    {
        settingsPanel.SetActive(true);
    }

    public void UpdatePSX()
    {
        
        if(psxEffects)
            psxEffects.enabled = psxToggle.isOn;
        else
            if(mainMenuPsxEffects)
            mainMenuPsxEffects.enabled = psxToggle.isOn;
    }

    private void SetSliders()
    {
        regularFov.maxValue = 120;
        regularFov.minValue = 60;
        regularFov.value = mainCamera.fieldOfView;
    }

    public void UpdateAudio()
    {
        masterVolume = volumeSlider.value;
        PlayerPrefs.SetFloat("Volume", masterVolume);
        volumeSlider.value = Mathf.Round(volumeSlider.value * 100f) / 100f;
        volumeValue.text = volumeSlider.value.ToString();
        AudioListener.volume = masterVolume;
    }

    public void UpdateSensitivity()
    {
        sensitivity = sensslider.value;
        PlayerPrefs.SetFloat("Sens", sensitivity);
        sensslider.value = Mathf.Round(sensslider.value * 100f) / 100f;
        sensValue.text = sensslider.value.ToString();
            
        //weaponSocket.baseSensitivity = sensitivity;
        if(weaponSocket)
            weaponSocket.SetFOVnSens();
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }
}