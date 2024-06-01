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
        protected ICharacterInfo CharacterInfo;

        public AttackData Data
        {
            protected set;
            get;
        }
        protected CancellationTokenSource cooldownCancellationTokenSource;
        protected InputAction inputAction;

        public bool OnCooldown
        {
            get;
            private set;
        }

        protected Attack(ICharacterInfo characterInfo, AttackData data)
        {
            this.CharacterInfo = characterInfo;
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
            OnCooldownEvent?.Invoke(duration);
            OnCooldown = true;
            await Task.Run(() =>
            {
                try
                {
                    Task.Delay(TimeSpan.FromSeconds(duration)).Wait(cooldownCancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    //suppressing operationcanceled exceptions
                }
                OnCooldown = false;
            });
        }
        protected bool CheckManaCostAndCooldown()
        {
            if (CharacterInfo is PlayerController playerController)
            {
                bool canCast = playerController.Mana >= Data.ManaCost && !OnCooldown;
                if(canCast)
                    playerController.ApplyManaCost(Data.ManaCost);
                return canCast;
            }
            return !OnCooldown;
        }

        protected void SetLayer(GameObject gameObject)
        {
            gameObject.layer = CharacterInfo is PlayerController
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
        }
        #endregion
        
        public Action<float> OnCooldownEvent;
    }
}
