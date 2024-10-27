using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using FischlWorks_FogWar;
using UnityEngine;
using Type = Elements.Type;

public enum EnemyControllerState
{
    Idle,Walk,Attack,Stunned
}

public abstract class EnemyController : MonoBehaviour, IDamageable, ICharacter
{
    [SerializeField] protected EnemyPathingController agent;
    [SerializeField] protected EnemyAnimator animator;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] protected EnemyData enemyData;
    [SerializeField] private csFogVisibilityAgent visibilityAgent;
    [SerializeField] private new Rigidbody rigidbody;
    [SerializeField] private bool stunnable = true;
    [SerializeField] private bool spawnHealthBar = true;
    [SerializeField] private Vector3 statBarOffset = new Vector3(0,20,0);

    public CharacterStats Stats
    {
        get;
        private set;
    }
    
    protected FSM StateMachine;
    protected List<Attack> Attacks;
    private Transform _target = null;
    private int _stunCounter = 0;
    protected bool Stunned;
    private bool _dead;

    public bool PlayerVisible
    {
        get;
        private set;
    }
    public Transform Transform => transform;
    public EnemyData EnemyData => enemyData;
    public List<Transform> Waypoints => waypoints;
    public bool Visible => visibilityAgent == null || visibilityAgent.Visible;
    public CharacterStats GetStats() => Stats;

    public Transform Target => _target;
    
    public Action OnDeath;
    public Action<int> OnDamage;
    
    #region Unity Event Functions
    protected void Start()
    {
        Attacks = new List<Attack>();
        Stats = new CharacterStats(enemyData);
        foreach (AttackData attackData in enemyData.Attacks)
        {
            var attack = attackData.CreateInstance(Stats, transform);
            Attacks.Add(attack);
        }

        StateMachine = new FSM(); 
        if(spawnHealthBar)
            EnemyHealthBars.Instance.SpawnHealthBar(transform, this, statBarOffset);
        agent.Speed = Stats.Speed;
    }

    protected void FixedUpdate()
    {
        if (GlobalReferences.Instance.Player != null)
        {
            var diff = transform.position - GlobalReferences.Instance.Player.transform.position;
            if (diff.magnitude < enemyData.AggroRange)
                PlayerVisible = IsPlayerVisible();
        }

        agent.Speed = Stats.Speed;
        StateMachine.Tick();
    }

    private void LateUpdate()
    {
        StateMachine.LateTick();
    }
    #endregion

    public void TakeDamage(float damage, float attackStat, Vector3 knockback, float hitStun, Type attackType)
    {
        float typeMultiplier = GlobalReferences.Instance.TypeManager.GetTypeMultiplier(Stats.ElementType, attackType);
        float statMultiplier = Stats.Defense > 0 ? attackStat / Stats.Defense : 1f;
        int roundedDamage = Mathf.CeilToInt(damage * typeMultiplier * statMultiplier);
        Stats.ApplyDamage(roundedDamage);
        OnDamage?.Invoke(roundedDamage);
        if (Stats.Health <= 0)
        {
            HandleDeath();
            return;
        }

        StartCoroutine(HandleKnockback(knockback, hitStun));
    }

    private IEnumerator HandleKnockback(Vector3 knockback, float hitStun)
    {
        if (hitStun > 0 && stunnable)
        {
            ApplyStun(knockback, hitStun);
            yield return new WaitForSeconds(hitStun);
            RemoveStun();
        }
    }

    private void HandleDeath()
    {
        if (_dead)
            return;
        _dead = true;
        if(ResourceManager.Instance != null)
            ResourceManager.Instance.SpawnCoins(enemyData.CoinsOnDeath, transform.position);
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public abstract bool Attack();

    private bool IsPlayerVisible()
    {
        if (GlobalReferences.Instance.Player == null)
            return false;
        Collider col = GlobalReferences.Instance.Player.GetComponentInChildren<Collider>();
        if (col == null)
            return false;
        bool hit = Physics.Raycast(transform.position,
            col.bounds.center - transform.position, 
            out RaycastHit hitInfo,
            enemyData.AggroRange+1, 
            LayerMask.GetMask("Player", "Walls", "Obstacles"));
        return hit && hitInfo.transform.CompareTag("Player");
    }

    protected bool PlayerOutOfRange()
    {
        if (GlobalReferences.Instance.Player != null)
        {
            var diff = transform.position - GlobalReferences.Instance.Player.transform.position;
            if (diff.magnitude >= enemyData.DeAggroRange)
            {
                _target = null;
                return true;
            }
        }

        return false;
    }

    protected bool PlayerInRange()
    {
        if (GlobalReferences.Instance.Player != null)
        {
            var diff = transform.position - GlobalReferences.Instance.Player.transform.position;
            if (diff.magnitude < enemyData.AggroRange && PlayerVisible)
            {
                _target = GlobalReferences.Instance.Player.transform;
                return true;
            }
        }

        return false;
    }

    private void ApplyStun(Vector3 knockback, float hitStun)
    {
        if (hitStun >= 1)
        {
            rigidbody.velocity = Vector3.zero;
            Stunned = true;
        }

        if (knockback != Vector3.zero)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(knockback, ForceMode.Impulse);
        }

        _stunCounter++;
    }

    private void RemoveStun()
    {
        _stunCounter--;
        if (_stunCounter == 0)
        {
            rigidbody.velocity = Vector3.zero;
            Stunned = false;
        }
    }

    public void SetWaypoints(List<Transform> waypoints)
    {
        this.waypoints = waypoints;
    }
}