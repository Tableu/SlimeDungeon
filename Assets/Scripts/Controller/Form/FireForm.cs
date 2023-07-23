using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Controller.Form
{
    public class FireForm : Form
    {
        public float Temperature { get; private set; }

        internal new FireFormData data;
        private Slider _slider;
        private PlayerController _playerController;

        public override void Equip(PlayerController playerController)
        {
            health = data.Health;
            speed = data.Speed;
            elementType = data.ElementType;
            var sliderObject = Instantiate(data.Slider, GlobalReferences.Instance.Canvas.gameObject.transform);
            _slider = sliderObject.GetComponent<Slider>();
            _slider.maxValue = data.MaxTemperature;
            _playerController = playerController;
            foreach (Attack attack in playerController.attacks)
            {
                attack.OnSpellCast += IncreaseTemperature;
            }
            playerController.playerInputActions.Movement.Pressed.canceled += MovementCanceled;
            playerController.playerInputActions.Movement.Pressed.started += MovementStarted;
            animator = GetComponent<Animator>();
        }

        public override void Drop()
        {
            Destroy(_slider.gameObject);
            foreach (Attack attack in _playerController.attacks)
            {
                attack.OnSpellCast -= IncreaseTemperature;
            }
            _playerController.playerInputActions.Movement.Pressed.canceled -= MovementCanceled;
            _playerController.playerInputActions.Movement.Pressed.started -= MovementStarted;
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

        private void FixedUpdate()
        {
            DecreaseTemperature();
            if (_slider != null)
            {
                _slider.value = Temperature;
                if (Temperature > data.MaxTemperature / 2)
                {
                    sizeMultiplier = 2;
                    damageMultiplier = 2;
                }
                else
                {
                    sizeMultiplier = 1;
                    damageMultiplier = 1;
                }
            }
        }

        public override void Attack()
        {
            animator.SetTrigger("Attack");
        }

        private void IncreaseTemperature()
        {
            Temperature += data.IncreaseRate;
            if (Temperature > data.MaxTemperature)
            {
                Temperature = data.MaxTemperature;
            }
        }

        private void DecreaseTemperature()
        {
            if (Temperature > 0)
            {
                Temperature -= data.DecreaseRate;
            }
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