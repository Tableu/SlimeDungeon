using System;
using UnityEditor.Animations;
using UnityEngine;

namespace Controller.Form
{
    [Serializable]
    public abstract class FormData : ScriptableObject
    {
        [SerializeField] private Elements.Type elementType;
        [SerializeField] private GameObject model;
        [SerializeField] private AnimatorController animatorController;
        [SerializeField] private Avatar avatar;

        public Elements.Type ElementType => elementType;
        public GameObject Model => model;
        public AnimatorController AnimatorController => animatorController;
        public Avatar Avatar => avatar;

        public abstract Form AttachScript(GameObject gameObject);
    }
}