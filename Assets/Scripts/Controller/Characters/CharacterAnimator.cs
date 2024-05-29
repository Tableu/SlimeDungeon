using UnityEngine;

namespace Controller.Player
{
    public abstract class CharacterAnimator : MonoBehaviour
    {
        protected Player.Character Character;
        protected Animator animator;
        protected PlayerInputActions inputActions;
        public abstract void Initialize(Player.Character character, PlayerInputActions inputActions);
    }
}