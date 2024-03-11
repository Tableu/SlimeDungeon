using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.Form
{
    public class SlimeFormAnimator : FormAnimator
    {
        public override void Initialize(Form form)
        {
            this.form = form;
            animator = GetComponent<Animator>();
            
            if (form.PlayerController.PlayerInputActions.Movement.Pressed.inProgress)
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", form.Speed);
                }
            }
            
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

