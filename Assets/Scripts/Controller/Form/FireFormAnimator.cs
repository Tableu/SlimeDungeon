using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Controller.Form
{
    public class FireFormAnimator : FormAnimator
    {
        public float Temperature { get; private set; }

        private FireFormData _data;
        private Slider _statBarSlider;

        public override void Initialize(Form form)
        {
            this.form = form;
            animator = GetComponent<Animator>();
            _data = form.Data as FireFormData;
            var sliderObject = Instantiate(_data.Slider, GlobalReferences.Instance.Canvas.gameObject.transform);
            _statBarSlider = sliderObject.GetComponent<Slider>();
            _statBarSlider.maxValue = _data.MaxTemperature;
            
            foreach (Attack attack in form.PlayerController.Attacks)
            {
                attack.OnBegin += IncreaseTemperature;
            }
            form.PlayerController.PlayerInputActions.Movement.Pressed.canceled += MovementCanceled;
            form.PlayerController.PlayerInputActions.Movement.Pressed.started += MovementStarted;
            animator = GetComponent<Animator>();
        }

        private void OnDestroy()
        {
            Destroy(_statBarSlider.gameObject);
            foreach (Attack attack in form.PlayerController.Attacks)
            {
                attack.OnBegin -= IncreaseTemperature;
            }
            form.PlayerController.PlayerInputActions.Movement.Pressed.canceled -= MovementCanceled;
            form.PlayerController.PlayerInputActions.Movement.Pressed.started -= MovementStarted;
        }
        
        public void OnAnimatorMove()
        {
            //Vector3 position = animator.rootPosition;
            //form.PlayerController.transform.position = position;
        }
        
        public void AlertObservers(string message)
        {
            if(form.PlayerController.CurrentAttack != null && Enum.TryParse(message, out Controller.AnimationState state))
                form.PlayerController.CurrentAttack.PassMessage(state);
        }

        private void FixedUpdate()
        {
            DecreaseTemperature();
            
            if (_statBarSlider != null)
            {
                _statBarSlider.value = Temperature;
                if (Temperature > _data.MaxTemperature / 2)
                {
                    form.PlayerController.sizeMultiplier = 2;
                    form.PlayerController.damageMultiplier = 2;
                }
                else
                {
                    form.PlayerController.sizeMultiplier = 1;
                    form.PlayerController.damageMultiplier = 1;
                }
            }
        }

        private void IncreaseTemperature(Attack attack)
        {
            Temperature += _data.IncreaseRate;
            if (Temperature > _data.MaxTemperature)
            {
                Temperature = _data.MaxTemperature;
            }
        }

        private void DecreaseTemperature()
        {
            if (Temperature > 0)
            {
                Temperature -= _data.DecreaseRate;
            }
        }
        
        private void MovementCanceled(InputAction.CallbackContext context)
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0);
            }
            form.PlayerController.rigidbody.velocity = Vector3.zero;
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