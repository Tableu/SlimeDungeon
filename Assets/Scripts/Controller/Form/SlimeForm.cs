using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.Form
{
    public class SlimeForm : Form
    {
        private PlayerController _playerController;
        public override void Equip(PlayerController playerController)
        {
            health = data.Health;
            speed = data.Speed;
            elementType = data.ElementType;
            _playerController = playerController;
            animator = GetComponent<Animator>();
            
            playerController.PlayerInputActions.Movement.Pressed.canceled += MovementCanceled;
            playerController.PlayerInputActions.Movement.Pressed.started += MovementStarted;
        }

        public override void Drop()
        {
            _playerController.PlayerInputActions.Movement.Pressed.canceled -= MovementCanceled;
            _playerController.PlayerInputActions.Movement.Pressed.started -= MovementStarted;
        }
        
        public void OnAnimatorMove()
        {
            Vector3 position = animator.rootPosition;
            _playerController.transform.position = position;
        }
        public void AlertObservers(string message)
        {
            if(_playerController.currentAttack != null && Enum.TryParse(message, out Controller.AnimationState state))
                _playerController.currentAttack.PassMessage(state);
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
            _playerController.rigidbody.velocity = Vector3.zero;
        }

        private void MovementStarted(InputAction.CallbackContext context)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", speed);
            }
        }
    }
}

