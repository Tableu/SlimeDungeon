using System;
using Systems.Modifiers;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Type = Elements.Type;

namespace Controller.Player
{
    public class Character
    {
        private CharacterData _data;

        private Vector2 _maxVelocity;
        private Type _elementType;
        private CharacterAnimator _characterAnimator;
        private Attack _basicAttack;
        private Attack _spell;
        private ICharacterInfo _characterInfo;
        private EquipmentData _equipment;
        public CharacterData Data => _data;
        public float Health { get; private set; }
        public ModifiableStat Speed { get; }

        public Vector2 MaxVelocity => _maxVelocity;
        public Type ElementType => _elementType;
        public Attack BasicAttack => _basicAttack;
        public Attack Spell => _spell;
        public EquipmentData Equipment => _equipment;

        public Character(CharacterData data, ICharacterInfo characterInfo)
        {
            _data = data;
            Health = data.Health;
            Speed = new ModifiableStat(data.Speed);
            _maxVelocity = data.MaxVelocity;
            _elementType = data.ElementType;
            _characterInfo = characterInfo;
            _basicAttack = _data.BasicAttack.CreateInstance(characterInfo);
        }

        public Character(CharacterData data, ICharacterInfo characterInfo, float health, AttackData spell)
        {
            _data = data;
            Health = health;
            Speed = new ModifiableStat(data.Speed);
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

        public void ApplyDamage(float damage, Type attackType)
        {
            float typeMultiplier = GlobalReferences.Instance.TypeManager.GetTypeMultiplier(ElementType, attackType);
            if (_equipment != null)
            {
                foreach (EquipmentData.Effect buff in _equipment.Buffs)
                {
                    if (buff.Element.HasFlag(attackType) && buff.Type == EquipmentData.EffectType.Armor)
                        damage -= buff.Value;
                }
            }

            Health -= damage*typeMultiplier;
        }

        public void Equip(GameObject model, PlayerInputActions playerInputActions)
        {
            _characterAnimator = model.GetComponent<CharacterAnimator>();
            _characterAnimator.Initialize(this, playerInputActions);
            if(_equipment != null)
                _characterAnimator.RefreshEquipment(_equipment);
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
                oldSpell = UnEquipSpell();

            _spell = attackData.CreateInstance(_characterInfo);
            return oldSpell;
        }
        
        public AttackData UnEquipSpell()
        {
            if (_spell == null)
                return null;
            AttackData oldSpell = _spell.Data;
            _spell.UnlinkInput();
            _spell.CleanUp();
            _spell = null;
            return oldSpell;
        }

        public EquipmentData AddEquipment(EquipmentData equipmentData)
        {
            EquipmentData oldEquipment = null;
            if (equipmentData == null)
                return null;
            bool hasEquipment = _equipment != null;
            if (hasEquipment)
                oldEquipment = RemoveEquipment();
            
            _equipment = equipmentData;
            equipmentData.Equip(this);
            if(_characterAnimator != null)
                _characterAnimator.RefreshEquipment(_equipment);
            return oldEquipment;
        }

        public EquipmentData RemoveEquipment()
        {
            if (_equipment == null)
                return null;
            EquipmentData oldEquipment = _equipment;
            _equipment.Drop(this);
            _equipment = null;
            if(_characterAnimator != null)
                _characterAnimator.RefreshEquipment(_equipment);
            return oldEquipment;
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