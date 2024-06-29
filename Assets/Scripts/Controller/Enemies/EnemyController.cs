using System;
using System.Collections;
using System.Collections.Generic;
using Controller;
using FischlWorks_FogWar;
using Systems.Modifiers;
using UnityEngine;
using UnityEngine.AI;
using Type = Elements.Type;

public enum EnemyControllerState
{
    Idle,Walk,Attack,Stunned
}

public abstract class EnemyController : MonoBehaviour, ICharacterInfo, IDamageable
{
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected EnemyAnimator animator;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private csFogVisibilityAgent visibilityAgent;
    [SerializeField] private new Rigidbody rigidbody;

    protected FSM StateMachine;
    protected List<Attack> Attacks;
    private Transform _target = null;
    private int _stunCounter = 0;
    protected bool Stunned;
    private bool _dead;

    public ModifiableStat Speed
    {
        get;
        private set;
    }
    public float Health
    {
        get;
        private set;
    }
    public LayerMask EnemyMask
    {
        get;
        private set;
    }
    public Type ElementType => enemyData.ElementType;
    public Vector3 SpellOffset => enemyData.SpellOffset;
    public Transform Transform => transform;
    public EnemyData EnemyData => enemyData;
    public List<Transform> Waypoints => waypoints;
    public bool Visible => visibilityAgent == null || visibilityAgent.Visible;
    public Transform Target => _target;
    
    public Action OnDeath;
    
    #region Unity Event Functions
    protected void Start()
    {
        Attacks = new List<Attack>();
        Speed = new ModifiableStat(enemyData.Speed);
        Health = enemyData.Health;
        foreach (AttackData attackData in enemyData.Attacks)
        {
            var attack = attackData.CreateInstance(this);
            Attacks.Add(attack);
        }

        StateMachine = new FSM(); 
        EnemyHealthBars.Instance.SpawnHealthBar(transform, this);
        agent.speed = Speed;
    }

    protected void FixedUpdate()
    {
        agent.speed = Speed;
        StateMachine.Tick();
    }

    private void LateUpdate()
    {
        StateMachine.LateTick();
    }
    #endregion

    public void TakeDamage(float damage, Vector3 knockback, float hitStun, Type attackType)
    {
        float typeMultiplier = GlobalReferences.Instance.TypeManager.GetTypeMultiplier(ElementType, attackType);
        Health -= damage*typeMultiplier;
        if (Health <= 0)
        {
            HandleDeath();
            return;
        }

        StartCoroutine(HandleKnockback(knockback, hitStun));
    }

    private IEnumerator HandleKnockback(Vector3 knockback, float hitStun)
    {
        if (hitStun > 0)
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
            ResourceManager.Instance.Coins.Add(enemyData.CoinsOnDeath);
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    public abstract bool Attack();

    public bool IsPlayerVisible()
    {
        if (GlobalReferences.Instance.Player == null)
            return false;
        bool hit = Physics.Raycast(transform.position,
            GlobalReferences.Instance.Player.transform.position - transform.position, 
            out RaycastHit hitInfo,
            enemyData.AggroRange+1, 
            ~(1 << LayerMask.GetMask("Player", "Walls", "Floor", "Obstacles")));
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
            if (diff.magnitude < enemyData.AggroRange && IsPlayerVisible())
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
            Stunned = true;
        }

        if (knockback != Vector3.zero)
        {
            rigidbody.isKinematic = false;
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
            rigidbody.isKinematic = true;
            Stunned = false;
        }
    }

    public void SetWaypoints(List<Transform> waypoints)
    {
        this.waypoints = waypoints;
    }
}