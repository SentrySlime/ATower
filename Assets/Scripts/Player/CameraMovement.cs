using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;

public class CameraMovement : MonoBehaviour
{
    public GameObject rootObj;
    public bool paused = false;

    [Header("Sensitivity")]
    public float sensitivity = 2f;
    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }

    [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
    [Range(0f, 90f)] [SerializeField] float yRotationLimit = 88f;

    public Vector2 rotation = Vector2.zero;

    [Header("DistortionEffect")]
    public float moveSpeed = 1;
    public GameObject image1;
    public Vector3 pos1;
    public GameObject image2;
    public Vector3 pos2;
    public GameObject image3;
    public Vector3 pos3;

    const string xAxis = "Mouse X";
    const string yAxis = "Mouse Y";

    GameObject player;
    Transform playerPos;
    PlayerHealth playerHealth;
    public WeaponSocket weaponSocket;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerPos = player.GetComponent<Transform>();
        playerHealth = player.GetComponent<PlayerHealth>();
        weaponSocket = GameObject.FindObjectOfType<WeaponSocket>();
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1);
    }

    private void Start()
    {
        sensitivity = PlayerPrefs.GetFloat("Sens", sensitivity);
    }

    private void LateUpdate()
    {
        if (paused || playerHealth.dead) { return; }

        rotation.x += Input.GetAxis(xAxis) * sensitivity;
        rotation.y += Input.GetAxis(yAxis) * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        transform.localRotation = yQuat;
        rootObj.transform.localRotation = xQuat;

    }

    private void MoveImages()
    {

        pos1.x += Input.GetAxis(xAxis) * moveSpeed;
        pos2.x += Input.GetAxis(xAxis) * moveSpeed;
        pos3.x += Input.GetAxis(xAxis) * moveSpeed;
        image1.transform.position = new Vector3(pos1.x, pos1.y, pos1.z);
        image2.transform.position = new Vector3(pos2.x, pos2.y, pos2.z);
        image3.transform.position = new Vector3(pos3.x, pos3.y, pos3.z);
    }

}