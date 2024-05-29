using UnityEngine;

namespace Controller.Form
{
    public abstract class CharacterAnimator : MonoBehaviour
    {
        protected Character.Character Character;
        protected Animator animator;
        protected PlayerInputActions inputActions;
        public abstract void Initialize(Character.Character character, PlayerInputActions inputActions);
    }
}