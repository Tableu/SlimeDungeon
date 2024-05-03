using Controller.Form;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChickenFormAnimator : FormAnimator
{
    public override void Initialize(Form form, PlayerInputActions inputActions)
    {
        animator = GetComponent<Animator>();
        this.form = form;
        this.inputActions = inputActions;
        if (inputActions.Movement.Pressed.inProgress)
        {
            if (animator != null)
            {
                animator.SetBool("Run", true);
            }
        }
        inputActions.Movement.Pressed.started += MovementPressed;
        inputActions.Movement.Pressed.canceled += MovementCanceled;
    }

    private void OnDestroy()
    {
        if (inputActions != null)
        {
            inputActions.Movement.Pressed.started -= MovementPressed;
            inputActions.Movement.Pressed.canceled -= MovementCanceled;
        }
    }

    private void MovementPressed(InputAction.CallbackContext context)
    {
        animator.SetBool("Run", true);
    }
    
    private void MovementCanceled(InputAction.CallbackContext context)
    {
        animator.SetBool("Run", false);
    }
}
