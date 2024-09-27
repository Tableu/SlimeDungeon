using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controller.Player
{
    [CreateAssetMenu(fileName = "PlayerCharacterData", menuName = "Data/PlayerCharacterData")]
    [Serializable]
    public class PlayerCharacterData : ScriptableObject, ILevelData
    {
        [Header("Stats")]
        [SerializeField] private float health;
        [SerializeField] private float speed;
        [SerializeField] private Elements.Type elementType;
        [SerializeField] private int cost;
        [SerializeField] private Vector3 spellOffset;
        [SerializeField] private Vector2 maxVelocity;
        [SerializeField] private new string name;

        [Header("Level Experience Requirements")] 
        [SerializeField] private List<int> experienceLevels;
        
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

        public int GetExperienceRequirement(int level)
        {
            if (experienceLevels == null || experienceLevels.Count >= level)
            {
                return 0;
            }
            return experienceLevels[level];
        }
    }
}