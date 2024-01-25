using System.Collections;
using Controller;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum EnemyControllerState
{
    Idle,Walk,Attack,Damage,Stunned
}

public abstract class EnemyController : Character
{
    public override Vector3 SpellOffset => spellOffset;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Vector3 spellOffset;
    [SerializeField] protected Animator animator;
    [SerializeField] private Vector2 idleTimeRange = new Vector2(2,3);
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private bool moveWhileAttacking;
    [SerializeField] private ParticleSystem stunEffect;
    [SerializeField] private ParticleSystem stunAura;
    [SerializeField] private GameObject enemyHealthbar;

    private bool _attackingPlayer = false;
    private Transform _target = null;
    private int _tick = 0;

    public override CharacterData CharacterData => enemyData;
    
    public float StunMeter
    {
        private set;
        get;
    }

    public EnemyControllerState CurrentState
    {
        protected set;
        get;
    }

    public float StunPercent => (StunMeter / CharacterData.StunResist)*100;

    private new void Start()
    {
        base.Start();
        GameObject healthbar = Instantiate(enemyHealthbar, transform.position, Quaternion.identity, GlobalReferences.Instance.EnemyHealthbars.transform);
        var script = healthbar.GetComponent<EnemyStatBar>();
        script.Initialize(this);
        StunMeter = 0;
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
                agent.SetDestination(_target.position);
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
    }

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
        if (attacks[0].Begin())
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
            else if(diff.magnitude < enemyData.AggroRange)
            {
                _target = GlobalReferences.Instance.Player.transform;
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
            attacks[0].End();
            if(CurrentState != EnemyControllerState.Stunned)
                Walk();          
        }
    }
    public void OnAnimatorMove()
    {
        // apply root motion to AI
        Vector3 position = animator.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }

    protected override void OnAttackBegin(Attack attack)
    {
        currentAttack = attack;
        StopAgent();
    }
    

    public override void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType)
    {
        if (CurrentState != EnemyControllerState.Damage)
        {
            ChangeState(EnemyControllerState.Damage);
            float typeMultiplier = GlobalReferences.Instance.TypeManager.GetTypeMultiplier(ElementType, attackType);
            Health -= damage*typeMultiplier;
            if (Health <= 0)
            {
                HandleDeath();
                return;
            }

            if (CurrentState != EnemyControllerState.Stunned)
            {
                if (StunMeter < CharacterData.StunResist)
                {
                    StunMeter += damage * typeMultiplier;
                    if (StunMeter >= CharacterData.StunResist)
                    {
                        StunMeter = CharacterData.StunResist;
                    }
                }
                else if (typeMultiplier > 1)
                {
                    StartCoroutine(ApplyStun());
                    return;
                }

                StartCoroutine(ApplyKnockback(knockback, hitStun));
            }
        }
    }

    protected override void HandleDeath()
    {
        /*GameObject item = Instantiate(enemyData.FormData.Item, transform.position, Quaternion.identity);
        FormItem script = item.GetComponent<FormItem>();
        script.Initialize(enemyData.FormData);*/
        StopCoroutine(ApplyStun());
        OnDeath.Invoke();
        Destroy(gameObject);
    }
    
    private IEnumerator ApplyStun()
    {
        CancelInvoke(nameof(WalkToNextDestination));
        ChangeState(EnemyControllerState.Stunned);
        agent.enabled = false;
        agent.updateRotation = false;
        stunEffect.Play();
        stunAura.Play();
        yield return new WaitForSeconds(2);
        StunMeter = 0;
        stunAura.Clear();
        stunAura.Stop();
        agent.enabled = true;
        agent.updateRotation = true;
        ChangeState(EnemyControllerState.Walk);
    }

    protected override IEnumerator ApplyKnockback(Vector3 knockback, float hitStun)
    {
        if (hitStun > 0)
        {
            CancelInvoke(nameof(WalkToNextDestination));
            ChangeState(EnemyControllerState.Damage);
            agent.enabled = false;
            rigidbody.isKinematic = false;
            rigidbody.velocity = knockback;
            yield return new WaitForSeconds(hitStun);
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
            agent.enabled = true;
        }
        if(CurrentState != EnemyControllerState.Stunned)
            Walk();
    }

    protected abstract void ChangeState(EnemyControllerState state);
}