using System;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PatrolState : IState
{
    private readonly EnemyController _controller;
    private readonly AIPath _agent;
    private readonly EnemyAnimator _animator;
    private bool _walking;
    private float _idleDuration;
    private float _idleTimer;

    public PatrolState(EnemyController controller, AIPath agent, EnemyAnimator animator)
    {
        _controller = controller;
        _agent = agent;
        _animator = animator;
    }
    public void Tick()
    {
        _idleTimer += Time.fixedDeltaTime;
        if (!_agent.enabled)
            return;

        if (!_walking && _idleTimer >= _idleDuration)
        {
            _walking = true;
            WalkToNextDestination();
        }

        if (_walking && _agent.remainingDistance < _agent.endReachedDistance)
        {
            _agent.isStopped = true;
            _agent.updateRotation = false;
            _animator.ChangeState(EnemyControllerState.Idle);
            _idleDuration = Random.Range(_controller.EnemyData.IdleTimeRange.x, _controller.EnemyData.IdleTimeRange.y);
            _idleTimer = 0;
            _walking = false;
        }
    }

    public void LateTick()
    {
        if (!_agent.enabled)
            return;
    }
    
    protected void WalkToNextDestination()
    {
        Walk();
        _agent.destination = new Vector3(Random.Range(_controller.Waypoints[0].position.x, _controller.Waypoints[1].position.x), 0,
            Random.Range(_controller.Waypoints[0].position.z, _controller.Waypoints[1].position.z));
    }
    
    private void Walk()
    {
        _agent.isStopped = false;
        _agent.updateRotation = true;
        _animator.ChangeState(EnemyControllerState.Walk);
        _walking = true;
    }

    public void OnEnter()
    {
        _animator.ChangeState(EnemyControllerState.Idle);
        _idleDuration = Random.Range(_controller.EnemyData.IdleTimeRange.x, _controller.EnemyData.IdleTimeRange.y);
        _idleTimer = 0;
        _walking = false;
        _agent.endReachedDistance = 0.1f;
    }

    public void OnExit()
    {
        
    }
}

public class StunState : IState
{
    private readonly AIPath _agent;
    private readonly EnemyAnimator _animator;

    public StunState(AIPath agent, EnemyAnimator animator)
    {
        _agent = agent;
        _animator = animator;
    }
    public void Tick()
    {
        
    }

    public void LateTick()
    {
        
    }

    public void OnEnter()
    {
        _animator.ChangeState(EnemyControllerState.Stunned);
        _animator.PlayStunEffect();
        _agent.enabled = false;
        _agent.updateRotation = false;
    }

    public void OnExit()
    {
        _agent.enabled = true;
        _agent.updateRotation = true;
        _animator.StopStunEffect();
    }
}

public class AttackState : IState
{
    private readonly EnemyController _controller;
    private readonly AIPath _agent;
    private readonly EnemyAnimator _animator;
    private readonly bool _lookAtTarget;
    
    public AttackState(EnemyController controller, AIPath agent, EnemyAnimator animator, bool lookAtTarget)
    {
        _controller = controller;
        _agent = agent;
        _animator = animator;
        _lookAtTarget = lookAtTarget;
    }
    public void Tick()
    {
        
    }

    public void LateTick()
    {
        if (_lookAtTarget && _controller.Target != null)
        {
            AttackTargeting.RotateTowards(_controller.transform, _controller.ProjectileTarget, 
                new Vector3(Random.Range(-0.25f, 0.25f),0f,Random.Range(-0.25f, 0.25f)));
        }
    }

    public void OnEnter()
    {
        _animator.ChangeState(EnemyControllerState.Attack);
        if (!_controller.EnemyData.MoveAndAttack)
        {
            _agent.isStopped = true;
            _agent.updateRotation = false;
        }
        
        if(_controller.ProjectileTarget != null)
        {
            //AttackTargeting.RotateTowards(_controller.transform, _controller.Target,
            //    new Vector3(Random.Range(-0.12f, 0.12f),0f,Random.Range(-0.3f, 0.3f)));
            _controller.transform.LookAt(_controller.ProjectileTarget);
        }
        _controller.Attack();
    }

    public void OnExit()
    {
        
    }
}

public class FollowState : IState
{
    private readonly EnemyController _controller;
    private readonly AIPath _agent;
    private readonly EnemyAnimator _animator;
    
