public class ChickenBossAnimator : EnemyAnimator
{
    private bool _isSecondPhase = false;
    public void SwitchToSecondPhase()
    {
        _isSecondPhase = true;
        PlayChargeEffect();
    }
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
                animator.SetBool(_isSecondPhase ? "Run" : "Walk", true);
                break;
            case EnemyControllerState.Attack:
                OnAlertObservers.Invoke("AttackEnded");
                break;
        }
    }
}
