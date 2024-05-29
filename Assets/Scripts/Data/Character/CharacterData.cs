using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller.Player
{
    [Serializable]
    public abstract class CharacterData : ScriptableObject
    {
        [Header("Stats")]
        [SerializeField] private float health;
        [SerializeField] private float speed;
        [SerializeField] private Elements.Type elementType;
        [SerializeField] private Vector3 spellOffset;
        [SerializeField] private Vector2 maxVelocity;
        [SerializeField] private new string name;
        
        [Header("References")]
        [SerializeField] private GameObject model;
        [FormerlySerializedAs("capturedForm")] 
        [FormerlySerializedAs("item")] 
        [SerializeField] private GameObject capturedCharacter;
        [SerializeField] private AttackData basicAttack;
        [SerializeField] private List<AttackData> spells;
        
        public Vector2 MaxVelocity => maxVelocity;
        public GameObject Model => model;
        public GameObject CapturedCharacter => capturedCharacter;
        public AttackData BasicAttack => basicAttack;
        public List<AttackData> Spells => spells;
        public string Name => name;
        public float Health => health;
        public float Speed => speed;
        public Elements.Type ElementType => elementType;
        public Vector3 SpellOffset => spellOffset;

        public abstract CharacterAnimator AttachScript(GameObject gameObject);
    }
}