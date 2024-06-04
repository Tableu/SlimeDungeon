using System;
using UnityEngine;

public abstract class EnemyAnimator : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] private ParticleSystem stunEffect;
    [SerializeField] private ParticleSystem stunAura;

    public Action<string> OnAlertObservers;

    public void PlayStunEffect()
    {
        stunEffect.Play();
        stunAura.Play();
    }

    public void StopStunEffect()
    {
        stunAura.Clear();
        stunAura.Stop();
        stunEffect.Stop();
    }
    
    public abstract void ChangeState(EnemyControllerState state);

    public void AlertObservers(string message)
    {
        OnAlertObservers.Invoke(message);
    }
}
