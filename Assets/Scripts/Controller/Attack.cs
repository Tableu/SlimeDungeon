using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    [Serializable]
    public abstract class Attack
    {
        protected Character character;

        public AttackData Data
        {
            protected set;
            get;
        }
        protected CancellationTokenSource cooldownCancellationTokenSource;
        protected bool onCooldown;
        protected InputAction inputAction;

        

        protected Attack(Character character, AttackData data)
        {
            this.character = character;
            Data = data;
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

        #region Helper Functions
        protected virtual async void Cooldown(float duration)
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
        protected bool CheckManaCostAndCooldown()
        {
            return character.Mana >= Data.ManaCost && !onCooldown;
        }

        protected void SetLayer(GameObject gameObject)
        {
            gameObject.layer = character is PlayerController
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
        }
        #endregion
        
        public Action<float> OnCooldown;
    }
}
