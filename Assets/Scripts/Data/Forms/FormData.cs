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
        [SerializeField] private float captureDifficulty = 1;
        
        [HeaderAttribute("References")]
        [SerializeField] private GameObject model;
        [FormerlySerializedAs("item")] [SerializeField] private GameObject capturedForm;
        [SerializeField] private Sprite icon;
        [SerializeField] private Vector3 spellOffset;
        [SerializeField] private AttackData basicAttack;
        [SerializeField] private List<AttackData> spells;

        public Elements.Type ElementType => elementType;
        public float Health => health;
        public float Speed => speed;
        public Vector2 MaxVelocity => maxVelocity;
        public float CaptureDifficulty => captureDifficulty;
        public GameObject Model => model;
        public Vector3 SpellOffset => spellOffset;
        public GameObject CapturedForm => capturedForm;
        public Sprite Icon => icon;
        public AttackData BasicAttack => basicAttack;
        public List<AttackData> Spells => spells;

        public abstract FormAnimator AttachScript(GameObject gameObject);
    }
}