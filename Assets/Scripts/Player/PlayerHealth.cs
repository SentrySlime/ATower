using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using static System.Net.Mime.MediaTypeNames;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;
using System;

public class PlayerHealth : MonoBehaviour, IDamageInterface
{
    public bool isInvincible;

    PlayerStats playerStats;
    HealthRegen healthRegen;
    Inventory inventory;
    GameObject canvas;
    PauseMenu pauseMenu;
    GameOverScreen gameOverScreen;


    [Header("Health stuff")]
    public int maxHP;
    public int currentHP;
    
    float iFrameDuration = 0.2f;
    private Vector3 startPos;
    
    public Slider barHP;
    public RectTransform hpFill;
    public RectTransform hpBackground;
    public TextMeshProUGUI textHP;

    public bool dead = false;

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

    [Header("Object References")]
    public GameObject playerTargetPoint;

    [HideInInspector] public float damageReductionPercent = 1;
    [HideInInspector] public int damageIgnoreChance = 0;

    float iFrameTimer = 0;
    bool canBeDamaged = true;

    private void Awake()
    {
        startPos = transform.position;
        playerStats = GetComponent<PlayerStats>();
        healthRegen = GetComponent<HealthRegen>();
        inventory = GetComponent<Inventory>();
        canvas = GameObject.Find("Canvas");
        pauseMenu = canvas.GetComponent<PauseMenu>();
        gameOverScreen = canvas.GetComponent<GameOverScreen>();
        pauseMenu.GetVignettes(this);
        barHP = GameObject.FindGameObjectWithTag("HPBar").GetComponent<Slider>();

        hpFill = GameObject.FindGameObjectWithTag("HPFill").GetComponent<RectTransform>();
        hpBackground = GameObject.FindGameObjectWithTag("HPBackground").GetComponent<RectTransform>();
        textHP = GameObject.FindGameObjectWithTag("textHP").GetComponent<TextMeshProUGUI>();

        maxHP = playerStats.maxHealth;

        UpdateMaxHP();
        StartHealth();
        UpdateHP();
    }

    void Start()
    {
        
    }


    void Update()
    {
     
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

        if(Input.GetKeyDown(KeyCode.M))
        {
            
            Damage(40);
        }

    }

    #region HP // ---------------

    public void HealOnKill()
    {
        Heal(playerStats.hpOnKill, false);
    }

    public void Heal(float incomingHealth, bool elite)
    {
        if (playerStats.onlyEliteKillHeal > 0 && !elite)
            return;

        int preHp = currentHP;
        int healthToAdd = (int)incomingHealth;

        if (playerStats.canOverheal > 0)
        {
            currentHP += healthToAdd;
        }
        else if (playerStats.healCap > 0)
        {
            int healCapValue = CalculatePercentage(playerStats.healCap);

            if (preHp < healCapValue)
            {
                currentHP += healthToAdd;
                currentHP = Mathf.Clamp(currentHP, 0, healCapValue);
            }
        }
        else
        {
            currentHP += healthToAdd;
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);
        }

        UpdateHP();
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
        if (isInvincible && dead) { return; }

        if (!canBeDamaged)
        {
            shieldvignetteAlpha = 1;
            shieldVignetteTimer = 0;
            return;
        }

        if (DamageChance())
        {
            return;
        }
        else
        {
            vignetteAlpha = 1;
            vignetteTimer = 0;
            damage = damage / damageReductionPercent;
        }

        if(playerStats.moneyIsHealth > 0)
        {
            int damageCost = (int)damage * 4;
            int moneyAvailable = inventory.money;

            int moneyToDeduct = Math.Min(moneyAvailable, damageCost);
            int remainingDamage = damageCost - moneyToDeduct;

            inventory.DecreaseMoney(moneyToDeduct);

            if (remainingDamage > 0)
            {
                currentHP -= remainingDamage;
                UpdateHP();
            }
        }
        else
        {
            currentHP -= ((int)damage);
            UpdateHP();
        }

        currentHP = Mathf.Clamp(currentHP, 0, 9999);

        canBeDamaged = false;
        iFrameTimer = 0;

        if (currentHP <= 0)
            PlayerDeath();
    }

    public void RemoveHealth(float hpToRemove)
    {
        vignetteAlpha = 1;
        vignetteTimer = 0;
        currentHP -= ((int)hpToRemove);
        UpdateHP();
    }

    public int CalculatePercentage(int value)
    {
        if (value < 1 || value > 100)
        {
            return 0;
        }

        float percentage = (value / 100f) * maxHP;
        int roundedPercentage = Mathf.CeilToInt(percentage);
        return roundedPercentage;
    }

    public void PlayerDeath()
    {
        dead = true;
        gameOverScreen.GameOver();
        //Scene scene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(scene.name);
        //Insert death logic
    }

    private bool DamageChance()
    {
        int ignoreChance = UnityEngine.Random.Range(0, 100);

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
        hpFill.sizeDelta = new Vector2(currentHP / 2, hpFill.sizeDelta.y);
        textHP.text = currentHP.ToString() + " / " + maxHP;
    }

    public void UpdateMaxHP()
    {
        barHP.maxValue = maxHP;
        hpBackground.sizeDelta = new Vector2(maxHP /  2, hpBackground.sizeDelta.y);
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
        if (healthRegen != null)
            healthRegen.UpdateHPRegen();
        UpdateHP();
        UpdateMaxHP();

    }

    public void ItemUpdateHealth()
    {
        if(healthRegen != null)
            healthRegen.UpdateHPRegen();
        UpdateHP();
        UpdateMaxHP();
    }

    public void Damage(float damage, bool criticalHit)
    {
        throw new System.NotImplementedException();
    }

}

