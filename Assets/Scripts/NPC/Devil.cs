using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Devil : MonoBehaviour
{
    public GameObject destroyVFX;
    public GameObject destroySFX;

    public AudioSource pentagramSFX;

    public GameObject devilItemVFX;

    [Header("Left - Right")]
    public GameObject rightHand;
    public GameObject leftHand;

    public Transform rightHandSpawnPoint;
    public Transform leftHandSpawnPoint;

    Animator rightHandAnimator;
    Animator leftHandAnimator;

    ItemPickUp rightItem;
    ItemPickUp leftItem;

    public Material material;

    LootSystem lootSystem;
    bool triggered = false;
    public float fadeDuration = 0.4f;

    void Start()
    {
        leftHandAnimator = leftHand.GetComponent<Animator>();
        rightHandAnimator = rightHand.GetComponent<Animator>();
        lootSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<LootSystem>();
        material.SetColor("_EmissionColor", Color.black);
    }

    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) { return; }

        if(other.CompareTag("Player"))
        {
            pentagramSFX.Play();
            StartCoroutine(FadeEmission());
            triggered = true;
            OpenHands();
            Invoke("SpawnItems", 1.35f);
        }
    }

    private void OpenHands()
    {
        leftHandAnimator.SetBool("OpenHand", true);
        rightHandAnimator.SetBool("OpenHand", true);
    }


    private void SpawnItems()
    {
        Instantiate(devilItemVFX, rightHandSpawnPoint.position, Quaternion.identity);
        rightItem = lootSystem.DropDevilItem(rightHandSpawnPoint.position);

        Instantiate(devilItemVFX, leftHandSpawnPoint.position, Quaternion.identity);
        leftItem = lootSystem.DropDevilItem(leftHandSpawnPoint.position);
    }

    public void DestroyOtherItem(ItemPickUp item)
    {
        if(item == leftItem)
        {
            StartCoroutine(CloseHand(true));
            Destroy(rightItem.gameObject);
        }

        if (item == rightItem)
        {
            StartCoroutine(CloseHand(false));
            Destroy(leftItem.gameObject);
        }
    }

    IEnumerator CloseHand(bool rightHand)
    {
        if (rightHand)
        {
            rightHandAnimator.SetBool("CloseHand", true);
            yield return new WaitForSeconds(0.25f);
            Instantiate(destroySFX, rightHandSpawnPoint.position, Quaternion.identity);
            Instantiate(destroyVFX, rightHandSpawnPoint.position, Quaternion.identity);
        }
        else
        {
            leftHandAnimator.SetBool("CloseHand", true);
            yield return new WaitForSeconds(0.25f);
            Instantiate(destroySFX, rightHandSpawnPoint.position, Quaternion.identity);
            Instantiate(destroyVFX, leftHandSpawnPoint.position, Quaternion.identity);
        }
    }

    private IEnumerator FadeEmission()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float t = elapsedTime / fadeDuration;
            Color currentColor = Color.red * Mathf.Lerp(0f, 1f, t);
            material.SetColor("_EmissionColor", currentColor);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it's fully set at the end
        material.SetColor("_EmissionColor", Color.red);
    }

}