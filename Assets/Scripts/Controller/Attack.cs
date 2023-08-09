using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public enum AnimationState
    {
        DamageEnded,
        AttackEnded,
        JumpEnded
    }
    [Serializable]
    public abstract class Attack
    {
        protected Character character;
        protected AttackData data;
        protected CancellationTokenSource cancellationTokenSource;
        protected bool cooldownActive = false;

        public AttackData Data => data;

        protected Attack(Character character, AttackData data)
        {
            this.character = character;
            this.data = data;
        }

        public virtual void Begin()
        {
            OnBegin?.Invoke(this);
        }

        public virtual void Begin(InputAction.CallbackContext callbackContext)
        {
            Begin();
        }

        public virtual void End()
        {
            OnEnd?.Invoke(this);
        }
        public virtual void End(InputAction.CallbackContext callbackContext)
        {
            End();
        }
        internal abstract void PassMessage(AnimationState state);
        public abstract void CleanUp();

        public virtual async void Cooldown(float duration)
        {
            cancellationTokenSource = new CancellationTokenSource();
            if (duration == 0)
                return;
            float time = 0;
            cooldownActive = true;
            while (time < duration)
            {
                time += Time.fixedDeltaTime;
                OnCooldown?.Invoke(time / duration);
                await Task.Yield();
            }
            cooldownActive = false;
        }

        public Action<Attack> OnBegin;
        public Action<Attack> OnEnd;
        public Action<float> OnCooldown;
    }
}
