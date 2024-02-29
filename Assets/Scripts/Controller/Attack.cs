using System;
using System.Threading;
using System.Threading.Tasks;
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
        protected CancellationTokenSource cooldownCancellationTokenSource;
        protected bool onCooldown = false;
        protected InputAction inputAction;

        public AttackData Data => data;

        protected Attack(Character character, AttackData data)
        {
            this.character = character;
            this.data = data;
        }

        public abstract bool Begin();

        public virtual void Begin(InputAction.CallbackContext callbackContext)
        {
            Begin();
        }

        public abstract void End();
        public virtual void End(InputAction.CallbackContext callbackContext)
        {
            End();
        }

        public abstract void LinkInput(InputAction action);
        public abstract void UnlinkInput();

        public virtual void CleanUp()
        {
            cooldownCancellationTokenSource?.Cancel();
        }

        public virtual async void Cooldown(float duration)
        {
            cooldownCancellationTokenSource = new CancellationTokenSource();
            if (duration == 0)
                return;
            OnCooldown?.Invoke(duration);
            onCooldown = true;
            await Task.Run(() =>
            {
                Task.Delay(TimeSpan.FromSeconds(duration)).Wait(cooldownCancellationTokenSource.Token);
                onCooldown = false;
            });
        }
        
        public Action<float> OnCooldown;
    }
}
