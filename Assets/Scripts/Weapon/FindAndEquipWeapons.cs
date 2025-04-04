using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FindAndEquipWeapons : MonoBehaviour
{
    [Header("WeaponIcons")]
    public GameObject weaponIcon;
    public GameObject iconParent;
    public List<WeaponIcon> weaponIcons;
    public WeaponIcon previousIcon;
    public WeaponIcon currentIcon;

    [Header("Camera & Stuff")]
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
    public Image popUpItemImage;

    [Header("Item panel")]
    public GameObject itemPanel;
    [HideInInspector] public CanvasGroup itemGroup;
    [HideInInspector] public TextMeshProUGUI itemName;
    [HideInInspector] public TextMeshProUGUI itemDescription;
    [HideInInspector] public Image image;
    [HideInInspector] public SelectedItem selectedItem;

    [Header("SFX")]
    public AudioSource equipSFX;

    PlayerStats playerStats;

    private void Awake()
    {
        baseWeapon = GetComponent<BaseWeapon>();
        Mcamera = Camera.main;
        inventory = GetComponent<Inventory>();
        shootPoint = GameObject.FindGameObjectWithTag("ShootPoint").transform;
        iconParent = GameObject.FindGameObjectWithTag("iconParent");

        //---
        popUpPanel = GameObject.FindGameObjectWithTag("PopUpItemPanel");
        popUpItemDescription = popUpPanel.transform.Find("PopUpDescription").GetComponent<TextMeshProUGUI>();
        popUpItemName = popUpPanel.transform.Find("PopUpName").GetComponent<TextMeshProUGUI>();
        popUpItemImage = popUpPanel.transform.Find("PopUpItemSprite").GetComponent<Image>();
        
        playerStats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        selectedItem = itemGroup.GetComponent<SelectedItem>();
        if (startWeapon)
            InitializeWeapon(startWeapon);
    }

    void Update()
    {
        CheckForItems();

        if (Input.GetKeyDown(KeyCode.E))
        {

            RaycastHit hit;
            if (Physics.Raycast(Mcamera.transform.position, Mcamera.transform.forward * interactDistance, out hit, interactDistance))
            {
                if (hit.transform.gameObject.CompareTag("Item"))
                    InitializeWeapon(hit.collider.gameObject);
                else if (hit.transform.gameObject.CompareTag("Interact"))
                    hit.transform.GetComponent<IInteractInterface>().Interact();
            }
        }
    }

    private void InitializeWeapon(GameObject IncomingWeaponObj)
    {
        if (!equipSFX.isPlaying)
            equipSFX.Play();

        WeaponPickUp weaponPickUp = IncomingWeaponObj.GetComponent<WeaponPickUp>();
        if (weaponPickUp != null)
        {
            GameObject weaponObj = IncomingWeaponObj.GetComponent<WeaponPickUp>().returnWeapon();
            SetNewWeaponIcon(weaponObj);

            inventory.weaponIndex = inventory.heldWeapons.Count;

            Destroy(IncomingWeaponObj);
            inventory.heldWeapons.Add(weaponObj);
            playerStats.AddStatsToPickedUpWeapon(weaponObj);

            if (baseWeapon != null)
            {
                weaponObj.GetComponent<Recoil>().DisableWeapon();
            }
            else
            {
                SetWeapon(weaponObj);
            }

        }
        else
        {
            ItemPickUp test2 = IncomingWeaponObj.GetComponent<ItemPickUp>();

            if (!test2) return;

            Vector3 SpawnPos = new Vector3(-9999, -9999, -9999);
            ItemBase tempObj = Instantiate(IncomingWeaponObj.GetComponent<ItemPickUp>().itemPrefab, SpawnPos, Quaternion.identity);
            inventory.heldItems.Add(tempObj.gameObject);

            tempObj.EquipItem();
            
            Destroy(IncomingWeaponObj);
            
            GameObject itemObj = Instantiate(itemPanel, itemGroup.transform);
            ItemPanel tempPanel = itemObj.GetComponent<ItemPanel>();
            tempPanel.SetPanel(test2);
            tempPanel.itemName = itemName;
            tempPanel.itemDescription = itemDescription;
            tempPanel.image = image;
        }
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
        {
            weaponSocket.SetUpWeapon(lastHitObj);
            SetActiveWeaponIcon();
        }
    }

    private void CheckForItems()
    {
        if (dissapearTimer < .25f)
        {
            dissapearTimer += Time.deltaTime;
        }
        else if (popUpPanel)
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
                    popUpItemImage.sprite = tempItem.itemIcon;

                }
            }
        }
    }

    private void SetNewWeaponIcon(GameObject weaponObj)
    {
        
        //Spawn the new weaponIcon
        GameObject obj = Instantiate(weaponIcon, iconParent.transform);
        WeaponIcon weaponIconObj = obj.GetComponent<WeaponIcon>();
        //Set the icon to resemble the weapon we picked up
        weaponIconObj.SetIcon(weaponObj.GetComponentInChildren<BaseWeapon>().weaponIcon);
        //Setting the hotkey number for what to press
        weaponIconObj.SetHotKeyIndex(inventory.weaponIndex = inventory.heldWeapons.Count + 1);
        //Add it to the list of weaponIcons
        weaponIcons.Add(weaponIconObj);
        weaponObj.GetComponentInChildren<BaseWeapon>().iconPrefab = obj;
        SetActiveWeaponIcon(weaponObj);
    }

    private void SetActiveWeaponIcon(GameObject obj)
    {
        foreach (WeaponIcon icon in weaponIcons)
        {
            icon.SetInactive();
        }
        obj.GetComponentInChildren<BaseWeapon>().iconPrefab.GetComponent<WeaponIcon>().Activate();
    }

    private void SetActiveWeaponIcon()
    {
        foreach (WeaponIcon icon in weaponIcons)
        {
            icon.SetInactive();
        }
        weaponSocket.equippedWeapon.iconPrefab.GetComponent<WeaponIcon>().Activate();
    }

}