using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectBase : MonoBehaviour
{
    [HideInInspector] public EnemyBase enemyBase;
    [HideInInspector] public Enemy_Movement enemyMovement;
    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public GameObject gameManager;
    [HideInInspector] public AMainSystem mainSystem;
    [HideInInspector] public IDamageInterface damageInterface;
    [HideInInspector] public IStatusEffect statusEffectInterface;
    [HideInInspector] public Animator animator;


    public ParticleSystem statusEffectParticles;
    [HideInInspector] private float stackDuration = 5f;
    [HideInInspector] public float baseFrequency = 1f;

    private Coroutine tickingCoroutine;

    //[HideInInspector]
    [System.Serializable]
    public class StackInstance
    {
        public float expiryTime;
        public float damage;

        public StackInstance(float duration, float damage)
        {
            this.expiryTime = Time.time + duration;
            this.damage = damage;
        }
    }

    public List<StackInstance> activeStacks = new List<StackInstance>();
    //[HideInInspector] public List<StackInstance> activeStacks = new List<StackInstance>();

    public void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        damageInterface = GetComponent<IDamageInterface>();
        statusEffectInterface = GetComponent<IStatusEffect>();
        enemyMovement = GetComponent<Enemy_Movement>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        mainSystem = gameManager.GetComponent<AMainSystem>();
    }

    public virtual void StartEffect(int statusEffectChance, float damage = 1f)
    {
        if (statusEffectChance <= 0 || ReturnCondition())
            return;

        int stackAmount = ChanceToApply(statusEffectChance);
        if (stackAmount == 0) return;
        

        for (int i = 0; i < stackAmount; i++)
        {
            activeStacks.Add(new StackInstance(stackDuration, damage));
        }

        SetParticleStrength();

        if (ImmediateEffectCondition())
            ImmediateEffect();

        if (tickingCoroutine == null)
        {
            statusEffectParticles.Play();
            tickingCoroutine = StartCoroutine(EffectTickLoop());
        }
    }

    public virtual IEnumerator EffectTickLoop()
    {
        //Ticking effects
        yield return null;
    }

    public virtual void DoEffect(float damage)
    {
        // Do effect here
    }

    public virtual void ImmediateEffect()
    {
        // Do immediate effect here
    }

    public virtual void EndEffect()
    {
        // Do immediate effect here
    }

    public virtual void KillStatusEffect()
    {

        EndEffect();

        if (statusEffectParticles != null)
        {
            statusEffectParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            statusEffectParticles.Clear(true);
        }

        activeStacks.Clear();
        tickingCoroutine = null;
    }


    public virtual int ChanceToApply(int statusEffectChance)
    {
        // 1) Guaranteed stacks
        int stacks = statusEffectChance / 100;

        // 2) Leftover percentage chance
        int remainder = statusEffectChance % 100;

        // 3) Roll for one extra stack using the remainder
        if (Random.Range(0, 100) < remainder)
            stacks++;

        return stacks;
    }

    public virtual bool ImmediateEffectCondition()
    {
        // Check condition here
        return false;
    }

    public void SetParticleStrength()
    {
        int stacks = activeStacks.Count;

        // Clamp stacks between 1 and 10
        stacks = Mathf.Clamp(stacks, 1, 10);

        // Map stacks (1–10) to alpha (0.1–1.0)
        float alpha = stacks / 10f;

        var colorOverLifetime = statusEffectParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;

        // Get the existing gradient
        Gradient gradient = colorOverLifetime.color.gradient;

        // Preserve existing color keys
        GradientColorKey[] colorKeys = gradient.colorKeys;

        // Replace alpha keys while keeping their time positions
        GradientAlphaKey[] alphaKeys = gradient.alphaKeys;

        for (int i = 0; i < alphaKeys.Length; i++)
        {
            alphaKeys[i].alpha = alpha;
        }

        // Apply modified gradient
        gradient.SetKeys(colorKeys, alphaKeys);
        colorOverLifetime.color = gradient;
    }

    public virtual bool ReturnCondition()
    {
        // Check condition here
        return false;
    }

}
