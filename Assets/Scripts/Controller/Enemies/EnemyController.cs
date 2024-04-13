using System.Collections;
using System.Collections.Generic;
using Controller;
using Controller.Form;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum EnemyControllerState
{
    Idle,Walk,Attack,Stunned
}

public abstract class EnemyController : Character, ICapturable
{
    public override Vector3 SpellOffset => spellOffset;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private Vector3 spellOffset;
    [SerializeField] protected Animator animator;
    [SerializeField] private Vector2 idleTimeRange = new Vector2(2,3);
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private bool moveWhileAttacking;
    [SerializeField] private ParticleSystem stunEffect;
    [SerializeField] private ParticleSystem stunAura;
    [SerializeField] private GameObject captureEffect;

    private bool _attackingPlayer = false;
    private Transform _target = null;
    private Vector3 _targetPosition;
    private int _tick = 0;
    private int _stunCounter = 0;
    private bool _isSuperEffectiveStunned;

    public override CharacterData CharacterData => enemyData;
    
    public float SuperEffectiveStunMeter
    {
        private set;
        get;
    }

    public EnemyControllerState CurrentState
    {
        protected set;
        get;
    }

    public float StunPercent => (SuperEffectiveStunMeter / CharacterData.StunResist)*100;
    
    #region Unity Event Functions
    private new void Start()
    {
        base.Start();
        EnemyHealthBars.Instance.SpawnHealthBar(transform, this);
        SuperEffectiveStunMeter = 0;
        agent.speed = Speed;
        agent.updateRotation = false;
        agent.SetDestination(new Vector3(Random.Range(waypoints[0].position.x, waypoints[1].position.x), waypoints[0].position.y,
            Random.Range(waypoints[0].position.z, waypoints[1].position.z)));
        ChangeState(EnemyControllerState.Walk);
    }

