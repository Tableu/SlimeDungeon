using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.Form
{
    public class SlimeForm : FormAnimator
    {
        public override void Initialize(Form form)
        {
            this.form = form;
            animator = GetComponent<Animator>();
            
            form.PlayerController.PlayerInputActions.Movement.Pressed.canceled += MovementCanceled;
            form.PlayerController.PlayerInputActions.Movement.Pressed.started += MovementStarted;
        }

        private void OnDestroy()
        {
            form.PlayerController.PlayerInputActions.Movement.Pressed.canceled -= MovementCanceled;
            form.PlayerController.PlayerInputActions.Movement.Pressed.started -= MovementStarted;
        }
        
        public void OnAnimatorMove()
        {
            Vector3 position = animator.rootPosition;
            form.PlayerController.transform.position = position;
        }
        public void AlertObservers(string message)
        {
            if(form.PlayerController.currentAttack != null && Enum.TryParse(message, out Controller.AnimationState state))
                form.PlayerController.currentAttack.PassMessage(state);
        }

        public override void Attack()
        {
            animator.SetTrigger("Attack");
        }

        private void MovementCanceled(InputAction.CallbackContext context)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }
            form.PlayerController.rigidbody.velocity = Vector3.zero;
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

