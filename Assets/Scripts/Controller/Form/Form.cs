using Elements;
using UnityEngine;

namespace Controller.Form
{
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
        private Attack _basicAttack;
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
            _basicAttack = _data.BasicAttack.EquipAttack(_playerController);
            _playerController.LinkInput(_playerController.PlayerInputActions.Other.BasicAttack, _basicAttack);
        }

        public void Drop()
        {
            Object.Destroy(_formAnimator);
            _playerController.UnlinkInput(_playerController.PlayerInputActions.Other.BasicAttack, _basicAttack);
            _basicAttack.OnEnd?.Invoke(_basicAttack);
            _basicAttack.CleanUp();
            _basicAttack = null;
        }

        public void Attack()
        {
            if (_formAnimator != null)
            {
                _formAnimator.Attack();
            }
        }
    }
}