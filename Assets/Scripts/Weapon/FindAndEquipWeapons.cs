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
    float interactDistance = 12;

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

    [HideInInspector] public GameObject interactPrompt;

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

        interactPrompt = GameObject.Find("Interact_Prompt").gameObject;

        interactPrompt.SetActive(false);

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
                if (hit.transform.gameObject.CompareTag("Interact"))
                {
                    hit.transform.GetComponent<IInteractInterface>().Interact();
                }
                else if (hit.transform.gameObject.CompareTag("PickUp"))
                {
                    if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Items"))
                    {    
                        InitializeItem(hit.collider.gameObject);
                        if (!equipSFX.isPlaying)
                            equipSFX.Play();
                    }
                    else
                    {
                        InitializeWeapon(hit.collider.gameObject);
                        if (!equipSFX.isPlaying)
                            equipSFX.Play();
                    }
                }

            }
        }
    }

    private void InitializeItem(GameObject incomingItem)
    {
        ItemPickUp test2 = incomingItem.GetComponent<ItemPickUp>();

        if (!test2) return;

        Vector3 SpawnPos = new Vector3(-9999, -9999, -9999);

        ItemBase tempObj = Instantiate(incomingItem.GetComponent<ItemPickUp>().itemPrefab, SpawnPos, Quaternion.identity);
        
        inventory.heldItems.Add(tempObj.gameObject);

        tempObj.EquipItem();

        Destroy(incomingItem);

        GameObject itemObj = Instantiate(itemPanel, itemGroup.transform);
        ItemPanel tempPanel = itemObj.GetComponent<ItemPanel>();
        tempPanel.SetPanel(test2);
        tempPanel.itemName = itemName;
        tempPanel.itemDescription = itemDescription;
        tempPanel.image = image;
    }

    private void InitializeWeapon(GameObject IncomingWeaponObj)
    {
       
        WeaponPickUp weaponPickUp = IncomingWeaponObj.GetComponent<WeaponPickUp>();
        if (weaponPickUp != null)
        {
            GameObject weaponObj = IncomingWeaponObj.GetComponent<WeaponPickUp>().ReturnWeapon();
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
        if (dissapearTimer < .15f)
        {
            dissapearTimer += Time.deltaTime;
        }
        else if (popUpPanel)
        {
            if (popUpPanel.activeInHierarchy)
                popUpPanel.SetActive(false);
            if (interactPrompt.activeInHierarchy)
                interactPrompt.SetActive(false);
        }

        RaycastHit hit;
        if (Physics.Raycast(shootPoint.transform.position, shootPoint.transform.forward, out hit, interactDistance))
        {
            if (hit.transform.CompareTag("PickUp") || hit.transform.CompareTag("Interact"))
            {
                if (!interactPrompt.activeInHierarchy)
                    interactPrompt.SetActive(true);
            }


            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Items"))
            {
                dissapearTimer = 0;
                var tempItem = hit.transform.GetComponent<Item>();
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
        obj.transform.SetSiblingIndex(0);

        WeaponIcon weaponIconObj = obj.GetComponent<WeaponIcon>();

        weaponIconObj.reloadTime = weaponObj.GetComponentInChildren<BaseWeapon>().reloadTime * 1.5f;

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