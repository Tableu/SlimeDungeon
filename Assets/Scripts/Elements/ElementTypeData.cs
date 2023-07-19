using System;
using UnityEngine;

namespace Elements
{
    [Flags]
    public enum Type
    {
        None = 0,
        Fire = 1,
        Water = 2,
        Grass = 4
    }
    [CreateAssetMenu(fileName = "Element Data", menuName = "Data/Element Data")]
    [Serializable]
    public class ElementTypeData : ScriptableObject
    {
        [SerializeField] private Type advantage;
        [SerializeField] private Type disadvantage;
        [SerializeField] private Type elementType;

        public Type Advantage => advantage;
        public Type Disadvantage => disadvantage;
        public Type ElementType => elementType;
    }
}

