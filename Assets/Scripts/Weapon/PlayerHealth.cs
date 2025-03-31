using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

public class PlayerHealth : MonoBehaviour, IDamageInterface
{
    public bool isInvincible;

    PauseMenu pauseMenu;

    [Header("Health stuff")]
    public int maxHP;
    public int currentHP;
    
    float iFrameDuration = 0.2f;
    private Vector3 startPos;
    
    public Slider barHP;
    public RectTransform hpFill;
    public RectTransform hpBackground;
    public TextMeshProUGUI textHP;

    [Header("DamageVignetteStuff")]
    public CanvasGroup damageVignette;
    public float vignetteAlpha = 0;
    float vignetteTimer = 0;
    public float vignetteRate = 0;
    public float vignetteThreshold = 0;

    [Header("ShieldVignetteStuff")]
    public PlayerStats playerStats;
    public CanvasGroup shieldVignette;
    public float shieldvignetteAlpha = 0;
    float shieldVignetteTimer = 0;
    public float shieldVignetteRate = 0;

    [Header("Object References")]
    public GameObject playerTargetPoint;

    [HideInInspector] public float damageReductionPercent = 1;
    [HideInInspector] public int damageIgnoreChance = 0;

    float iFrameTimer = 0;
    bool canBeDamaged = true;

    float hpRegenCooldown = 0;
    float hpRegenTimer = 0;

    private void Awake()
    {
        startPos = transform.position;
        playerStats = GetComponent<PlayerStats>();
        pauseMenu = GameObject.Find("Canvas").GetComponent<PauseMenu>();
        pauseMenu.GetVignettes(this);
        barHP = GameObject.FindGameObjectWithTag("HPBar").GetComponent<Slider>();

        hpFill = GameObject.FindGameObjectWithTag("HPFill").GetComponent<RectTransform>();
        hpBackground = GameObject.FindGameObjectWithTag("HPBackground").GetComponent<RectTransform>();
        textHP = GameObject.FindGameObjectWithTag("textHP").GetComponent<TextMeshProUGUI>();

        maxHP = playerStats.maxHealth;

        UpdateMaxHP();
        StartHealth();
        UpdateHP();
        HealthRegen();
        UpdateHPRegen();
    }

    void Start()
    {
        
    }


    void Update()
    {

        HealthRegen();
     
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

        if(Input.GetKeyDown(KeyCode.U))
        {
            Damage(10);
        }

    }

    #region HP // ---------------

    public void HealOnKill()
    {
        Heal(playerStats.hpOnKill);
    }

    private void HealthRegen()
    {
        if(hpRegenTimer < hpRegenCooldown)
            hpRegenTimer += Time.deltaTime;
        else
        {
           Heal(1);
           hpRegenTimer = 0;
        }
    }

    public void Heal(float incomingHealth)
    {
        currentHP += ((int)incomingHealth);
        currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        UpdateHP();
    }
    public void UpdateHPRegen()
    {
        hpRegenCooldown = 1 / playerStats.hpRegen;
    }

    public bool HasFullHP()
    {
        if(currentHP >= maxHP)
            return true;
        else
            return false;
    }

    #endregion

    #region Damage

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

        UpdateHP();

        canBeDamaged = false;
        iFrameTimer = 0;

        if (currentHP <= 0)
            PlayerDeath();
    }

    public void PlayerDeath()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
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

    #endregion

    public void UpdateHP()
    {
        barHP.value = currentHP;
        hpFill.sizeDelta = new Vector2(currentHP, hpFill.sizeDelta.y);
        textHP.text = currentHP.ToString() + " / " + maxHP;
    }

    public void UpdateMaxHP()
    {
        barHP.maxValue = maxHP;
        hpBackground.sizeDelta = new Vector2(maxHP, hpBackground.sizeDelta.y);
    }

    public void PlayerMaxHP()
    {
        currentHP = maxHP;
    }

    public GameObject GetTargetPoint()
    {
        return playerTargetPoint;
    }


    public void StartHealth()
    {
        PlayerMaxHP();
        UpdateHPRegen();
        UpdateHP();
        UpdateMaxHP();

    }

    public void ItemUpdateHealth()
    {
        UpdateHPRegen();
        UpdateHP();
        UpdateMaxHP();
    }
}

