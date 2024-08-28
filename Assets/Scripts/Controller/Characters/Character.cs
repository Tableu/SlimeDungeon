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
        private Attack _spell;
        private ICharacterInfo _characterInfo;
        public CharacterData Data => _data;
        public float Health { get; set; }
        public float Speed => _speed;
        public Vector2 MaxVelocity => _maxVelocity;
        public Type ElementType => _elementType;
        public Attack BasicAttack => _basicAttack;
        public Attack Spell => _spell;

        public Character(CharacterData data, ICharacterInfo characterInfo)
        {
            _data = data;
            Health = data.Health;
            _speed = data.Speed;
            _maxVelocity = data.MaxVelocity;
            _elementType = data.ElementType;
            _characterInfo = characterInfo;
            _basicAttack = _data.BasicAttack.CreateInstance(characterInfo);
        }

        public Character(CharacterData data, ICharacterInfo characterInfo, float health, AttackData spell)
        {
            _data = data;
            Health = health;
            _speed = data.Speed;
            _maxVelocity = data.MaxVelocity;
            _elementType = data.ElementType;
            _characterInfo = characterInfo;
            _basicAttack = _data.BasicAttack.CreateInstance(characterInfo);
            if(spell != null)
                _spell = spell.CreateInstance(characterInfo);
        }

        ~Character()
        {
            if (_basicAttack != null)
            {
                _basicAttack.CleanUp();
            }

            if (_spell != null)
            {
                _spell.CleanUp();
            }
        }

        public void Equip(GameObject model, PlayerInputActions playerInputActions)
        {
            _characterAnimator = _data.AttachScript(model);
            _characterAnimator.Initialize(this, playerInputActions);
        }

        public void Drop()
        {
            if (_characterAnimator != null)
            {
                Object.Destroy(_characterAnimator);
            }
        }

        public void CastBasicAttack()
        {
            _basicAttack?.Begin();
        }

        public void CastSpell()
        {
            _spell?.Begin();
        }
        
        public AttackData EquipSpell(AttackData attackData)
        {
            AttackData oldSpell = null;
            if (attackData == null)
                return null;
            bool hasSpell = _spell != null;
            if (hasSpell && _spell.OnCooldown)
                return null;
            if (hasSpell)
            {
                oldSpell = _spell.Data;
                UnEquipSpell();
            }
            
            _spell = attackData.CreateInstance(_characterInfo);
            return oldSpell;
        }
        
        private void UnEquipSpell()
        {
            if (_spell == null)
                return;
            _spell.UnlinkInput();
            _spell.CleanUp();
            _spell = null;
        }
        
        [Serializable]
        public struct SaveData
        {
            public SaveData(string character, float health, string spell)
            {
                Character = character;
                Health = health;
                Spell = spell;
            }
            public float Health;
            public string Spell;
            [FormerlySerializedAs("Form")] public string Character;
        }
    }
}