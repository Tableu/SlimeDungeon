using UnityEngine.InputSystem;

namespace Controller.Form
{
    public class BaseForm : Form
    {
        private PlayerController _playerController;
        public override void Equip(PlayerController playerController)
        {
            health = data.Health;
            speed = data.Speed;
            elementType = data.ElementType;
            _playerController = playerController;
            
            playerController.ChangeModel(data);
            playerController.playerInputActions.Movement.Pressed.canceled += MovementCanceled;
            playerController.playerInputActions.Movement.Pressed.started += MovementStarted;
        }

        public override void Drop()
        {
            _playerController.playerInputActions.Movement.Pressed.canceled -= MovementCanceled;
            _playerController.playerInputActions.Movement.Pressed.started -= MovementStarted;
        }

        private void MovementCanceled(InputAction.CallbackContext context)
        {
            if (_playerController.animator != null)
            {
                _playerController.animator.SetFloat("Speed", 0);
            }
        }

        private void MovementStarted(InputAction.CallbackContext context)
        {
            if (_playerController.animator != null)
            {
                _playerController.animator.SetFloat("Speed", speed);
            }
        }
    }
}

