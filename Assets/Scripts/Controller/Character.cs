using System;
using UnityEngine;

namespace Controller
{
    public abstract class Character : MonoBehaviour
    {
        public float Speed;
        public float Health;
        public float Armor;
        [SerializeField] internal Animator animator;
        internal Attack currentAttack;
        
        public void AlertObservers(string message)
        {
            if(currentAttack != null && Enum.TryParse(message, out Controller.AnimationState state))
                currentAttack.PassMessage(state);
        }
    }
}