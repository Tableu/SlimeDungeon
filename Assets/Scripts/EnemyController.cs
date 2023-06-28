using System;
using System.Collections;
using Controller;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : Character
{
    public SlimeAnimationState CurrentState;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private GameObject smileBody;
    [SerializeField] private Face faces;
    
    private Material _faceMaterial;
    private int _currentWaypointIndex;
    private bool _attackingPlayer = false;
    private Transform _target;
    private int _tick = 0;
    
    private new void Start()
    {
        base.Start();
        _faceMaterial = smileBody.GetComponent<Renderer>().materials[1];
        _target = waypoints[0];
    }

    // Update is called once per frame
    private void Update()
    {
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

                // agent reaches the destination
                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    if (_attackingPlayer)
                    {
                        CurrentState = SlimeAnimationState.Attack;
                    }
                    else
                    {
                        CurrentState = SlimeAnimationState.Idle;
                        //wait 2s before go to next destionation
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
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
                break;
        }
    }

    private void FixedUpdate()
    {
        _tick++;
        if (_tick >= characterData.DetectTick)
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
            if (diff.magnitude < characterData.AggroRange)
            {
                _target = GlobalReferences.Instance.Player.transform;
                _attackingPlayer = true;
            }
            else
            {
                _target = waypoints[_currentWaypointIndex];
                _attackingPlayer = false;
            }
        }
    }
    // Animation Event
    public new void AlertObservers(string message)
    {
        if (message.Equals("AnimationAttackEnded"))
        {
            CurrentState = SlimeAnimationState.Walk;           
        }

        if (message.Equals("AnimationJumpEnded"))
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
    
    public void OnCollisionEnter(Collision other)
    {
        if (enemyMask == (enemyMask | (1 << other.gameObject.layer)))
        {
            IDamageable health = other.gameObject.GetComponent<IDamageable>();
            health.TakeDamage(characterData.BodyDamage,(other.transform.position - transform.position).normalized*10);
        }
    }

    public override void TakeDamage(float damage, Vector3 knockback)
    {
        if (CurrentState != SlimeAnimationState.Damage)
        {
            CurrentState = SlimeAnimationState.Damage;
            base.TakeDamage(damage, knockback);
        }
    }

    protected override IEnumerator ApplyKnockback(Vector3 knockback)
    {
        CancelInvoke(nameof(WalkToNextDestination));
        animator.SetFloat("Speed", 0);
        agent.enabled = false;
        rigidbody.isKinematic = false;
        rigidbody.velocity = knockback;
        yield return new WaitForSeconds(characterData.HitStun);
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;
        agent.enabled = true;
        CurrentState = SlimeAnimationState.Walk;
    }
}
