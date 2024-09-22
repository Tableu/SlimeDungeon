using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.Player
{
    public class SlimeAnimator : CharacterAnimator
    {
        private bool _movementEnabledLastFrame;
        public override void Initialize(Player.Character character, PlayerInputActions inputActions)
        {
            this.Character = character;
            this.inputActions = inputActions;
            animator = GetComponent<Animator>();
            
            if (inputActions.Movement.Pressed.inProgress)
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", character.Stats.Speed);
                }
            }
            
            inputActions.Movement.Pressed.canceled += MovementCanceled;
            inputActions.Movement.Pressed.started += MovementStarted;
        }

        private void Update()
        {
            if (inputActions == null)
                return;
            if (_movementEnabledLastFrame != inputActions.Movement.Pressed.enabled && 
                inputActions.Movement.Pressed.IsPressed())
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", Character.Stats.Speed);
                }
            }
            _movementEnabledLastFrame = inputActions.Movement.Pressed.enabled;
        }

        private void OnDestroy()
        {
            if (inputActions == null)
                return;
            inputActions.Movement.Pressed.canceled -= MovementCanceled;
            inputActions.Movement.Pressed.started -= MovementStarted;
        }
        
        public void OnAnimatorMove()
        {
            //I'm not sure why this works, but movement breaks without it
        }

        private void MovementCanceled(InputAction.CallbackContext context)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }
        }

        private void MovementStarted(InputAction.CallbackContext context)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", Character.Stats.Speed);
            }
        }
    }
}

