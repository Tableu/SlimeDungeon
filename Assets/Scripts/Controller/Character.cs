using System;
using System.Collections;
using System.Collections.Generic;
using Systems.Modifiers;
using UnityEngine;
using Type = Elements.Type;

namespace Controller
{
    public abstract class Character : MonoBehaviour, IDamageable
    {
        public ModifiableStat Speed
        {
            protected set;
            get;
        }
        public virtual float Health
        {
            protected set;
            get;
        }

        public float Mana
        {
            protected set;
            get;
        }

        public virtual Type ElementType
        {
            protected set;
            get;
        }

        public virtual Vector3 SpellOffset
        {
            protected set;
            get;
        }

        public abstract CharacterData CharacterData
        {
            get;
        }
        
        //not currently needed
        [SerializeField] protected LayerMask enemyMask;
        [SerializeField] protected new Rigidbody rigidbody;
        protected List<Attack> attacks;

        protected bool disableRotation = false;

        //todo refactor spell multipliers
        public float DamageMultiplier
        {
            protected set;
            get;
        } = 1;

        public float SpeedMultiplier
        {
            protected set;
            get;
        } = 1;

        public float SizeMultiplier
        {
            protected set;
            get;
        } = 1;
        public LayerMask EnemyMask => enemyMask;
        public Action OnDeath;

        protected void Start()
        {
            Speed = new ModifiableStat(CharacterData.Speed);
            Health = CharacterData.Health;
            Mana = CharacterData.Mana;
            ElementType = CharacterData.ElementType;

            attacks = new List<Attack>();
            
            foreach (AttackData attackData in CharacterData.Attacks)
            {
                var attack = attackData.CreateInstance(this);
                attacks.Add(attack);
            }
        }

        protected void FixedUpdate()
        {
            Mana += CharacterData.ManaRegen;
            if (Mana > CharacterData.Mana)
            {
                Mana = CharacterData.Mana;
            }
        }

        public abstract void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType);

        protected virtual void HandleDeath()
        {
            Destroy(gameObject);
        }

        public void ApplyManaCost(float manaCost)
        {
            Mana -= manaCost;
        }

        public void SetMultipliers(float size, float damage, float speed)
        {
            SizeMultiplier = size;
            DamageMultiplier = damage;
            SpeedMultiplier = speed;
        }

        public void DisableRotation()
        {
            disableRotation = true;
        }

        public void EnableRotation()
        {
            disableRotation = false;
        }

        protected abstract IEnumerator HandleKnockback(Vector3 knockback, float hitStun, float typeMultiplier);
    }
}