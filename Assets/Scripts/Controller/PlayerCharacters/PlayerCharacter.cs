using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Type = Elements.Type;

namespace Controller.Player
{
    public class PlayerCharacter
    {
        private PlayerCharacterData _data;

        private Vector2 _maxVelocity;
        private PlayerCharacterAnimator _playerCharacterAnimator;
        private Attack _basicAttack;
        private Attack _spell;
        private EquipmentData _equipment;
        private Transform _transform;
        public PlayerCharacterData Data => _data;

        public CharacterStats Stats
        {
            get;
        }

        public Vector2 MaxVelocity => _maxVelocity;
        public Attack BasicAttack => _basicAttack;
        public Attack Spell => _spell;
        public EquipmentData Equipment => _equipment;

        public PlayerCharacter(PlayerCharacterData data, Transform transform)
        {
            _transform = transform;
            _data = data;
            
            _maxVelocity = data.MaxVelocity;
            Stats = new CharacterStats(_data);
            _basicAttack = _data.BasicAttack.CreateInstance(Stats, transform);
        }

        public PlayerCharacter(PlayerCharacterData data, float health, AttackData spell, Transform transform)
        {
            _transform = transform;
            _data = data;
            Stats = new CharacterStats(_data, health);
            _maxVelocity = data.MaxVelocity;
            _basicAttack = _data.BasicAttack.CreateInstance(Stats, transform);
            if(spell != null)
                _spell = spell.CreateInstance(Stats, transform);
        }

        ~PlayerCharacter()
        {
            _basicAttack?.CleanUp();
            _spell?.CleanUp();
        }

        public void ApplyDamage(float damage, Type attackType)
        {
            float typeMultiplier = GlobalReferences.Instance.TypeManager.GetTypeMultiplier(Stats.ElementType, attackType);
            if (_equipment != null)
            {
                foreach (EquipmentData.Effect buff in _equipment.Buffs)
                {
                    if (buff.Element.HasFlag(attackType) && buff.Type == EquipmentData.EffectType.Armor)
                        damage -= buff.Value;
                }
            }

            Stats.ApplyDamage(damage*typeMultiplier);
        }

        public void Equip(GameObject model, PlayerInputActions playerInputActions)
        {
            _playerCharacterAnimator = model.GetComponent<PlayerCharacterAnimator>();
            _playerCharacterAnimator.Initialize(this, playerInputActions);
            if(_equipment != null)
                _playerCharacterAnimator.RefreshHat(_equipment);
        }

        public void Drop()
        {
            if (_playerCharacterAnimator != null)
            {
                Object.Destroy(_playerCharacterAnimator);
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

            _spell = attackData.CreateInstance(Stats, _transform);
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
            if(_playerCharacterAnimator != null)
                _playerCharacterAnimator.RefreshHat(_equipment);
            return oldEquipment;
        }

        public EquipmentData RemoveEquipment()
        {
            if (_equipment == null)
                return null;
            EquipmentData oldEquipment = _equipment;
            _equipment.Drop(this);
            _equipment = null;
            if(_playerCharacterAnimator != null)
                _playerCharacterAnimator.RefreshHat(_equipment);
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