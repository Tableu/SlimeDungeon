using UnityEngine;

public class MeleeEnemyController : EnemyController
{
    [SerializeField] private CollisionAttack collisionAttack;
    private bool _attackAnimationComplete;

    protected new void Start()
    {
        base.Start();
        var patrol = new PatrolState(this, agent, animator);
        var stunned = new StunState(this, agent, animator);
        var attack = new MeleeAttackState(this, agent, animator);
        var follow = new FollowState(this, agent, animator);
        StateMachine.AddTransition(patrol, follow, PlayerInRange);
        StateMachine.AddTransition(follow, patrol, PlayerOutOfRange);
        StateMachine.AddTransition(follow, attack, CanAttack);
        StateMachine.AddTransition(attack, follow, IsAttackAnimationComplete);
        StateMachine.AddAnyTransition(stunned, () => StunCounter > 0);
        StateMachine.AddTransition(stunned, patrol, () => StunCounter <= 0);
        StateMachine.SetState(patrol);
        animator.OnAlertObservers += OnAlertObservers;
    }

    public override bool Attack()
    {
        collisionAttack.enabled = true;
        return true;
    }

    private void OnAlertObservers(string message)
    {
        if (message.Equals("AttackEnded"))
        {
            _attackAnimationComplete = true;
        }
    }

    private bool CanAttack()
    {
        return Target != null &&
               Vector3.Distance(Target.position, transform.position) < EnemyData.AttackRange;
    }

    private bool IsAttackAnimationComplete()
    {
        var complete = _attackAnimationComplete;
        collisionAttack.enabled = complete;
        if (complete)
        {
            _attackAnimationComplete = false;
            return true;
        }

        return false;
    }

    private void OnDestroy()
    {
        if(animator != null)
            animator.OnAlertObservers -= OnAlertObservers;
    }
}