    protected void Update()
    {
        if (!agent.enabled)
            return;
        if (CurrentState == EnemyControllerState.Walk)
        {
            if (_target != null)
            {
                agent.SetDestination(_targetPosition);
            }

            if (agent.remainingDistance < agent.stoppingDistance)
            {
                
                if (_attackingPlayer)
                { 
                    if(!moveWhileAttacking)
                        StopAgent(); 
                    Attack();
                }
                else
                {
                    StopAgent();
                    ChangeState(EnemyControllerState.Idle);
                    Invoke(nameof(WalkToNextDestination), Random.Range(idleTimeRange.x, idleTimeRange.y));
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (CurrentState != EnemyControllerState.Stunned)
        {
            if (agent.velocity.sqrMagnitude > Mathf.Epsilon)
            {
                transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
            }
            else if (_target != null && _attackingPlayer)
            {
                AttackTargeting.RotateTowards(transform, _target);
            }
        }
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        _tick++;
        if (_tick >= enemyData.DetectTick)
        {
            _tick = 0;
            DetectPlayer();
        }
        agent.speed = Speed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if(enemyData.EnableCollisionDamage)
            CollisionAttack(other);
    }

    #endregion
    #region Base Class Overrides

    public override void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType)
    {
        float typeMultiplier = GlobalReferences.Instance.TypeManager.GetTypeMultiplier(ElementType, attackType);
        Health -= damage*typeMultiplier;
        if (Health <= 0)
        {
            HandleDeath();
            return;
        }

        StartCoroutine(HandleKnockback(knockback, hitStun, typeMultiplier));
        
        if (SuperEffectiveStunMeter < CharacterData.StunResist)
        {
            SuperEffectiveStunMeter += damage * typeMultiplier;
            if (SuperEffectiveStunMeter >= CharacterData.StunResist)
            {
                SuperEffectiveStunMeter = CharacterData.StunResist;
            }
        }
    }

    protected override void HandleDeath()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    protected override IEnumerator HandleKnockback(Vector3 knockback, float hitStun, float typeMultiplier)
    {
        bool isStunMeterFull = SuperEffectiveStunMeter >= CharacterData.StunResist;
        bool isSuperEffective = typeMultiplier > 1;
        bool applySuperEffectiveStun = isStunMeterFull && isSuperEffective && !_isSuperEffectiveStunned;
        if (applySuperEffectiveStun)
        {
            hitStun += 2;
        }
        
        if (hitStun > 0)
        {
            if (applySuperEffectiveStun)
            {
                stunEffect.Play();
                stunAura.Play();
                ApplyStun(knockback);
                _isSuperEffectiveStunned = true;
                yield return new WaitForSeconds(hitStun);
                _isSuperEffectiveStunned = false;
                SuperEffectiveStunMeter = 0;
                stunAura.Clear();
                stunAura.Stop();
                stunEffect.Stop();
                RemoveStun();
            }
            else
            {
                ApplyStun(knockback);
                yield return new WaitForSeconds(hitStun);
                RemoveStun();
            }
        }
        if(_stunCounter == 0)
            Walk();
    }
    #endregion
    protected void StopAgent()
    {
        agent.isStopped = true;
        agent.updateRotation = false;
    }

    protected void Walk()
    {
        agent.isStopped = false;
        agent.updateRotation = true;
        ChangeState(EnemyControllerState.Walk);
    }

    private void Attack()
    {
        if (attacks[0] != null && attacks[0].Begin())
        {
            ChangeState(EnemyControllerState.Attack);
        }
        else
        {
            Walk();
        }
    }
    
    protected void WalkToNextDestination()
    {
        Walk();
        agent.SetDestination(new Vector3(Random.Range(waypoints[0].position.x, waypoints[1].position.x), waypoints[0].position.y,
            Random.Range(waypoints[0].position.z, waypoints[1].position.z)));
    }

    private bool IsPlayerVisible()
    {
        bool hit = Physics.Raycast(transform.position,
            GlobalReferences.Instance.Player.transform.position - transform.position, 
            out RaycastHit hitInfo,
            enemyData.AggroRange+1, 
            ~(1 << LayerMask.GetMask("Player", "Walls", "Floor")));
        return hit && hitInfo.transform.CompareTag("Player");
    }

    private void DetectPlayer()
    {
        if (GlobalReferences.Instance.Player != null && CurrentState != EnemyControllerState.Stunned)
        {
            var diff = transform.position - GlobalReferences.Instance.Player.transform.position;
            if (diff.magnitude >= enemyData.DeAggroRange)
            {
                _target = null;
                _attackingPlayer = false;
                agent.stoppingDistance = enemyData.StoppingDistance;
            }
            else if(diff.magnitude < enemyData.AggroRange && IsPlayerVisible())
            {
                _target = GlobalReferences.Instance.Player.transform;
                _targetPosition = _target.position + new Vector3(Random.Range(-1,1), 0, Random.Range(-1,1));
                _attackingPlayer = true;
                transform.rotation = Quaternion.LookRotation(_target.transform.position - transform.position);
                agent.stoppingDistance = enemyData.AttackRange;
            }
        }
    }
    // Animation Event
    public void AlertObservers(string message)
    {
        if (message.Equals("AttackEnded"))
        {
            if(CurrentState != EnemyControllerState.Stunned)
                Walk();          
        }
    }

    private void ApplyStun(Vector3 knockback)
    {
        CancelInvoke(nameof(WalkToNextDestination));
        ChangeState(EnemyControllerState.Stunned);
        agent.enabled = false;
        agent.updateRotation = false;
        rigidbody.isKinematic = false;
        rigidbody.velocity = Vector3.zero;
        rigidbody.AddForce(knockback, ForceMode.Impulse);
        _stunCounter++;
    }

    private void RemoveStun()
    {
        _stunCounter--;
        if (_stunCounter == 0)
        {
            agent.enabled = true;
            agent.updateRotation = true;
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
            ChangeState(EnemyControllerState.Walk);
        }
    }

    public void SetWaypoints(List<Transform> waypoints)
    {
        this.waypoints = waypoints;
    }

    public void AttemptCapture(float hitStun)
    {
        StartCoroutine(AttemptCaptureCoroutine(hitStun));
    }

    private IEnumerator AttemptCaptureCoroutine(float hitStun)
    {
        bool captured = enemyData.FormData.CaptureDifficulty*Health < Random.Range(21,25);
        if (SuperEffectiveStunMeter >= 100)
        {
            hitStun++;
        }
        ApplyStun(Vector3.zero);
        captureEffect.SetActive(true);
        yield return new WaitForSeconds(hitStun);
        captureEffect.SetActive(false);
        if (captured)
        {
            SpawnFormItem(enemyData.FormData);
            HandleDeath();
        }
        else
        {
            RemoveStun();
        }
    }
    
    private void SpawnFormItem(FormData data)
    {
        GameObject item = Instantiate(data.Item, transform.position, Quaternion.identity);
        FormItem script = item.GetComponent<FormItem>();
        script.Initialize(data);
    }
    
    private void CollisionAttack(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            CollisionData collisionData = enemyData.CollisionData;
            IDamageable health = other.gameObject.GetComponent<IDamageable>();
            health.TakeDamage(collisionData.Damage,(other.transform.position - transform.position).normalized*collisionData.Knockback, 
                collisionData.HitStun, CharacterData.ElementType);
        }
    }
    
    protected abstract void ChangeState(EnemyControllerState state);
}