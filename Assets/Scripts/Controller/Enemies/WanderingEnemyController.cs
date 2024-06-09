using UnityEngine;

public class WanderingEnemyController : EnemyController
{
    private bool _attackAnimationComplete;

    protected new void Start()
    {
        base.Start();
        var stunned = new StunState(agent, animator);
        var attack = new AttackState(this, agent, animator, true);
        var bounce = new WanderingState(this, agent);
        StateMachine.AddTransition(bounce, attack, 
            () => Target != null && 
                  Vector3.Distance(Target.position, transform.position) < EnemyData.AttackRange &&
                  CanAttack());
        StateMachine.AddTransition(attack, bounce, IsAttackAnimationComplete);
        StateMachine.AddAnyTransition(stunned, () => StunCounter > 0);
        StateMachine.AddTransition(stunned, bounce, () => StunCounter <= 0);
        StateMachine.SetState(bounce);
        animator.OnAlertObservers += OnAlertObservers;
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();
        if (!PlayerInRange())
        {
            PlayerOutOfRange();
        }
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
