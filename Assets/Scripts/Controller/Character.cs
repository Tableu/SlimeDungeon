using System;
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

        [SerializeField] internal CharacterData characterData;
        [SerializeField] internal Animator animator;
        [SerializeField] protected LayerMask enemyMask;
        internal Attack currentAttack;
        internal bool disableRotation = false;

        protected void Start()
        {
            Speed = characterData.Speed;
            Health = characterData.Health;
            Armor = characterData.Armor;
        }

        public void AlertObservers(string message)
        {
            if(currentAttack != null && Enum.TryParse(message, out Controller.AnimationState state))
                currentAttack.PassMessage(state);
        }

        public virtual void TakeDamage(float damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}