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
        form.PlayerController.PlayerInputActions.Movement.VerticalPressed.canceled += VerticalMovementCanceled;
        form.PlayerController.PlayerInputActions.Movement.HorizontalPressed.canceled += HorizontalMovementCanceled;
    }

    private void OnDestroy()
    {
        form.PlayerController.PlayerInputActions.Movement.Pressed.started -= MovementPressed;
        form.PlayerController.PlayerInputActions.Movement.Pressed.canceled -= MovementCanceled;
        form.PlayerController.PlayerInputActions.Movement.VerticalPressed.canceled -= VerticalMovementCanceled;
        form.PlayerController.PlayerInputActions.Movement.HorizontalPressed.canceled -= HorizontalMovementCanceled;
    }

    public override void Attack()
    {
        if(form.PlayerController.CurrentAttack != null)
            form.PlayerController.CurrentAttack.End();
    }

    private void MovementPressed(InputAction.CallbackContext context)
    {
        animator.SetBool("Run", true);
    }
    
    private void MovementCanceled(InputAction.CallbackContext context)
    {
        animator.SetBool("Run", false);
        form.PlayerController.rigidbody.velocity = Vector3.zero;
    }

    private void VerticalMovementCanceled(InputAction.CallbackContext context)
    {
        var velocity = form.PlayerController.rigidbody.velocity;
        form.PlayerController.rigidbody.velocity = new Vector3(velocity.x, velocity.y, 0);
    }
    private void HorizontalMovementCanceled(InputAction.CallbackContext context)
    {
        var velocity = form.PlayerController.rigidbody.velocity;
        form.PlayerController.rigidbody.velocity = new Vector3(0, velocity.y, velocity.z);
    }
}
