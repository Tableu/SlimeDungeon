using System.Collections.Generic;
using Controller;
using Controller.Form;
using UnityEngine;

namespace Controller.Form
{
    public abstract class FormData : ScriptableObject
    {
        [SerializeField] private List<AttackData> attacks;

        public List<AttackData> Attacks => attacks;

        public abstract Form AttachScript(GameObject gameObject);
    }
}