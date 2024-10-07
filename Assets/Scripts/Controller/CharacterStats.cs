using System;
using Controller.Player;
using Newtonsoft.Json;
using Systems.Modifiers;
using UnityEngine;
using Type = Elements.Type;

namespace Controller
{
    public enum Attribute
    {
        Health,
        Attack,
        Defense
    }
    
    [Serializable]
    public class CharacterStats
    {
        public ModifiableStat Speed { get; }
        public ModifiableStat Defense { get; }
        public ModifiableStat Attack { get; }
        public ModifiableStat MaxHealth { get; }
        public float Health { get; private set; }
        public Type ElementType { get; }
        public Vector3 SpellOffset { get; }
        public LayerMask EnemyMask { get; }
        public int SkillPoints { get; private set; }
        //For saving with JSON
        [JsonConstructor]
        private CharacterStats(ModifiableStat speed, ModifiableStat defense, ModifiableStat attack, ModifiableStat maxHealth,
            float health, Type elementType, Vector3 spellOffset, int enemyMask)
        {
            Speed = speed;
            Defense = defense;
            Attack = attack;
            MaxHealth = maxHealth;
            Health = health;
            ElementType = elementType;
            SpellOffset = spellOffset;
            EnemyMask = enemyMask;
        }
        //New Player Character
        public CharacterStats(PlayerCharacterData data)
        {
            MaxHealth = new ModifiableStat(data.Health);
            Health = data.Health;
            Speed = new ModifiableStat(data.Speed);
            Attack = new ModifiableStat(data.Attack);
            Defense = new ModifiableStat(data.Defense);
            ElementType = data.ElementType;
            SpellOffset = data.SpellOffset;
            EnemyMask = LayerMask.GetMask("Enemy");
        }
        //Enemy Character
        public CharacterStats(EnemyData data)
        {
            MaxHealth = new ModifiableStat(data.Health);
            Speed = new ModifiableStat(data.Speed);
            Defense = new ModifiableStat(data.Defense);
            Attack = new ModifiableStat(data.Attack);
            Health = data.Health;
            EnemyMask = LayerMask.GetMask("Player");
            ElementType = data.ElementType;
            SpellOffset = data.SpellOffset;
        }

        public void ApplyDamage(float damage)
        {
            Health -= damage;
        }

        public void Heal(int amount)
        {
            Health += amount;
            if (Health > MaxHealth)
                Health = MaxHealth.CurrentValue;
        }

        public void AddSkillPoint()
        {
            SkillPoints++;
        }

        public void UpgradeAttribute(Attribute attribute)
        {
            if (SkillPoints <= 0)
                return;
            
            SkillPoints--;
            switch (attribute)
            {
                case Attribute.Health:
                    MaxHealth.BaseModifier += 10;
                    Heal(10);
                    break;
                case Attribute.Attack:
                    Attack.BaseModifier++;
                    break;
                case Attribute.Defense:
                    Defense.BaseModifier++;
                    break;
            }
            
        }
    }
}