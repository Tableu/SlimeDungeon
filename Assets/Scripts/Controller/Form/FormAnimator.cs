using Elements;
using UnityEngine;

namespace Controller.Form
{
    public abstract class FormAnimator : MonoBehaviour
    {
        protected Form form;
        protected Animator animator;
        public abstract void Initialize(Form form);
        public abstract void Attack();
    }

    public class Form
    {
        private FormData _data;
        //todo move multiplier logic to new feature
        private float _damageMultiplier = 1;
        private float _sizeMultiplier = 1;
        private float _speedMultiplier = 1;
        private float _speed;
        private PlayerController _playerController;
        private Type _elementType;
        private FormAnimator _formAnimator;
        public FormData Data => _data;
        public float Health { get; set; }
        public float DamageMultiplier => _damageMultiplier;
        public float SizeMultiplier => _sizeMultiplier;
        public float SpeedMultiplier => _speedMultiplier;
        public float Speed => _speed;
        public Type ElementType => _elementType;
        public PlayerController PlayerController => _playerController;

        public Form(FormData data, PlayerController playerController)
        {
            _data = data;
            _playerController = playerController;
            Health = data.Health;
            _speed = data.Speed;
            _elementType = data.ElementType;
        }

        public void Equip(GameObject model)
        {
            _formAnimator = _data.AttachScript(model);
            _formAnimator.Initialize(this);
        }

        public void Drop()
        {
            Object.Destroy(_formAnimator);
        }

        public void Attack()
        {
            if (_formAnimator != null)
            {
                _formAnimator.Attack();
            }
        }
    }
    
    public enum Forms
    {
        FireForm
    }
}