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

    public ParticleSystem statusEffectParticles;
    [HideInInspector] public float effectDuration = 5f;
    [HideInInspector] public float baseFrequency = 1f;

    private Coroutine tickingCoroutine;

    private class StackInstance
    {
        public float expiryTime;
        public float damage;

        public StackInstance(float duration, float damage)
        {
            this.expiryTime = Time.time + duration;
            this.damage = damage;
        }
    }

    private List<StackInstance> activeStacks = new List<StackInstance>();

    public void Start()
    {
        enemyBase = GetComponent<EnemyBase>();
        damageInterface = GetComponent<IDamageInterface>();
        enemyMovement = GetComponent<Enemy_Movement>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerStats = player.GetComponent<PlayerStats>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        mainSystem = gameManager.GetComponent<AMainSystem>();
    }

    public virtual void StartEffect(int statusEffectChance, float damage = 1f)
    {
        if (!ChanceToApply(statusEffectChance)) return;
            

        activeStacks.Add(new StackInstance(effectDuration, damage));

        if (tickingCoroutine == null)
        {
            statusEffectParticles.Play();
            tickingCoroutine = StartCoroutine(EffectTickLoop());
        }
    }

    private IEnumerator EffectTickLoop()
    {
        float nextTickTime = Time.time;

        while (activeStacks.Count > 0)
        {
            float now = Time.time;

            activeStacks.RemoveAll(stack => stack.expiryTime <= now);

            if (activeStacks.Count == 0)
                break;

            float effectiveFrequency = Mathf.Max(baseFrequency / activeStacks.Count, 0.05f);

            if (now >= nextTickTime)
            {
                float totalDamage = 0f;
                foreach (var stack in activeStacks)
                {
                    totalDamage += stack.damage;
                }

                print(totalDamage);
                DoEffect(totalDamage);
                nextTickTime = now + effectiveFrequency;
            }

            yield return null;
        }

        EndEffect();
    }

    public virtual void DoEffect(float damage)
    {
        if (damageInterface != null)
        {
            damageInterface.Damage(damage, false); // You can implement crit logic elsewhere
        }

        Debug.Log($"Tick: {activeStacks.Count} stacks, total damage: {damage} at {Time.time}");
    }

    public virtual void EndEffect()
    {
        if (statusEffectParticles.isPlaying)
            statusEffectParticles.Stop();

        activeStacks.Clear();
        tickingCoroutine = null;
    }

    public virtual bool ChanceToApply(int statusEffectChance)
    {
        return Random.Range(0, 100) < statusEffectChance;
    }
}
