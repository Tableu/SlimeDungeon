using System;
using UnityEngine;

namespace Controller
{
    public abstract class Character : MonoBehaviour, IHealth
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
        [SerializeField] internal Animator animator;
        [SerializeField] protected LayerMask enemyMask;
        internal Attack currentAttack;
        internal bool disableRotation = false;
        
        public void AlertObservers(string message)
        {
            if(currentAttack != null && Enum.TryParse(message, out Controller.AnimationState state))
                currentAttack.PassMessage(state);
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            if (Health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}