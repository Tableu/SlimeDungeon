using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller.Form
{
    [Serializable]
    public abstract class FormData : ScriptableObject
    {
        [HeaderAttribute("Stats")]
        [SerializeField] private Elements.Type elementType;
        [SerializeField] private float health;
        [SerializeField] private float speed;
        [SerializeField] private Vector2 maxVelocity;
        
        [HeaderAttribute("References")]
        [SerializeField] private GameObject model;
        [FormerlySerializedAs("item")] [SerializeField] private GameObject capturedForm;
        [SerializeField] private Vector3 spellOffset;
        [SerializeField] private AttackData basicAttack;
        [SerializeField] private List<AttackData> spells;

        public Elements.Type ElementType => elementType;
        public float Health => health;
        public float Speed => speed;
        public Vector2 MaxVelocity => maxVelocity;
        public GameObject Model => model;
        public Vector3 SpellOffset => spellOffset;
        public GameObject CapturedForm => capturedForm;
        public AttackData BasicAttack => basicAttack;
        public List<AttackData> Spells => spells;

        public abstract FormAnimator AttachScript(GameObject gameObject);
    }
}