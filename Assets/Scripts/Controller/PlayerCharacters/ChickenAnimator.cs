using Controller.Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChickenAnimator : PlayerCharacterAnimator
{
    private bool _movementEnabledLastFrame;
    public override void Initialize(PlayerCharacter playerCharacter, PlayerInputActions inputActions)
    {
        animator = GetComponent<Animator>();
        this.PlayerCharacter = playerCharacter;
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
    
    private void Update()
    {
        if (inputActions == null)
            return;
        if (_movementEnabledLastFrame != inputActions.Movement.Pressed.enabled && 
            inputActions.Movement.Pressed.IsPressed())
        {
            if (animator != null)
            {
                animator.SetBool("Run", true);
            }
        }
        _movementEnabledLastFrame = inputActions.Movement.Pressed.enabled;
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
        if(animator != null)
            animator.SetBool("Run", true);
    }
    
    private void MovementCanceled(InputAction.CallbackContext context)
    {
        if(animator != null)
            animator.SetBool("Run", false);
    }
}
