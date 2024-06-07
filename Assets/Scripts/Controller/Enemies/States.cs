using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IState
{
    private readonly EnemyController _controller;
    private readonly NavMeshAgent _agent;
    private readonly EnemyAnimator _animator;
    private bool _walking;
    private float _idleDuration;
    private float _idleTimer;

    public PatrolState(EnemyController controller, NavMeshAgent agent, EnemyAnimator animator)
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

        if (_walking && _agent.remainingDistance < _agent.stoppingDistance)
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
        if (_agent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            _controller.transform.rotation = Quaternion.LookRotation(_agent.velocity.normalized);
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
        _agent.stoppingDistance = _controller.EnemyData.StoppingDistance;
    }

    public void OnExit()
    {
        
    }
}

public class StunState : IState
{
    private readonly NavMeshAgent _agent;
    private readonly EnemyController _controller;
    private readonly EnemyAnimator _animator;

    public StunState(EnemyController controller, NavMeshAgent agent, EnemyAnimator animator)
    {
        _agent = agent;
        _controller = controller;
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
    private readonly NavMeshAgent _agent;
    private readonly EnemyAnimator _animator;
    
    public AttackState(EnemyController controller, NavMeshAgent agent, EnemyAnimator animator)
    {
        _controller = controller;
        _agent = agent;
        _animator = animator;
    }
    public void Tick()
    {
        
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
        _controller.Attack();
        _animator.ChangeState(EnemyControllerState.Attack);
        _agent.isStopped = true;
        _agent.updateRotation = false;
    }

    public void OnExit()
    {
        
    }
}

public class MeleeAttackState : IState
{
    private readonly EnemyController _controller;
    private readonly NavMeshAgent _agent;
    private readonly EnemyAnimator _animator;
    
    public MeleeAttackState(EnemyController controller, NavMeshAgent agent, EnemyAnimator animator)
    {
        _controller = controller;
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
        _controller.Attack();
        _animator.ChangeState(EnemyControllerState.Attack);
        _agent.isStopped = true;
        _agent.updateRotation = false;
    }

    public void OnExit()
    {
        
    }
}

public class FollowState : IState
{
    private readonly EnemyController _controller;
    private readonly NavMeshAgent _agent;
    private readonly EnemyAnimator _animator;
    
    public FollowState(EnemyController controller, NavMeshAgent agent, EnemyAnimator animator)
    {
        _controller = controller;
        _agent = agent;
        _animator = animator;
    }
    public void Tick()
    {
        if (_controller.Target != null)
        {
            _agent.SetDestination(_controller.Target.position+ new Vector3(Random.Range(-1,1), 0, Random.Range(-1,1)));
        }

        _animator.ChangeState(_agent.velocity.sqrMagnitude > Mathf.Epsilon
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
        _agent.stoppingDistance = _controller.EnemyData.AttackRange;
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
    private readonly NavMeshAgent _agent;
    private readonly EnemyAnimator _animator;
    
    public FollowAtDistanceState(EnemyController controller, NavMeshAgent agent, EnemyAnimator animator)
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
        }

        _animator.ChangeState(_agent.velocity.sqrMagnitude > Mathf.Epsilon
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
            _agent.stoppingDistance = 0.1f;
            _agent.SetDestination(_controller.Target.position + (-1*diff.normalized * _controller.EnemyData.AttackRange));
        }
        else if(diff.magnitude > _controller.EnemyData.AttackRange)
        {
            _agent.SetDestination(_controller.Target.position+ new Vector3(Random.Range(-2,2), 0, Random.Range(-2,2)));
            _agent.stoppingDistance = _controller.EnemyData.AttackRange;
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