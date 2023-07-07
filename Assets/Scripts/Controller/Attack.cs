using System;
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
    public abstract class Attack : ScriptableObject
    {
        protected Character character;
        public abstract void Equip(Character character);
        public abstract void Drop();
        public abstract void Begin(InputAction.CallbackContext callbackContext);
        public abstract void End();
        internal abstract void PassMessage(AnimationState state);

        public Action OnSpellCast;
    }
}
