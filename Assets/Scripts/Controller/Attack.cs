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
        protected CharacterStats CharacterStats;
        protected Transform Transform;

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

        protected Attack(CharacterStats characterStats, AttackData data, Transform transform)
        {
            this.CharacterStats = characterStats;
            Data = data;
            Transform = transform;
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
        protected bool CheckCooldown() => !OnCooldown;

        protected GameObject SpawnProjectile(Transform transform)
        {
            Vector3 offset = CharacterStats.SpellOffset + Data.SpawnOffset;
            GameObject projectile = GameObject.Instantiate(Data.Prefab,
                transform.position + new Vector3(offset.x*transform.forward.x, offset.y, offset.z*transform.forward.z), Quaternion.identity);
            return projectile;
        }

        protected void SetLayer(GameObject gameObject)
        {
            gameObject.layer = CharacterStats.EnemyMask == (CharacterStats.EnemyMask | (1 << LayerMask.NameToLayer("Enemy")))
                ? LayerMask.NameToLayer("PlayerAttacks")
                : LayerMask.NameToLayer("EnemyAttacks");
        }
        #endregion
        
        public Action<float> OnCooldownEvent;
    }
}
