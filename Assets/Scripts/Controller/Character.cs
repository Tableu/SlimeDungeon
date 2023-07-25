using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Type = Elements.Type;

namespace Controller
{
    public abstract class Character : MonoBehaviour, IDamageable
    {
        public virtual float Speed
        {
            internal set;
            get;
        }
        public virtual float Health
        {
            internal set;
            get;
        }
        public float Armor
        {
            internal set;
            get;
        }

        public float Mana
        {
            internal set;
            get;
        }

        public virtual Type ElementType
        {
            internal set;
            get;
        }

        public virtual Vector3 SpellOffset
        {
            internal set;
            get;
        }

        internal abstract CharacterData CharacterData
        {
            get;
        }

        [SerializeField] internal LayerMask enemyMask;
        [SerializeField] internal new Rigidbody rigidbody;
        internal Attack currentAttack;
        internal List<Attack> attacks;
        internal bool disableRotation = false;
        

        public Action<Collision> CollisionEnter;

        protected void Start()
        {
            Speed = CharacterData.Speed;
            Health = CharacterData.Health;
            Armor = CharacterData.Armor;
            Mana = CharacterData.Mana;
            ElementType = CharacterData.ElementType;

            attacks = new List<Attack>();
            foreach (AttackData attackData in CharacterData.Attacks)
            {
                attackData.EquipAttack(this);
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

        public virtual void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType)
        {
            StartCoroutine(ApplyKnockback(knockback, hitStun));
            float typeMultiplier = GlobalReferences.Instance.TypeManager.GetTypeMultiplier(ElementType, attackType);
            Health -= damage*typeMultiplier;
            if (Health <= 0)
            {
                OnDeath();
            }
        }

        protected virtual void OnDeath()
        {
            Destroy(gameObject);
        }

        public abstract void Attack();

        public void OnCollisionEnter(Collision other)
        {
            CollisionEnter?.Invoke(other);
        }

        protected abstract IEnumerator ApplyKnockback(Vector3 knockback, float hitStun);
    }
}