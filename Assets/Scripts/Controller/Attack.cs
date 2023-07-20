using System;
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

        protected Attack(Character character, AttackData data)
        {
            this.character = character;
            this.data = data;
        }
        
        public abstract void Begin();

        public virtual void Begin(InputAction.CallbackContext callbackContext)
        {
            Begin();
        }
        public abstract void End();
        public virtual void End(InputAction.CallbackContext callbackContext)
        {
            End();
        }
        internal abstract void PassMessage(AnimationState state);
        public abstract void CleanUp();

        public Action OnSpellCast;
        public Action OnCooldown;
    }
}
