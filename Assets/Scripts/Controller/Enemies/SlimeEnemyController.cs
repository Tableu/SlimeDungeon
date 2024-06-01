using UnityEngine;

public class SlimeEnemyController : EnemyController
{
    [SerializeField] private Face faces;
    [SerializeField] private Material faceMaterial;
    
    private void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        if (CurrentState == EnemyControllerState.Walk || CurrentState == EnemyControllerState.Idle)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }
    
    protected override void ChangeState(EnemyControllerState state)
    {
        CurrentState = state;
        switch (CurrentState)
        {
            case EnemyControllerState.Idle:
            case EnemyControllerState.Stunned:
                SetFace(faces.Idleface);
                animator.SetFloat("Speed", 0);
                break;
            case EnemyControllerState.Walk:
                SetFace(faces.WalkFace);
                animator.SetFloat("Speed", agent.velocity.magnitude);
                break;
            case EnemyControllerState.Attack:
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
                StopAgent();
                break;
        }
    }
    
    public void OnAnimatorMove()
    {
        //I'm not sure why this works, but movement breaks without it
    }
}
