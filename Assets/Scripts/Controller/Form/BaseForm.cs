using UnityEngine;
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
            
            playerController.playerInputActions.Movement.Pressed.canceled += MovementCanceled;
            playerController.playerInputActions.Movement.Pressed.started += MovementStarted;
        }

        public override void Drop()
        {
            _playerController.playerInputActions.Movement.Pressed.canceled -= MovementCanceled;
            _playerController.playerInputActions.Movement.Pressed.started -= MovementStarted;
        }
        
        public void OnAnimatorMove()
        {
            Vector3 position = _playerController.animator.rootPosition;
            _playerController.transform.position = position;
        }

        private void MovementCanceled(InputAction.CallbackContext context)
        {
            if (_playerController.animator != null)
            {
                _playerController.animator.SetFloat("Speed", 0);
            }
            _playerController.rigidbody.velocity = Vector3.zero;
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

