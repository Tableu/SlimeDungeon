public class ChickenBossAnimator : EnemyAnimator
{
    public override void ChangeState(EnemyControllerState state)
    {
        switch (state)
        {
            case EnemyControllerState.Idle:
            case EnemyControllerState.Stunned:
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
                break;
            case EnemyControllerState.Walk:
                animator.SetBool("Walk", true);
                animator.SetBool("Run", false);
                break;
            case EnemyControllerState.Attack:
                OnAlertObservers.Invoke("AttackEnded");
                break;
        }
    }
}
