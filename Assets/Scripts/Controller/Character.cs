using System;
using System.Collections;
using UnityEngine;

namespace Controller
{
    public abstract class Character : MonoBehaviour, IDamageable
    {
        public float Speed
        {
            internal set;
            get;
        }
        public float Health
        {
            internal set;
            get;
        }
        public float Armor
        {
            internal set;
            get;
        }

        public float HitStun
        {
            internal set;
            get;
        }

        [SerializeField] internal CharacterData characterData;
        [SerializeField] internal Animator animator;
        [SerializeField] protected LayerMask enemyMask;
        [SerializeField] protected new Rigidbody rigidbody;
        internal Attack currentAttack;
        internal bool disableRotation = false;

        protected void Start()
        {
            Speed = characterData.Speed;
            Health = characterData.Health;
            Armor = characterData.Armor;
            HitStun = characterData.HitStun;
        }

        public void AlertObservers(string message)
        {
            if(currentAttack != null && Enum.TryParse(message, out Controller.AnimationState state))
                currentAttack.PassMessage(state);
        }

        public virtual void TakeDamage(float damage, Vector3 knockback)
        {
            StartCoroutine(ApplyKnockback(knockback));
            Health -= damage;
            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }

        protected abstract IEnumerator ApplyKnockback(Vector3 knockback);
    }
}