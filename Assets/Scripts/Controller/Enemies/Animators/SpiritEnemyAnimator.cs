
public class SpiritEnemyAnimator : EnemyAnimator
{
    public override void ChangeState(EnemyControllerState state)
    {
        switch (state)
        {
            case EnemyControllerState.Attack:
                OnAlertObservers.Invoke("AttackEnded");
                break;
        }
    }
}
