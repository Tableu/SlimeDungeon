using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Type = Elements.Type;

namespace Controller.Player
{
    public class Character
    {
        private CharacterData _data;
        
        private float _speed;
        private Vector2 _maxVelocity;
        private Type _elementType;
        private CharacterAnimator _characterAnimator;
        private Attack _basicAttack;
        public CharacterData Data => _data;
        public float Health { get; set; }
        public float Speed => _speed;
        public Vector2 MaxVelocity => _maxVelocity;
        public Type ElementType => _elementType;
        public Attack BasicAttack => _basicAttack;

        public Character(CharacterData data)
        {
            _data = data;
            Health = data.Health;
            _speed = data.Speed;
            _maxVelocity = data.MaxVelocity;
            _elementType = data.ElementType;
        }

        public Character(CharacterData data, float health)
        {
            _data = data;
            Health = health;
            _speed = data.Speed;
            _maxVelocity = data.MaxVelocity;
            _elementType = data.ElementType;
        }

        public void Equip(GameObject model, PlayerController controller)
        {
            _characterAnimator = _data.AttachScript(model);
            _characterAnimator.Initialize(this, controller.PlayerInputActions);
            _basicAttack = _data.BasicAttack.CreateInstance(controller);
        }

        public void Drop()
        {
            if (_characterAnimator != null)
            {
                Object.Destroy(_characterAnimator);
            }

            if (_basicAttack != null)
            {
                _basicAttack.CleanUp();
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
            public SaveData(string character, float health)
            {
                Character = character;
                Health = health;
            }
            public float Health;
            [FormerlySerializedAs("Form")] public string Character;
        }
    }
}