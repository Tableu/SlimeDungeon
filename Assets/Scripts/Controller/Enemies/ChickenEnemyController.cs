public class ChickenEnemyController : EnemyController
{
    protected override void ChangeState(EnemyControllerState state)
    {
        CurrentState = state;
        switch (CurrentState)
        {
            case EnemyControllerState.Idle:
                animator.SetBool("Walk", false);
                animator.SetBool("Run", false);
                break;
            case EnemyControllerState.Walk:
                animator.SetBool("Walk", false);
                animator.SetBool("Run", true);
                break;
            case EnemyControllerState.Attack:
                attacks[0].End();
                Walk();   
                break;
            case EnemyControllerState.Damage:
                animator.SetBool("Walk", true);
                animator.SetBool("Run", false);
                break;
        }
    }
}
