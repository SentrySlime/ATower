using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageInterface
{
    public bool isInvincible;

    PauseMenu pauseMenu;

    [Header("Health stuff")]
    public int maxHP;
    public int currentHP;
    public Slider barHP;
    private Vector3 startPos;
    float iFrameDuration = 0.2f;


    [Header("DamageVignetteStuff")]
    public CanvasGroup damageVignette;
    public float vignetteAlpha = 0;
    float vignetteTimer = 0;
    public float vignetteRate = 0;
    public float vignetteThreshold = 0;

    [Header("ShieldVignetteStuff")]
    public CanvasGroup shieldVignette;
    public float shieldvignetteAlpha = 0;
    float shieldVignetteTimer = 0;
    public float shieldVignetteRate = 0;


    [HideInInspector] public float damageReductionPercent = 1;
    [HideInInspector] public int damageIgnoreChance = 0;

    float iFrameTimer = 0;
    bool canBeDamaged = true;

    void Start()
    {
        startPos = transform.position;
        pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenu>();
        pauseMenu.GetVignettes(this);
        barHP = GameObject.FindGameObjectWithTag("HPBar").GetComponent<Slider>();
        currentHP = maxHP;
        barHP.maxValue = maxHP;

    }


    void Update()
    {



        barHP.value = currentHP;

        if (iFrameTimer < iFrameDuration)
        {
            iFrameTimer += Time.deltaTime;
        }
        else if (canBeDamaged == false)
        {
            canBeDamaged = true;
        }

        if (shieldvignetteAlpha > 0)
            ShieldVignette();

        if (currentHP <= 30)
            vignetteThreshold = 0.3f;
        else
            vignetteThreshold = 0;

        if (vignetteAlpha > vignetteThreshold)
            DamageVignette();

    }

    public void Heal(float incomingHealth)
    {
        currentHP += ((int)incomingHealth);
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
    }

    public void Damage(float damage)
    {
        if (isInvincible) { return; }

        if (!canBeDamaged)
        {
            shieldvignetteAlpha = 1;
            shieldVignetteTimer = 0;
            return;
        }




        if (DamageChance())
        {
            //Do something here
        }
        else
        {
            vignetteAlpha = 1;
            vignetteTimer = 0;
            damage = damage / damageReductionPercent;
            currentHP -= ((int)damage);
        }

        canBeDamaged = false;
        iFrameTimer = 0;



        if (currentHP <= 0)
            PlayerDeath();
    }

    public void PlayerDeath()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
        //print("You dead");
        //Insert death logic

    }

    private bool DamageChance()
    {
        int ignoreChance = Random.Range(0, 100);

        if (damageIgnoreChance > ignoreChance)
        {
            return true;

        }
        else
            return false;

    }

    private void DamageVignette()
    {
        damageVignette.alpha = 1;
        //vignetteAlpha = 1;
        if (vignetteTimer < vignetteRate)
        {
            vignetteTimer += Time.deltaTime * 2;
            vignetteAlpha -= Time.deltaTime * 2;
            damageVignette.alpha = vignetteAlpha;
        }
    }

    private void ShieldVignette()
    {
        shieldVignette.alpha = 1;
        //vignetteAlpha = 1;
        if (shieldVignetteTimer < shieldVignetteRate)
        {
            shieldVignetteTimer += Time.deltaTime * 3;
            shieldvignetteAlpha -= Time.deltaTime * 3;
            shieldVignette.alpha = shieldvignetteAlpha;
        }
    }

}

