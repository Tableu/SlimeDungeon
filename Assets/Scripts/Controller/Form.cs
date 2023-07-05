using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controller
{
    [Serializable]
    public abstract class Form : ScriptableObject
    {
        [SerializeField] protected List<Attack> attacks;
        protected Character character;
        public abstract void Equip(Character character);
        public abstract void Drop();
    }
}