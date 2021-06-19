using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class UnitManager : MonoBehaviour
{
    [HideInInspector] public SquadInfluenceManager squadInfluenceManager;
    [HideInInspector] public AIPath aiPath;
    [HideInInspector] public HarveyManager harveyManager;

    [Header("Stats")]
    public int _MaxHealth = 100;
    private int _Health;
    public float _AttackRate = 3f;
    public int _WalkSpeed = 1;
    public int _Damage = 10;
    

    private float _TimerTicker = 0f;

    [HideInInspector] public Transform _Target;
    [HideInInspector] public Vector3 _WalkTarget;

    [Header("Weapon")] public Weapon _Weapon;

    public void getDamage(int amount)
    {
        _Health -= amount;

        if (_Health <= 0)
        {
            GameController.Instance.StartRespawn(squadInfluenceManager);
            
            if (squadInfluenceManager.style == SquadInfluenceManager.InfluenceStyle.SHARED)
            {
                IterationManager.Instance.currentMatchData.SharedMatchDeath.DeathPositions.Add(transform.position);
                IterationManager.Instance.currentMatchData.UnsharedMatchDeath.KillPositions.Add(transform.position);
            } else
            {
                IterationManager.Instance.currentMatchData.UnsharedMatchDeath.DeathPositions.Add(transform.position);
                IterationManager.Instance.currentMatchData.SharedMatchDeath.KillPositions.Add(transform.position);
            }
            
            Destroy(gameObject);
        }

        // TODO Callback for Manager
    }

    private void Awake()
    {
        _Health = _MaxHealth;
        _Weapon = GetComponentInChildren<Weapon>();
        squadInfluenceManager = GetComponentInParent<SquadInfluenceManager>();
        harveyManager = GetComponent<HarveyManager>();
        aiPath = GetComponent<AIPath>();
    }

    public bool checkTimer(float time)
    {
        if (_TimerTicker <= 0)
        {
            _TimerTicker = time;
            return true;
        }
        else
            _TimerTicker -= Time.deltaTime;

        return false;
    }


    private void Update()
    {
        RotateWeaponTowardsTarget();
    }

    private void RotateWeaponTowardsTarget()
    {
        if (_Target != null)
        {
            Vector3 relativePos = _Target.position - _Weapon.gameObject.transform.position;
            relativePos.Normalize();

            float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
            _Weapon.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, angle - 90);
        }
    }

}
