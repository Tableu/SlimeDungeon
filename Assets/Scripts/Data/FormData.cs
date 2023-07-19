using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.Form
{
    [Serializable]
    public abstract class FormData : ScriptableObject
    {
        [SerializeField] private List<AttackData> attacks;
        [SerializeField] private Elements.Type elementType;

        public List<AttackData> Attacks => attacks;
        public Elements.Type ElementType => elementType;

        public abstract Form AttachScript(GameObject gameObject);
    }
}