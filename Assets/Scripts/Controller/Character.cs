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
        public float StunMeter
        {
            internal set;
            get;
        }

        public float StunPercent => (StunMeter / CharacterData.StunResist)*100;

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

        public abstract CharacterData CharacterData
        {
            get;
        }

        [SerializeField] internal LayerMask enemyMask;
        [SerializeField] internal new Rigidbody rigidbody;
        protected Attack currentAttack;
        protected List<Attack> attacks;
        internal bool disableRotation = false;
        internal float damageMultiplier = 1;
        internal float speedMultiplier = 1;
        internal float sizeMultiplier = 1;

        public Attack CurrentAttack => currentAttack;
        public List<Attack> Attacks => attacks;
        public Action<Collision> CollisionEnter;

        protected void Start()
        {
            Speed = CharacterData.Speed;
            Health = CharacterData.Health;
            StunMeter = 0;
            Mana = CharacterData.Mana;
            ElementType = CharacterData.ElementType;

            attacks = new List<Attack>();
            
            foreach (AttackData attackData in CharacterData.Attacks)
            {
                var attack = attackData.EquipAttack(this);
                attack.OnBegin += OnAttackBegin;
                attack.OnEnd += OnAttackEnd;
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

        public virtual void TakeDamage(float damage, Vector3 knockback, float hitStun, Elements.Type attackType)
        {
            StartCoroutine(ApplyKnockback(knockback, hitStun));
            float typeMultiplier = GlobalReferences.Instance.TypeManager.GetTypeMultiplier(ElementType, attackType);
            Health -= damage*typeMultiplier;
            if (StunMeter < CharacterData.StunResist)
            {
                StunMeter += damage * typeMultiplier;
                if (StunMeter >= CharacterData.StunResist)
                {
                    StunMeter = CharacterData.StunResist;

                }
            }
            else if(typeMultiplier > 1)
            {
                //todo stun implementation
                StunMeter = 0;
            }

            if (Health <= 0)
            {
                HandleDeath();
            }
        }

        protected virtual void HandleDeath()
        {
            Destroy(gameObject);
        }

        protected abstract void OnAttackBegin(Attack attack);

        public void OnCollisionEnter(Collision other)
        {
            CollisionEnter?.Invoke(other);
        }

        protected void OnAttackEnd(Attack attack)
        {
            if (currentAttack == attack)
            {
                currentAttack = null;
            }
        }

        protected abstract IEnumerator ApplyKnockback(Vector3 knockback, float hitStun);
    }
}