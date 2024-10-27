using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class PatrolState : IState
{
    private readonly EnemyController _controller;
    private readonly EnemyPathingController _agent;
    private readonly EnemyAnimator _animator;
    private bool _walking;
    private float _idleDuration;
    private float _idleTimer;

    public PatrolState(EnemyController controller, EnemyPathingController agent, EnemyAnimator animator)
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

        if (_walking && _agent.RemainingDistance < _agent.StoppingDistance)
        {
            _agent.IsStopped = true;
            _agent.UpdateRotation = false;
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
        if (_agent.Velocity.sqrMagnitude > Mathf.Epsilon)
        {
            _controller.transform.rotation = Quaternion.LookRotation(_agent.Velocity.normalized);
        }
    }
    
    protected void WalkToNextDestination()
    {
        Walk();
        _agent.SetDestination(new Vector3(Random.Range(_controller.Waypoints[0].position.x, _controller.Waypoints[1].position.x), _controller.Waypoints[0].position.y,
            Random.Range(_controller.Waypoints[0].position.z, _controller.Waypoints[1].position.z)));
    }
    
    private void Walk()
    {
        _agent.IsStopped = false;
        _agent.UpdateRotation = true;
        _animator.ChangeState(EnemyControllerState.Walk);
        _walking = true;
    }

    public void OnEnter()
    {
        _animator.ChangeState(EnemyControllerState.Idle);
        _idleDuration = Random.Range(_controller.EnemyData.IdleTimeRange.x, _controller.EnemyData.IdleTimeRange.y);
        _idleTimer = 0;
        _walking = false;
        _agent.StoppingDistance = 0.1f;
    }

    public void OnExit()
    {
        
    }
}

public class StunState : IState
{
    private readonly EnemyPathingController _agent;
    private readonly EnemyAnimator _animator;

    public StunState(EnemyPathingController agent, EnemyAnimator animator)
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
        _agent.UpdateRotation = false;
    }

    public void OnExit()
    {
        _agent.enabled = true;
        _agent.UpdateRotation = true;
        _animator.StopStunEffect();
    }
}

public class AttackState : IState
{
    private readonly EnemyController _controller;
    private readonly EnemyPathingController _agent;
    private readonly EnemyAnimator _animator;
    private readonly bool _lookAtTarget;
    
    public AttackState(EnemyController controller, EnemyPathingController agent, EnemyAnimator animator, bool lookAtTarget)
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
            AttackTargeting.RotateTowards(_controller.transform, _controller.Target);
        }
    }

    public void OnEnter()
    {
        _controller.Attack();
        _animator.ChangeState(EnemyControllerState.Attack);
        if (!_controller.EnemyData.MoveAndAttack)
        {
            _agent.IsStopped = true;
            _agent.UpdateRotation = false;
        }
        if(_controller.Target != null)
        {
            AttackTargeting.RotateTowards(_controller.transform, _controller.Target);
        }
    }

    public void OnExit()
    {
        
    }
}

public class FollowState : IState
{
    private readonly EnemyController _controller;
    private readonly EnemyPathingController _agent;
    private readonly EnemyAnimator _animator;
    
    public FollowState(EnemyController controller, EnemyPathingController agent, EnemyAnimator animator)
    {
        _controller = controller;
        _agent = agent;
        _animator = animator;
    }
    public void Tick()
    {
        if (_controller.Target != null)
        {
            _agent.SetDestination(_controller.Target.position);
            _agent.StoppingDistance = _controller.PlayerVisible ? _controller.EnemyData.StoppingDistance : 0;
        }

        _animator.ChangeState(Mathf.RoundToInt(_agent.Velocity.sqrMagnitude) > 0
            ? EnemyControllerState.Walk
            : EnemyControllerState.Idle);
    }

    public void LateTick()
    {
        if (_controller.Target != null)
        {
            AttackTargeting.RotateTowards(_controller.transform, _controller.Target);
        }
    }

    public void OnEnter()
    {
        _agent.IsStopped = false;
        _agent.UpdateRotation = true;
    }

    public void OnExit()
    {
    }
}

public class FollowAtDistanceState : IState
{
    private readonly EnemyController _controller;
    private readonly EnemyPathingController _agent;
    private readonly EnemyAnimator _animator;
    
    public FollowAtDistanceState(EnemyController controller, EnemyPathingController agent, EnemyAnimator animator)
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
                _agent.StoppingDistance = 0;
            }
        }

        _animator.ChangeState(_agent.Velocity.sqrMagnitude > Mathf.Epsilon
            ? EnemyControllerState.Walk
            : EnemyControllerState.Idle);
    }

    public void LateTick()
    {
        if (_controller.Target != null)
        {
            AttackTargeting.RotateTowards(_controller.transform, _controller.Target);
        }
    }

    private void SetTarget()
    {
        var diff = _controller.Target.position - _controller.Transform.position;
        if (diff.magnitude <= _controller.EnemyData.AttackRange)
        {
            _agent.StoppingDistance = _controller.EnemyData.StoppingDistance;
            _agent.SetDestination(_controller.Target.position + (-1*diff.normalized * _controller.EnemyData.AttackRange));
        }
        else if(diff.magnitude > _controller.EnemyData.AttackRange)
        {
            _agent.SetDestination(_controller.Target.position+ new Vector3(Random.Range(-2,2), 0, Random.Range(-2,2)));
            _agent.StoppingDistance = _controller.EnemyData.AttackRange;
        }
    }

    public void OnEnter()
    {
        _agent.IsStopped = false;
        _agent.UpdateRotation = true;
    }

    public void OnExit()
    {
    }
}

public class WanderingState : IState
{
    private readonly EnemyController _controller;
    private readonly EnemyPathingController _agent;
    private Vector3 _direction;
    private Vector3 _destinationNormal;
    private bool _initialized = false;
    
    public WanderingState(EnemyController controller, EnemyPathingController agent)
    {
        _controller = controller;
        _agent = agent;
        _direction = Vector3.left;
    }
    public void Tick()
    {
        if (Vector3.Distance(_controller.Transform.position, _agent.Destination) < 1)
        {
            SetDestination();
        }
    }

    public void LateTick()
    {
        if (_controller.Target != null)
        {
            AttackTargeting.RotateTowards(_controller.transform, _controller.Target);
        }
    }

    public void OnEnter()
    {
        if (!_initialized)
        {
            _initialized = true;
            _agent.IsStopped = false;
            _agent.UpdateRotation = true;
            _agent.StoppingDistance = 1f;
            _direction = Quaternion.AngleAxis(45 + (Random.Range(0, 3) * 90), Vector3.up)*Vector3.right;
            Physics.Raycast(_controller.Transform.position, _direction, out RaycastHit hitInfo, Single.PositiveInfinity,
                LayerMask.GetMask("Walls", "Obstacles"));
            _destinationNormal = hitInfo.normal;
            _agent.SetDestination(hitInfo.point);
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
        _agent.SetDestination(hitInfo.point);
    }
}