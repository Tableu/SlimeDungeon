using UnityEngine;

public class SlimeEnemyAnimator : EnemyAnimator
{
    [SerializeField] private Face faces;
    [SerializeField] private Material faceMaterial;
    [SerializeField] private EnemyController controller;
    [SerializeField] private bool meleeAttack;
    private bool _attacking;
    private void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    public void OnAnimatorMove()
    {
        //I'm not sure why this works, but movement breaks without it
        if (_attacking && meleeAttack)
            transform.position += 2*controller.EnemyData.Speed*transform.forward * Time.deltaTime;
    }
    
    public override void ChangeState(EnemyControllerState state)
    {
        switch (state)
        {
            case EnemyControllerState.Idle:
                animator.SetFloat("Speed", 0);
                break;
            case EnemyControllerState.Stunned:
                SetFace(faces.Idleface);
                animator.SetFloat("Speed", 0);
                break;
            case EnemyControllerState.Walk:
                SetFace(faces.WalkFace);
                animator.SetFloat("Speed", controller.Stats.Speed);
                break;
            case EnemyControllerState.Attack:
                SetFace(faces.attackFace);
                _attacking = true;
                animator.SetTrigger("Attack");
                break;
        }
    }
    
    public new void AlertObservers(string message)
    {
        base.AlertObservers(message);
        if (message == "AttackEnded")
            _attacking = false;
    }
}
