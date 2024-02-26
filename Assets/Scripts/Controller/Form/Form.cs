using Elements;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Controller.Form
{
    public class Form
    {
        private FormData _data;
        
        private float _speed;
        private Vector2 _maxVelocity;
        private PlayerController _playerController;
        private Type _elementType;
        private FormAnimator _formAnimator;
        private Attack _basicAttack;
        public FormData Data => _data;
        public float Health { get; set; }
        public float Speed => _speed;
        public Vector2 MaxVelocity => _maxVelocity;
        public Type ElementType => _elementType;
        public PlayerController PlayerController => _playerController;

        public Form(FormData data, PlayerController playerController)
        {
            _data = data;
            _playerController = playerController;
            Health = data.Health;
            _speed = data.Speed;
            _maxVelocity = data.MaxVelocity;
            _elementType = data.ElementType;
        }

        public void Equip(GameObject model, InputAction basicAttackAction)
        {
            _formAnimator = _data.AttachScript(model);
            _formAnimator.Initialize(this);
            _basicAttack = _data.BasicAttack.EquipAttack(_playerController);
            _basicAttack.LinkInput(basicAttackAction);
        }

        public void Drop()
        {
            Object.Destroy(_formAnimator);
            _basicAttack.CleanUp();
            _basicAttack.UnlinkInput();
            _basicAttack = null;
        }
    }
}