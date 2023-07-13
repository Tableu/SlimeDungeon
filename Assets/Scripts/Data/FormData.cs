using System;
using System.Collections.Generic;
using UnityEngine;

namespace Controller.Form
{
    [Serializable]
    public abstract class FormData : ScriptableObject
    {
        [SerializeField] private List<AttackData> attacks;

        public List<AttackData> Attacks => attacks;

        public abstract Form AttachScript(GameObject gameObject);
    }
}