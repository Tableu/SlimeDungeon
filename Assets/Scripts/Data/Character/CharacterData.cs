using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller.Player
{
    [CreateAssetMenu(fileName = "CharacterData", menuName = "Data/CharacterData")]
    [Serializable]
    public class CharacterData : ScriptableObject
    {
        [Header("Stats")]
        [SerializeField] private float health;
        [SerializeField] private float speed;
        [SerializeField] private Elements.Type elementType;
        [SerializeField] private int cost;
        [SerializeField] private Vector3 spellOffset;
        [SerializeField] private Vector2 maxVelocity;
        [SerializeField] private new string name;
        
        [Header("References")]
        [SerializeField] private GameObject model;
        [SerializeField] private AttackData basicAttack;
        [FormerlySerializedAs("spells")] [SerializeField] private List<AttackData> startingSpells;
        
        public Vector2 MaxVelocity => maxVelocity;
        public GameObject Model => model;
        public AttackData BasicAttack => basicAttack;
        public List<AttackData> StartingSpells => startingSpells;
        public string Name => name;
        public float Health => health;
        public float Speed => speed;
        public Elements.Type ElementType => elementType;
        public Vector3 SpellOffset => spellOffset;
        public int Cost => cost;
    }
}