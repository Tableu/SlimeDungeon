using UnityEngine;

public class RangedEnemyController : EnemyController
{
    private bool _attackAnimationComplete;

    protected new void Start()
    {
        base.Start();
        var patrol = new PatrolState(this, agent, animator);
        var stunned = new StunState(agent, animator);
        var attack = new AttackState(this, agent, animator, true);
        var follow = new FollowState(this, agent, animator);
        StateMachine.AddTransition(patrol, follow, PlayerInRange);
        StateMachine.AddTransition(follow, patrol, PlayerOutOfRange);
        StateMachine.AddTransition(follow, attack, 
            () => Target != null && 
                  Vector3.Distance(Target.position, transform.position) < EnemyData.AttackRange &&
                  CanAttack());
        StateMachine.AddTransition(attack, follow, IsAttackAnimationComplete);
        StateMachine.AddAnyTransition(stunned, () => Stunned);
        StateMachine.AddTransition(stunned, patrol, () => !Stunned);
        StateMachine.SetState(patrol);
        animator.OnAlertObservers += OnAlertObservers;
    }

    public override bool Attack()
    {
        return Attacks.Count > 0 && Attacks[0] != null && Attacks[0].Begin();
    }
    
    private bool CanAttack()
    {
        return Attacks.Count == 0 || !Attacks[0].OnCooldown;
    }

    private void OnAlertObservers(string message)
    {
        if (message.Equals("AttackEnded"))
        {
            _attackAnimationComplete = true;
        }
    }

    private bool IsAttackAnimationComplete()
    {
        var complete = _attackAnimationComplete;
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
