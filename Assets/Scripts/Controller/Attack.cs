using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller
{
    public enum AnimationState
    {
        AnimationDamageEnded,
        AnimationAttackEnded,
        AnimationJumpEnded
    }
    [Serializable]
    public abstract class Attack : ScriptableObject
    {
        protected Character _character;
        protected PlayerInputActions _inputActions;
        public abstract void Equip(Character character, PlayerInputActions inputActions = null);
        public abstract void Drop();
        public abstract void Begin(InputAction.CallbackContext callbackContext);
        public abstract void End();
        internal abstract void PassMessage(AnimationState state);
    }
}
