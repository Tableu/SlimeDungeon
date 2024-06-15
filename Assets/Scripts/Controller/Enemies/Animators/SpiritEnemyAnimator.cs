using UnityEngine;

public class SpiritEnemyAnimator : EnemyAnimator
{
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private EnemyController controller;
    [SerializeField] private Light light;
    public override void ChangeState(EnemyControllerState state)
    {
        switch (state)
        {
            case EnemyControllerState.Attack:
                OnAlertObservers.Invoke("AttackEnded");
                break;
        }
    }

    private void Update()
    {
        if (controller.Visible && !particleSystem.isPlaying)
        {
            particleSystem.Play();
            light.enabled = true;
        }
        else if(!controller.Visible && particleSystem.isPlaying)
        {
            particleSystem.Stop();
            light.enabled = false;
        }
    }
}
