using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CameraMovement2 : MonoBehaviour
{
    WeaponSocket weaponSocket;
    public GameObject rootObj;
    public Slider sensslider;
    public TextMeshProUGUI sensValue;
    private float currentSens = 0;
    private float previousSens = 0;
    public bool paused = false;
    public float Sensitivity
    {
        get { return sensitivity; }
        set { sensitivity = value; }
    }
    [Range(0.1f, 9f)] [SerializeField] float sensitivity = 2f;
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

    const string xAxis = "Mouse X"; //Strings in direct code generate garbage, storing and re-using them creates no garbage
    const string yAxis = "Mouse Y";

    Transform playerPos;

    private void Start()
    {
        //pos1 = image1.transform.position;
        //pos2 = image2.transform.position;
        //pos3 = image3.transform.position;
        weaponSocket = GetComponentInParent<WeaponSocket>();
        playerPos = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        sensslider = GameObject.FindGameObjectWithTag("Senstivity").GetComponent<Slider>();
        sensValue = GameObject.Find("Sensitivity_Text").GetComponent<TextMeshProUGUI>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        if (paused) { return; }

        rotation.x += Input.GetAxis(xAxis) * sensitivity;
        rotation.y += Input.GetAxis(yAxis) * sensitivity;
        rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        /*transform.localRotation = xQuat * yQuat;*/ //Quaternions seem to rotate more consistently than EulerAngles. Sensitivity seemed to change slightly at certain degrees using Euler. transform.localEulerAngles = new Vector3(-rotation.y, rotation.x, 0);
        transform.localRotation = yQuat;
        rootObj.transform.localRotation = xQuat;

        //MoveImages();
    }

    private void MoveImages()
    {
        //image1.transform.position = new Vector3(pos1.x + newRot, pos1.y, pos1.z);

        pos1.x += Input.GetAxis(xAxis) * moveSpeed;
        pos2.x += Input.GetAxis(xAxis) * moveSpeed;
        pos3.x += Input.GetAxis(xAxis) * moveSpeed;
        image1.transform.position = new Vector3(pos1.x, pos1.y, pos1.z);
        image2.transform.position = new Vector3(pos2.x, pos2.y, pos2.z);
        image3.transform.position = new Vector3(pos3.x, pos3.y, pos3.z);
    }

    void Update()
    {
        //currentSens = sensslider.value;

        //if (currentSens != previousSens)
        //{
        //    previousSens = currentSens;
        //    sensitivity = sensslider.value;
        //    sensslider.value = Mathf.Round(sensslider.value * 100f) / 100f;
        //    sensValue.text = "Sensitivity: " + sensslider.value.ToString();
        //    weaponSocket.SetFOVnSens();
        //}

        //rotation.x += Input.GetAxis(xAxis) * sensitivity;
        //rotation.y += Input.GetAxis(yAxis) * sensitivity;
        //rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
        //var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
        //var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

        ///*transform.localRotation = xQuat * yQuat;*/ //Quaternions seem to rotate more consistently than EulerAngles. Sensitivity seemed to change slightly at certain degrees using Euler. transform.localEulerAngles = new Vector3(-rotation.y, rotation.x, 0);
        //transform.localRotation = yQuat;
        //rootObj.transform.localRotation = xQuat;
    }
}