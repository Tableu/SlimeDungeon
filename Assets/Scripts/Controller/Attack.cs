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
        
        public abstract void Begin(InputAction.CallbackContext callbackContext);
        public abstract void End();
        internal abstract void PassMessage(AnimationState state);
        public abstract void CleanUp();

        public Action OnSpellCast;
        public Action OnCooldown;
    }
}
