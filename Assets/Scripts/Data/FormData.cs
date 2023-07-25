using System;
using UnityEngine;

namespace Controller.Form
{
    [Serializable]
    public abstract class FormData : ScriptableObject
    {
        [HeaderAttribute("Stats")]
        [SerializeField] private Elements.Type elementType;
        [SerializeField] private float health;
        [SerializeField] private float speed;
        
        [HeaderAttribute("References")]
        [SerializeField] private GameObject model;
        [SerializeField] private GameObject item;
        [SerializeField] private Vector3 spellOffset;

        public Elements.Type ElementType => elementType;
        public float Health => health;
        public float Speed => speed;
        public GameObject Model => model;
        public Vector3 SpellOffset => spellOffset;
        public GameObject Item => item;

        public abstract Form AttachScript(GameObject gameObject);
    }
}