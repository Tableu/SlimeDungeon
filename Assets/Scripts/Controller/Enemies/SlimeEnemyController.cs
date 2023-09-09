using UnityEngine;

public class SlimeEnemyController : EnemyController
{
    [SerializeField] private Face faces;
    [SerializeField] private Material faceMaterial;
    
    private void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    private new void Update()
    {
        base.Update();
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
                SetFace(faces.Idleface);
                break;
            case EnemyControllerState.Walk:
                SetFace(faces.WalkFace);
                animator.SetFloat("Speed", agent.velocity.magnitude);
                break;
            case EnemyControllerState.Attack:
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
                break;
            case EnemyControllerState.Damage:
                animator.SetFloat("Speed", 0);
                break;
        }
    }
}
