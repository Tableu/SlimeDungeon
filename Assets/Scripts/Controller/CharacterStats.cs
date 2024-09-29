using System;
using Controller.Player;
using Newtonsoft.Json;
using Systems.Modifiers;
using UnityEngine;
using Type = Elements.Type;

namespace Controller
{
    [Serializable]
    public class CharacterStats
    {
        public ModifiableStat Speed
        {
            get;
        }
        public ModifiableStat Armor
        {
            get;
        }
        public ModifiableStat Damage
        {
            get;
        }
        public float Health
        {
            get;
            private set;
        }

        public Type ElementType
        {
            get;
        }

        public Vector3 SpellOffset
        {
            get;
        }

        public LayerMask EnemyMask
        {
            get;
        }
        //For saving with JSON
        [JsonConstructor]
        private CharacterStats(ModifiableStat speed, ModifiableStat armor, ModifiableStat damage, 
            float health, Type elementType, Vector3 spellOffset, int enemyMask)
        {
            Speed = speed;
            Armor = armor;
            Damage = damage;
            Health = health;
            ElementType = elementType;
            SpellOffset = spellOffset;
            EnemyMask = enemyMask;
        }
        //New Player Character
        public CharacterStats(PlayerCharacterData data)
        {
            Health = data.Health;
            Speed = new ModifiableStat(data.Speed);
            Damage = new ModifiableStat(1.0f);
            Armor = new ModifiableStat(0);
            ElementType = data.ElementType;
            SpellOffset = data.SpellOffset;
            EnemyMask = LayerMask.GetMask("Enemy");
        }
        //Enemy Character
        public CharacterStats(EnemyData data)
        {
            Speed = new ModifiableStat(data.Speed);
            Armor = new ModifiableStat(0);
            Damage = new ModifiableStat(1f);
            Health = data.Health;
            EnemyMask = LayerMask.GetMask("Player");
            SpellOffset = data.SpellOffset;
        }

        public void ApplyDamage(float damage)
        {
            Health -= damage;
        }
    }
}