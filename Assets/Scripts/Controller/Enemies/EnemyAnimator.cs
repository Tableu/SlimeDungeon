using System;
using UnityEngine;

public abstract class EnemyAnimator : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    [SerializeField] private ParticleSystem stunEffect;
    [SerializeField] private ParticleSystem stunAura;
    [SerializeField] private ParticleSystem chargeEffect;

    public Action<string> OnAlertObservers;

    public void PlayStunEffect()
    {
        if (stunEffect == null || stunAura == null)
            return;
        stunEffect.Play();
        stunAura.Play();
    }

    public void StopStunEffect()
    {
        if (stunEffect == null || stunAura == null)
            return;
        stunAura.Clear();
        stunAura.Stop();
        stunEffect.Stop();
    }

    public void PlayChargeEffect()
    {
        if (chargeEffect == null)
            return;
        chargeEffect.Play();
    }

    public abstract void ChangeState(EnemyControllerState state);

    public void AlertObservers(string message)
    {
        OnAlertObservers.Invoke(message);
    }
}
