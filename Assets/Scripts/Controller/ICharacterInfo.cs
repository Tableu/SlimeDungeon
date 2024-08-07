using Systems.Modifiers;
using UnityEngine;
using Type = Elements.Type;

namespace Controller
{
    public interface ICharacterInfo
    {
        public ModifiableStat Speed
        {
            get;
        }
        public float Health
        {
            get;
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

        public Transform Transform
        {
            get;
        }
    }
}