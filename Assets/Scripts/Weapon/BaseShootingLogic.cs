using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseShootingLogic : MonoBehaviour
{
    public GameObject shootPoint;

    [Header("Accuracy")]
    public float ADSAccuracy = 0;
    public float HIPAccuracy = 0;

    public float damage = 1;

    [HideInInspector] public GameObject player;
    [HideInInspector] public PlayerStats playerStats;

    [HideInInspector] public BaseWeapon baseWeapon;
    [HideInInspector] public Recoil recoil;
    [HideInInspector] public WeaponSocket weaponSocket;

    [HideInInspector] public ScreenShake screenShake;
    [HideInInspector] public Camera aimCamera;

    [HideInInspector] public GameObject gameManager;
    [HideInInspector] public AMainSystem aMainSystem;
    [HideInInspector] public ExplosionSystem explosionSystem;

    public enum FiringMode { fullAuto, semiAuto, burst };

    public FiringMode firingMode;


    [Header("Burst")]
    public float burstDelay = 1;
    public float burstAmount = 1;

    public virtual void TriggerItem()
    {

    }
}
