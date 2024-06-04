using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeEnemyAnimator : EnemyAnimator
{
    [SerializeField] private Face faces;
    [SerializeField] private Material faceMaterial;
    [SerializeField] private EnemyController controller;
    private void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    public void OnAnimatorMove()
    {
        //I'm not sure why this works, but movement breaks without it
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
                animator.SetFloat("Speed", controller.Speed);
                break;
            case EnemyControllerState.Attack:
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");
                break;
        }
    }
}
