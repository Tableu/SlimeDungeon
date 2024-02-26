using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Controller.Form
{
    //todo fix fire form slider and attack temperature increase/decrease
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
            //var sliderObject = Instantiate(_data.Slider, GlobalReferences.Instance.Canvas.gameObject.transform);
            //_statBarSlider = sliderObject.GetComponent<Slider>();
            //_statBarSlider.maxValue = _data.MaxTemperature;
            
            /*foreach (Attack attack in form.PlayerController.Attacks)
            {
                attack.OnBegin += IncreaseTemperature;
            }*/
            if (form.PlayerController.PlayerInputActions.Movement.Pressed.inProgress)
            {
                if (animator != null)
                {
                    animator.SetFloat("Speed", form.Speed);
                }
            }
            form.PlayerController.PlayerInputActions.Movement.Pressed.canceled += MovementCanceled;
            form.PlayerController.PlayerInputActions.Movement.Pressed.started += MovementStarted;
            animator = GetComponent<Animator>();
        }

        private void OnDestroy()
        {
            //Destroy(_statBarSlider.gameObject);
            /*foreach (Attack attack in form.PlayerController.Attacks)
            {
                attack.OnBegin -= IncreaseTemperature;
            }*/
            form.PlayerController.PlayerInputActions.Movement.Pressed.canceled -= MovementCanceled;
            form.PlayerController.PlayerInputActions.Movement.Pressed.started -= MovementStarted;
        }
        
        public void OnAnimatorMove()
        {
            //Vector3 position = animator.rootPosition;
            //form.PlayerController.transform.position = position;
        }

        private void FixedUpdate()
        {
            DecreaseTemperature();

            if (_statBarSlider != null)
            {
                _statBarSlider.value = Temperature;
            }

            if (Temperature > _data.MaxTemperature / 2)
            {
                form.PlayerController.SetMultipliers(2, 2, 1);
            }
            else
            {
                form.PlayerController.SetMultipliers(1, 1, 1);
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