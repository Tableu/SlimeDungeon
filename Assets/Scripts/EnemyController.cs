using System.Collections;
using Controller;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Character
{
    public SlimeAnimationState CurrentState;
    public override Vector3 SpellOffset => spellOffset;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private GameObject smileBody;
    [SerializeField] private Face faces;
    [SerializeField] internal Animator animator;
    [SerializeField] private Vector3 spellOffset;
    [SerializeField] private EnemyData enemyData;
    
    private Material _faceMaterial;
    private int _currentWaypointIndex;
    private bool _attackingPlayer = false;
    private Transform _target;
    private int _tick = 0;

    internal override CharacterData CharacterData => enemyData;

    private new void Start()
    {
        base.Start();
        _faceMaterial = smileBody.GetComponent<Renderer>().materials[1];
        _target = waypoints[0];
        agent.updateRotation = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!agent.enabled)
            return;
        switch (CurrentState)
        {
            case SlimeAnimationState.Idle:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) return;
                StopAgent();
                SetFace(faces.Idleface);
                break;
            case SlimeAnimationState.Walk:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) return;
                agent.isStopped = false;
                agent.updateRotation = true;
                SetFace(faces.Idleface);
                if (waypoints[0] == null) return;
                
                if (_target != null)
                {
                    agent.SetDestination(_target.position);
                }
                else
                {
                    CurrentState = SlimeAnimationState.Idle;
                    break;
                }

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    if (_attackingPlayer)
                    {
                        CurrentState = SlimeAnimationState.Attack;
                    }
                    else
                    {
                        CurrentState = SlimeAnimationState.Idle;
                        //wait 2s before go to next destination
                        Invoke(nameof(WalkToNextDestination), 2f);
                    }
                }
                
                animator.SetFloat("Speed", agent.velocity.magnitude);
                break;
            case SlimeAnimationState.Jump:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) return;
                StopAgent();
                SetFace(faces.jumpFace);
                animator.SetTrigger("Jump");
                break;
            case SlimeAnimationState.Attack:
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) return;
                StopAgent();
                attacks[0].Begin();
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
                break;
        }
    }

    private void LateUpdate()
    {
        if(agent.velocity.sqrMagnitude > Mathf.Epsilon)
            transform.rotation = Quaternion.LookRotation(agent.velocity.normalized);
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

    private void StopAgent()
    {
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);
        agent.updateRotation = false;
    }
    private void SetFace(Texture tex)
    {
        _faceMaterial.SetTexture("_MainTex", tex);
    }
    private void WalkToNextDestination()
    {
        CurrentState = SlimeAnimationState.Walk;
        _currentWaypointIndex = (_currentWaypointIndex + 1) % waypoints.Length;
        _target = waypoints[_currentWaypointIndex];
        agent.SetDestination(_target.position);
        SetFace(faces.WalkFace);
    }

    private void DetectPlayer()
    {
        if (GlobalReferences.Instance.Player != null)
        {
            var diff = transform.position - GlobalReferences.Instance.Player.transform.position;
            if (diff.magnitude >= enemyData.DeAggroRange)
            {
                _target = waypoints[_currentWaypointIndex];
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
            CurrentState = SlimeAnimationState.Walk;           
        }

        if (message.Equals("JumpEnded"))
        {
            CurrentState = SlimeAnimationState.Idle;
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

    public override void Attack()
    {
        animator.SetTrigger("Attack");
    }
    

    public override void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType)
    {
        if (CurrentState != SlimeAnimationState.Damage)
        {
            CurrentState = SlimeAnimationState.Damage;
            base.TakeDamage(damage, knockback, hitStun, attackType);
        }
    }

    protected override IEnumerator ApplyKnockback(Vector3 knockback, float hitStun)
    {
        if (hitStun > 0)
        {
            CancelInvoke(nameof(WalkToNextDestination));
            animator.SetFloat("Speed", 0);
            agent.enabled = false;
            rigidbody.isKinematic = false;
            rigidbody.velocity = knockback;
            yield return new WaitForSeconds(hitStun);
            rigidbody.velocity = Vector3.zero;
            rigidbody.isKinematic = true;
            agent.enabled = true;
        }
        CurrentState = SlimeAnimationState.Walk;
    }
}
