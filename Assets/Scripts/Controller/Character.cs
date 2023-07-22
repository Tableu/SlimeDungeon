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

        [SerializeField] internal CharacterData characterData;
        [SerializeField] internal Animator animator;
        [SerializeField] internal LayerMask enemyMask;
        [SerializeField] internal new Rigidbody rigidbody;
        internal Attack currentAttack;
        internal List<Attack> attacks;
        internal bool disableRotation = false;
        

        public Action<Collision> CollisionEnter;

        protected void Start()
        {
            Speed = characterData.Speed;
            Health = characterData.Health;
            Armor = characterData.Armor;
            Mana = characterData.Mana;
            ElementType = characterData.ElementType;

            attacks = new List<Attack>();
            foreach (AttackData attackData in characterData.Attacks)
            {
                attackData.EquipAttack(this);
            }
        }

        protected void FixedUpdate()
        {
            Mana += characterData.ManaRegen;
            if (Mana > characterData.Mana)
            {
                Mana = characterData.Mana;
            }
        }

        public void AlertObservers(string message)
        {
            if(currentAttack != null && Enum.TryParse(message, out Controller.AnimationState state))
                currentAttack.PassMessage(state);
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

        public void OnCollisionEnter(Collision other)
        {
            CollisionEnter?.Invoke(other);
        }

        protected abstract IEnumerator ApplyKnockback(Vector3 knockback, float hitStun);
    }
}