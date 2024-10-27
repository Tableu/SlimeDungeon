using Controller;
using UnityEngine;

public class ChickenBossController : EnemyController
{
    [SerializeField] private EnemyData secondPhaseData;
    [SerializeField] private ChickenBossAnimator chickenBossAnimator;
    [SerializeField] private GameObject bossHealthBar;
    private bool _attackAnimationComplete;
    private int _attackIndex = 0;
    private bool _inSecondPhase = false;
    private bool _detectedPlayer = false;

    protected new void Start()
    {
        base.Start();
        var patrol = new PatrolState(this, agent, animator);
        var attack = new AttackState(this, agent, animator, true);
        var follow = new FollowState(this, agent, animator);
        StateMachine.AddTransition(patrol, follow, PlayerInRange);
        StateMachine.AddTransition(follow, patrol, PlayerOutOfRange);
        StateMachine.AddTransition(follow, attack, CanAttack);
        StateMachine.AddTransition(attack, follow, IsAttackAnimationComplete);
        StateMachine.SetState(patrol);
        animator.OnAlertObservers += OnAlertObservers;
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();
        if(Stats.Health <= secondPhaseData.Health && !_inSecondPhase)
            StartSecondPhase();
        if (!_detectedPlayer && PlayerVisible)
        {
            GameObject healthBar = Instantiate(bossHealthBar, GlobalReferences.Instance.Canvas.transform);
            BossHealthBar script = healthBar.GetComponent<BossHealthBar>();
            script.Initialize(this, "Chicken Lord");
            _detectedPlayer = true;
        }
    }

    private void StartSecondPhase()
    {
        enemyData = secondPhaseData;
        foreach (Attack attack in Attacks)
        {
            attack.End();
        }
        Attacks.Clear();
        Stats.Speed.UpdateBaseValue(enemyData.Speed);
        foreach (AttackData attackData in enemyData.Attacks)
        {
            var attack = attackData.CreateInstance(Stats, transform);
            Attacks.Add(attack);
        }
        agent.Speed = Stats.Speed;
        _inSecondPhase = true;
        chickenBossAnimator.SwitchToSecondPhase();
    }

    public override bool Attack()
    {
        return Attacks.Count > 0 && Attacks[_attackIndex] != null && Attacks[_attackIndex].Begin();;
    }
    
    private bool CanAttack()
    {
        if (Attacks.Count == 0)
            return false;
        if (Target != null && Vector3.Distance(Target.position, transform.position) < EnemyData.AttackRange && PlayerVisible)
        {
            for (var index = 0; index < Attacks.Count; index++)
            {
                Attack attack = Attacks[index];
                if (!attack.OnCooldown)
                {
                    _attackIndex = index;
                    return true;
                }
            }
        }

        foreach (Attack attack in Attacks)
        {
            attack.End();
        }
        return false;
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
