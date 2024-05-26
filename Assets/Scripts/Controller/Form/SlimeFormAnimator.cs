using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.Form
{
    public class SlimeFormAnimator : FormAnimator
    {
        private bool _movementEnabledLastFrame;
        public override void Initialize(Form form, PlayerInputActions inputActions)
        {
            this.form = form;
            this.inputActions = inputActions;
            animator = GetComponent<Animator>();
            
            if (inputActions.Movement.Pressed.inProgress)
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", form.Speed);
                }
            }
            
            inputActions.Movement.Pressed.canceled += MovementCanceled;
            inputActions.Movement.Pressed.started += MovementStarted;
        }

        private void Update()
        {
            if (_movementEnabledLastFrame != inputActions.Movement.Pressed.enabled && 
                inputActions.Movement.Pressed.IsPressed())
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", form.Speed);
                }
            }
            _movementEnabledLastFrame = inputActions.Movement.Pressed.enabled;
        }

        private void OnDestroy()
        {
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
                animator.SetFloat("Speed", form.Speed);
            }
        }
    }
}

