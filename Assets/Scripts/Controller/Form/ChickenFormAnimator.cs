using Controller.Form;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChickenFormAnimator : FormAnimator
{
    public override void Initialize(Form form)
    {
        animator = GetComponent<Animator>();
        this.form = form;
        form.PlayerController.PlayerInputActions.Movement.Pressed.started += MovementPressed;
        form.PlayerController.PlayerInputActions.Movement.Pressed.canceled += MovementCanceled;
    }

    private void OnDestroy()
    {
        form.PlayerController.PlayerInputActions.Movement.Pressed.started -= MovementPressed;
        form.PlayerController.PlayerInputActions.Movement.Pressed.canceled -= MovementCanceled;
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
