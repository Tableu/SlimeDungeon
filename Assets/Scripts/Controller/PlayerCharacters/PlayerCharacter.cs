using System;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Type = Elements.Type;

namespace Controller.Player
{
    public class PlayerCharacter
    {
        private PlayerCharacterAnimator _playerCharacterAnimator;
        private Transform _transform;
        
        public PlayerCharacterData Data { get; }
        public CharacterStats Stats { get; }
        public Vector2 MaxVelocity { get; }
        public Attack BasicAttack { get; }
        public Attack Spell { get; private set; }
        public EquipmentData Equipment { get; private set; }
        public ExperienceSystem ExperienceSystem { get; }
        public int SkillPoints { get; private set; }
        //Initial loading constructor
        public PlayerCharacter(PlayerCharacterData data, Transform transform)
        {
            _transform = transform;
            Data = data;
            MaxVelocity = data.MaxVelocity;
            Stats = new CharacterStats(Data);
            BasicAttack = Data.BasicAttack.CreateInstance(Stats, transform);
            ExperienceSystem = new ExperienceSystem(0, data);
            ExperienceSystem.OnLevelUp += OnLevelUp;
        }
        //Constructor for loading player
        public PlayerCharacter(PlayerCharacterData data, CharacterStats stats, EquipmentData equipment, AttackData spell, int level, Transform transform)
        {
            _transform = transform;
            Data = data;
            Stats = stats;
            MaxVelocity = data.MaxVelocity;
            BasicAttack = Data.BasicAttack.CreateInstance(Stats, transform);
            Equipment = equipment;
            if(spell != null)
                Spell = spell.CreateInstance(Stats, transform);
            ExperienceSystem = new ExperienceSystem(level, data);
            ExperienceSystem.OnLevelUp += OnLevelUp;
        }

        ~PlayerCharacter()
        {
            BasicAttack?.CleanUp();
            Spell?.CleanUp();
            if(ExperienceSystem != null)
                ExperienceSystem.OnLevelUp -= OnLevelUp;
        }

        public void ApplyDamage(float damage, float attackStat, Type attackType)
        {
            float typeMultiplier = GlobalReferences.Instance.TypeManager.GetTypeMultiplier(Stats.ElementType, attackType);
            float statMultiplier = Stats.Defense > 0 ? attackStat / Stats.Defense : 1f;
            if (Equipment != null)
            {
                foreach (EquipmentData.Effect buff in Equipment.Buffs)
                {
                    if (buff.Element.HasFlag(attackType) && buff.Type == EquipmentData.EffectType.Armor)
                        damage -= buff.Value;
                }
            }

            Stats.ApplyDamage(damage*typeMultiplier*statMultiplier);
        }

        public void Equip(GameObject model, PlayerInputActions playerInputActions)
        {
            _playerCharacterAnimator = model.GetComponent<PlayerCharacterAnimator>();
            _playerCharacterAnimator.Initialize(this, playerInputActions);
            if(Equipment != null)
                _playerCharacterAnimator.RefreshHat(Equipment);
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
            BasicAttack?.Begin();
        }

        public void CastSpell()
        {
            Spell?.Begin();
        }
        
        public AttackData EquipSpell(AttackData attackData)
        {
            AttackData oldSpell = null;
            if (attackData == null)
                return null;
            bool hasSpell = Spell != null;
            if (hasSpell && Spell.OnCooldown)
                return null;
            if (hasSpell)
                oldSpell = UnEquipSpell();

            Spell = attackData.CreateInstance(Stats, _transform);
            return oldSpell;
        }
        
        public AttackData UnEquipSpell()
        {
            if (Spell == null)
                return null;
            AttackData oldSpell = Spell.Data;
            Spell.UnlinkInput();
            Spell.CleanUp();
            Spell = null;
            return oldSpell;
        }

        public EquipmentData AddEquipment(EquipmentData equipmentData)
        {
            EquipmentData oldEquipment = null;
            if (equipmentData == null)
                return null;
            bool hasEquipment = Equipment != null;
            if (hasEquipment)
                oldEquipment = RemoveEquipment();
            
            Equipment = equipmentData;
            equipmentData.Equip(this);
            if(_playerCharacterAnimator != null)
                _playerCharacterAnimator.RefreshHat(Equipment);
            return oldEquipment;
        }

        public EquipmentData RemoveEquipment()
        {
            if (Equipment == null)
                return null;
            EquipmentData oldEquipment = Equipment;
            Equipment.Drop(this);
            Equipment = null;
            if(_playerCharacterAnimator != null)
                _playerCharacterAnimator.RefreshHat(Equipment);
            return oldEquipment;
        }

        public void RemoveSkillPoint()
        {
            SkillPoints--;
            if (SkillPoints < 0)
                SkillPoints = 0;
        }

        private void OnLevelUp()
        {
            SkillPoints++;
        }
        
        [Serializable]
        public struct SaveData
        {
            public SaveData(string character, CharacterStats characterStats, string equipment, string spell, int level)
            {
                Character = character;
                Stats = characterStats;
                Spell = spell;
                Level = level;
                Equipment = equipment;
            }

            public CharacterStats Stats;
            public string Spell;
            public int Level;
            public string Equipment;
            [FormerlySerializedAs("Form")] public string Character;
        }
    }
    /// <summary>
    /// A class that manages a level system abstractly defined exp formulas.
    /// It does not control the formula which calculates the required exp for each level.
    /// </summary>
    /// <remarks>
    /// The level argument starts at zero.
    /// An experience requirement of zero indicates the max level. 
    /// </remarks>
    public class ExperienceSystem
    {
        public int Level
        {
            get;
            private set;
        }

        public float ExperiencePercentage
        {
            get
            {
                if (_data == null)
                    return 0;
                if (_data.GetExperienceRequirement(Level) == 0)
                    return 0;
                return _experience / (float)_data.GetExperienceRequirement(Level);
            }
        }

        public Action OnLevelUp;

        private ILevelData _data;
        private int _experience;

        public ExperienceSystem(int level, ILevelData data)
        {
            _data = data;
            Level = level;
        }

        public void AddExperience(int exp)
        {
            int requiredExperience = _data.GetExperienceRequirement(Level);
            _experience += exp;
            if (requiredExperience == 0)
                return;
            if (_experience >= requiredExperience)
            {
                Level++;
                _experience -= requiredExperience;
                OnLevelUp?.Invoke();
            }
        }
    }
    /// <summary>
    /// An interface that ExperienceSystem uses to access the experience requirement for each level.
    /// </summary>
    /// <remarks>
    /// The level argument starts at zero.
    /// GetExperienceRequirement should return zero if the max level has been reached.
    /// </remarks>
    public interface ILevelData
    {
        public int GetExperienceRequirement(int level);
    }
}