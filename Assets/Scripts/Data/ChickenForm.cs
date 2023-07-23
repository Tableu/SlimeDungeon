using Controller.Form;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChickenForm : Form
{
    private PlayerController _playerController;
    public override void Equip(PlayerController playerController)
    {
        health = data.Health;
        speed = data.Speed;
        elementType = data.ElementType;
        _playerController = playerController;
        animator = GetComponent<Animator>();

        _playerController.playerInputActions.Movement.Pressed.started += MovementPressed;
        _playerController.playerInputActions.Movement.Pressed.canceled += MovementCanceled;
        _playerController.playerInputActions.Movement.VerticalPressed.canceled += VerticalMovementCanceled;
        _playerController.playerInputActions.Movement.HorizontalPressed.canceled += HorizontalMovementCanceled;
    }

    public override void Drop()
    {
        _playerController.playerInputActions.Movement.Pressed.started -= MovementPressed;
        _playerController.playerInputActions.Movement.Pressed.canceled -= MovementCanceled;
        _playerController.playerInputActions.Movement.VerticalPressed.canceled -= VerticalMovementCanceled;
        _playerController.playerInputActions.Movement.HorizontalPressed.canceled -= HorizontalMovementCanceled;
    }

    public override void Attack()
    {
        
    }

    private void MovementPressed(InputAction.CallbackContext context)
    {
        animator.SetBool("Run", true);
    }
    
    private void MovementCanceled(InputAction.CallbackContext context)
    {
        animator.SetBool("Run", false);
        _playerController.rigidbody.velocity = Vector3.zero;
    }

    private void VerticalMovementCanceled(InputAction.CallbackContext context)
    {
        var velocity = _playerController.rigidbody.velocity;
        _playerController.rigidbody.velocity = new Vector3(velocity.x, velocity.y, 0);
    }
    private void HorizontalMovementCanceled(InputAction.CallbackContext context)
    {
        var velocity = _playerController.rigidbody.velocity;
        _playerController.rigidbody.velocity = new Vector3(0, velocity.y, velocity.z);
    }
}
