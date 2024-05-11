using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Type = Elements.Type;

namespace Controller.Form
{
    public class Form
    {
        private FormData _data;
        
        private float _speed;
        private Vector2 _maxVelocity;
        private Type _elementType;
        private FormAnimator _formAnimator;
        private Attack _basicAttack;
        public FormData Data => _data;
        public float Health { get; set; }
        public float Speed => _speed;
        public Vector2 MaxVelocity => _maxVelocity;
        public Type ElementType => _elementType;

        public Form(FormData data)
        {
            _data = data;
            Health = data.Health;
            _speed = data.Speed;
            _maxVelocity = data.MaxVelocity;
            _elementType = data.ElementType;
        }

        public Form(FormData data, float health)
        {
            _data = data;
            Health = health;
            _speed = data.Speed;
            _maxVelocity = data.MaxVelocity;
            _elementType = data.ElementType;
        }

        public void Equip(GameObject model, PlayerController controller)
        {
            _formAnimator = _data.AttachScript(model);
            _formAnimator.Initialize(this, controller.PlayerInputActions);
            _basicAttack = _data.BasicAttack.CreateInstance(controller);
            _basicAttack.LinkInput(controller.PlayerInputActions.Other.BasicAttack);
        }

        public void Drop()
        {
            if (_formAnimator != null)
            {
                Object.Destroy(_formAnimator);
            }

            if (_basicAttack != null)
            {
                _basicAttack.CleanUp();
                _basicAttack.UnlinkInput();
                _basicAttack = null;
            }
        }

        public void CastBasicAttack()
        {
            _basicAttack?.Begin();
        }
        
        [Serializable]
        public struct SaveData
        {
            public SaveData(string form, float health)
            {
                Form = form;
                Health = health;
            }
            public float Health;
            public string Form;
        }
    }
}