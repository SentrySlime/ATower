using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FindAndEquipWeapons : MonoBehaviour
{
    public Camera Mcamera;
    [SerializeField] public BaseWeapon baseWeapon;
    [SerializeField] WeaponSocket weaponSocket;
    [SerializeField] GameObject weaponParent;
    [SerializeField] float interactDistance;

    GameObject lastHitObj;

    public GameObject startWeapon;

    

    float dissapearTimer;

    Inventory inventory;

    [HideInInspector] private Transform shootPoint;

    [Header("Panel")]
    public GameObject popUpPanel;
    public CanvasGroup popUpItemGroup;
    public TextMeshProUGUI popUpItemName;
    public TextMeshProUGUI popUpItemDescription;
    public Image image2;

    [Header("Item panel")]
    public GameObject itemPanel;
    [HideInInspector] public CanvasGroup itemGroup;
    [HideInInspector] public TextMeshProUGUI itemName;
    [HideInInspector] public TextMeshProUGUI itemDescription;
    [HideInInspector] public Image image;

    [Header("SFX")]
    public AudioSource equipSFX;

    private void Awake()
    {
        baseWeapon = GetComponent<BaseWeapon>();
        Mcamera = Camera.main;
        inventory = GetComponent<Inventory>();
        shootPoint = GameObject.FindGameObjectWithTag("ShootPoint").transform;
    }

    void Start()
    {
        if (startWeapon != null)
            StartUpWeapon();
    }

    void Update()
    {
        CheckForItems();
        
        if (Input.GetKeyDown(KeyCode.E))
        {

            RaycastHit hit;
            if (Physics.Raycast(Mcamera.transform.position, Mcamera.transform.forward * interactDistance, out hit, interactDistance) && hit.transform.gameObject.CompareTag("Item"))
            {

                if (!equipSFX.isPlaying)
                    equipSFX.Play();

                WeaponPickUp test = hit.collider.gameObject.GetComponent<WeaponPickUp>();
                if (test != null)
                {
                    GameObject tempObj = hit.collider.gameObject.GetComponent<WeaponPickUp>().returnWeapon();

                    Destroy(hit.collider.gameObject);
                    inventory.heldWeapons.Add(tempObj);

                    if (baseWeapon != null)
                    {
                        tempObj.GetComponent<Recoil>().DisableWeapon();
                        //tempObj.SetActive();
                        //hit.transform.gameObject.SetActive(false);

                    }
                    else
                    {
                        SetWeapon(tempObj);
                        //SetWeapon(hit.transform.gameObject);
                    }

                    //inventory.heldWeapons.Add(hit.transform.gameObject);

                    //hit.transform.GetComponent<Collider>().enabled = false;

                    //if (baseWeapon != null)
                    //{
                    //    hit.transform.gameObject.SetActive(false);

                    //}
                    //else
                    //{
                    //    SetWeapon(hit.transform.gameObject);
                    //}
                }
                else
                {
                    ItemPickUp test2 = hit.collider.gameObject.GetComponent<ItemPickUp>();

                    if (!test2) return;

                    Vector3 SpawnPos = new Vector3(-9999, -9999, -9999);
                    ItemBase tempObj = Instantiate(hit.collider.gameObject.GetComponent<ItemPickUp>().itemPrefab, SpawnPos, Quaternion.identity);
                    tempObj.EquipItem();
                    Destroy(hit.collider.gameObject);
                    //tempObj.GetComponent<ItemBase>().EquipItem();

                    GameObject itemObj = Instantiate(itemPanel, itemGroup.transform);
                    ItemPanel tempPanel = itemObj.GetComponent<ItemPanel>();
                    tempPanel.SetPanel(test2);
                    tempPanel.itemName = itemName;
                    tempPanel.itemDescription = itemDescription;
                    tempPanel.image = image;
                }

            }

        }

    }

    private void StartUpWeapon()
    {
        inventory.heldWeapons.Add(startWeapon);
        //startWeapon.transform.GetComponent<Collider>().enabled = false;
        SetWeapon(startWeapon);
    }

    public void SetWeapon(GameObject hitObj)
    {


        baseWeapon = hitObj.GetComponentInChildren<BaseWeapon>();

        inventory.baseWeapon = baseWeapon;
        inventory.recoil = baseWeapon.recoil;

        baseWeapon.transform.parent.SetParent(weaponParent.transform, true);
        baseWeapon.transform.parent.transform.rotation = new Quaternion(0, 0, 0, 0);
        baseWeapon.transform.parent.transform.localPosition = new Vector3(0, 0, 0);
        lastHitObj = hitObj;
        if (lastHitObj != null)
            weaponSocket.SetUpWeapon(lastHitObj);
    }

    private void CheckForItems()
    {


        if (dissapearTimer < .25f)
        {
            dissapearTimer += Time.deltaTime;
        }
        else if(popUpPanel)
        {
            popUpPanel.SetActive(false);
        }

        RaycastHit hit;
        if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out hit, interactDistance))
        {
            if (hit.transform.CompareTag("Item"))
            {
                dissapearTimer = 0;
                var tempItem = hit.transform.GetComponent<ItemPickUp>();
                if (tempItem)
                {
                    if (!popUpPanel.activeInHierarchy)
                        popUpPanel.SetActive(true);

                    popUpItemName.text = tempItem.itemName;
                    popUpItemDescription.text = tempItem.itemDescription;
                    image2.sprite = tempItem.itemIcon;

                }
            }
            else
            {

            }

        }
    }

}