    public FollowState(EnemyController controller, AIPath agent, EnemyAnimator animator)
    {
        _controller = controller;
        _agent = agent;
        _animator = animator;
    }
    public void Tick()
    {
        if (_controller.Target != null)
        {
            _agent.destination = _controller.Target.position;
            _agent.endReachedDistance = _controller.PlayerVisible ? _controller.EnemyData.StoppingDistance : 0;
        }

        _animator.ChangeState(Mathf.RoundToInt(_agent.velocity.sqrMagnitude) > 0
            ? EnemyControllerState.Walk
            : EnemyControllerState.Idle);
    }

    public void LateTick()
    {
        
    }

    public void OnEnter()
    {
        _agent.isStopped = false;
        _agent.updateRotation = true;
    }

    public void OnExit()
    {
    }
}

public class FollowAtDistanceState : IState
{
    private readonly EnemyController _controller;
    private readonly AIPath _agent;
    private readonly EnemyAnimator _animator;
    
    public FollowAtDistanceState(EnemyController controller, AIPath agent, EnemyAnimator animator)
    {
        _controller = controller;
        _agent = agent;
        _animator = animator;
    }
    public void Tick()
    {
        if (_controller.Target != null)
        {
            SetTarget();
            if (!_controller.PlayerVisible)
            {
                _agent.endReachedDistance = 0;
            }
        }

        _animator.ChangeState(_agent.velocity.sqrMagnitude > Mathf.Epsilon
            ? EnemyControllerState.Walk
            : EnemyControllerState.Idle);
    }

    public void LateTick()
    {
        
    }

    private void SetTarget()
    {
        var diff = _controller.Target.position - _controller.Transform.position;
        if (diff.magnitude <= _controller.EnemyData.AttackRange)
        {
            _agent.endReachedDistance = _controller.EnemyData.StoppingDistance;
            _agent.destination = _controller.Target.position + (-1*diff.normalized * _controller.EnemyData.AttackRange);
        }
        else if(diff.magnitude > _controller.EnemyData.AttackRange)
        {
            _agent.destination = _controller.Target.position+ new Vector3(Random.Range(-2,2), 0, Random.Range(-2,2));
            _agent.endReachedDistance = _controller.EnemyData.AttackRange;
        }
    }

    public void OnEnter()
    {
        _agent.isStopped = false;
        _agent.updateRotation = true;
    }

    public void OnExit()
    {
    }
}

public class WanderingState : IState
{
    private readonly EnemyController _controller;
    private readonly AIPath _agent;
    private Vector3 _direction;
    private Vector3 _destinationNormal;
    private bool _initialized = false;
    
    public WanderingState(EnemyController controller, AIPath agent)
    {
        _controller = controller;
        _agent = agent;
        _direction = Vector3.left;
    }
    public void Tick()
    {
        if (_agent.remainingDistance < 1)
        {
            SetDestination();
        }
    }

    public void LateTick()
    {
        if (_controller.Target != null)
        {
            AttackTargeting.RotateTowards(_controller.transform, _controller.ProjectileTarget, 
                new Vector3(Random.Range(-0.25f, 0.25f),0f,Random.Range(-0.25f, 0.25f)));
        }
    }

    public void OnEnter()
    {
        if (!_initialized)
        {
            _initialized = true;
            _agent.isStopped = false;
            _agent.updateRotation = false;
            _agent.endReachedDistance = 1f;
            _direction = Quaternion.AngleAxis(45 + (Random.Range(0, 3) * 90), Vector3.up)*Vector3.right;
            Physics.Raycast(_controller.Transform.position, _direction, out RaycastHit hitInfo, Single.PositiveInfinity,
                LayerMask.GetMask("Walls", "Obstacles"));
            _destinationNormal = hitInfo.normal;
            _agent.destination = hitInfo.point;
        }
    }

    public void OnExit()
    {
        
    }

    private void SetDestination()
    {
        _direction = Vector3.Reflect(_direction, _destinationNormal);
        Physics.Raycast(_controller.Transform.position, _direction, out RaycastHit hitInfo, Single.PositiveInfinity,
            LayerMask.GetMask("Walls", "Obstacles"));
        _destinationNormal = hitInfo.normal;
        _agent.destination = hitInfo.point;
    }
}